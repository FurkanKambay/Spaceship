using FK.Common;
using FK.Common.Extensions;
using FK.Spaceship.Gameplay.Data;
using UnityEngine;
using UnityEngine.Assertions;
using Vertx.Attributes;
using Vertx.Debugging;
using IPlayerActions = FK.Spaceship.Controls.IPlayerActions;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace FK.Spaceship.Gameplay
{
    public class ShipController : MonoBehaviour, IPlayerActions
    {
        [Header("Prefab References")]
        [SerializeField] private Transform leftThruster;
        [SerializeField] private Transform rightThruster;

        [Header("Config")]
        [SerializeField] private bool invertControls;
        [SerializeField] private ShipStats stats;

        [Header("Debug - Input")]
        [SerializeField, ReadOnlyField] private Duo moveInputs;
        [SerializeField, ReadOnlyField] private Duo dashInputs;

        [Header("Debug - Thrust")]
        [SerializeField, ReadOnlyField] private float desiredThrust;
        [SerializeField, ReadOnlyField] private float totalThrust;
        [SerializeField, ReadOnlyField] private float dashVelocity;

        [Header("Debug - Turning")]
        [SerializeField, ReadOnlyField] private float desiredYaw;
        [SerializeField, ReadOnlyField] private float yaw;
        // TODO: turn speed, decay, damping

        [Header("Debug - Level")]
        [SerializeField] private Duo levelBounds;

        internal void SetLevel(Duo bounds) => levelBounds = bounds;

        private void Awake()
        {
            Assert.IsNotNull(stats);
        }

        private void Update()
        {
            Quaternion rotation = Turn(Time.deltaTime);
            Vector3 position = Move(Time.deltaTime);
            transform.SetPositionAndRotation(position, rotation);
        }

        private void LateUpdate()
        {
            UpdateVisuals();
#if UNITY_EDITOR
            D.raw(new Shape.Arrow2D(leftThruster.position, leftThruster.up * moveInputs.Left));
            D.raw(new Shape.Arrow2D(rightThruster.position, rightThruster.up * moveInputs.Right));
#endif
        }

        private Quaternion Turn(float deltaTime)
        {
            float turn = (moveInputs.Left - moveInputs.Right) * (invertControls ? -1 : 1);
            desiredYaw = turn.Remap(-1, 1).To(stats.YawLimits.x, stats.YawLimits.y);

            yaw = yaw.ExpDecay(desiredYaw, stats.TurnSpeed, deltaTime);
            if (Mathf.Abs(desiredYaw - yaw) < 0.1)
                yaw = desiredYaw;

            return Quaternion.Euler(transform.forward * yaw);
        }

        private Vector3 Move(float deltaTime)
        {
            float manualThrust = (moveInputs.Left + moveInputs.Right) * stats.ThrusterForce;
            desiredThrust = Mathf.Min(stats.MaxTotalThrust, stats.BaseThrust + manualThrust);

            totalThrust = totalThrust.ExpDecay(desiredThrust, stats.Acceleration, deltaTime);
            if (Mathf.Abs(desiredThrust - totalThrust) < 0.1)
                totalThrust = desiredThrust;

            // apply movement
            var localMovement = new Vector3(0, totalThrust * deltaTime, 0);
            Vector3 desiredPosition = transform.position + transform.TransformDirection(localMovement);

            HandleDash(deltaTime);
            desiredPosition.x += dashVelocity * deltaTime;

            // confine to level bounds
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, levelBounds.Left, levelBounds.Right);

            return desiredPosition;
        }

        private void HandleDash(float deltaTime)
        {
            if (Mathf.Abs(dashVelocity) > 0)
            {
                dashVelocity = dashVelocity.ExpDecay(0, stats.DashDecay, deltaTime);
                if (Mathf.Abs(dashVelocity) < 0.001)
                    dashVelocity = 0;
            }
            else
            {
                float dashDirection = dashInputs.Right - dashInputs.Left;
                if (dashDirection != 0)
                    dashVelocity = dashDirection * stats.DashForce;
            }
        }

        private void UpdateVisuals()
        {
            // TODO: move visualization out
            leftThruster.gameObject.SetActive(moveInputs.Right > 0.1f);
            rightThruster.gameObject.SetActive(moveInputs.Left > 0.1f);
        }

        void IPlayerActions.OnMoveLeft(InputContext context) => moveInputs.Left = context.ReadValue<float>();
        void IPlayerActions.OnMoveRight(InputContext context) => moveInputs.Right = context.ReadValue<float>();

        void IPlayerActions.OnDashLeft(InputContext context) => dashInputs.Left = context.ReadValue<float>();
        void IPlayerActions.OnDashRight(InputContext context) => dashInputs.Right = context.ReadValue<float>();

        void IPlayerActions.OnAttack(InputContext context) => Log.Info("[Input] Attack");
        void IPlayerActions.OnPrevious(InputContext context) => Log.Info("[Input] Previous");
        void IPlayerActions.OnNext(InputContext context) => Log.Info("[Input] Next");
    }
}
