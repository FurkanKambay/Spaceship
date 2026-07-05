using FK.Common;
using FK.Common.Extensions;
using FK.Spaceship.Gameplay.Data;
using UnityEditor;
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

        public float DesiredThrust => desiredThrust;
        public float TotalThrust => totalThrust;
        public float MaxTotalThrust => stats.MaxTotalThrust;
        public float DesiredYaw => desiredYaw;
        public float Yaw => yaw;

        private Vector3 shipVelocity;
        private Quaternion shipRotation;

        internal void SetLevel(Duo bounds) => levelBounds = bounds;

        private void Awake()
        {
            Assert.IsNotNull(stats);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            shipRotation = FindDirection(deltaTime);
            shipVelocity = FindVelocity(deltaTime);
            transform.position = FindNextPosition(deltaTime);

            D.raw(new Shape.Arrow2D(transform.position, shipVelocity), Color.green);
            D.raw(new Shape.Arrow2D(transform.position, desiredYaw + 90), Color.softYellow);
        }

        private void LateUpdate()
        {
            UpdateVisuals();
        }

#region Locomotion
        private Quaternion FindDirection(float deltaTime)
        {
            // determine desired yaw
            float turn = (moveInputs.Left - moveInputs.Right) * (invertControls ? -1 : 1);
            desiredYaw = turn.Remap(-1, 1).To(stats.YawLimits.x, stats.YawLimits.y);

            // move towards desired yaw
            yaw = yaw.ExpDecay(desiredYaw, stats.TurnSpeed, deltaTime);
            if (Mathf.Abs(desiredYaw - yaw) < 0.1)
                yaw = desiredYaw;

            return Quaternion.Euler(0, 0, yaw);
        }

        private Vector3 FindVelocity(float deltaTime)
        {
            // determine desired thrust
            float manualThrust = (moveInputs.Left + moveInputs.Right) * stats.ThrusterForce;
            desiredThrust = Mathf.Min(stats.MaxTotalThrust, stats.BaseThrust + manualThrust);

            // move towards desired thrust
            totalThrust = totalThrust.ExpDecay(desiredThrust, stats.Acceleration, deltaTime);
            if (Mathf.Abs(desiredThrust - totalThrust) < 0.1)
                totalThrust = desiredThrust;

            // apply forward movement
            var localVelocity = new Vector3(0, totalThrust, 0);
            return shipRotation * localVelocity;
        }

        private Vector3 FindNextPosition(float deltaTime)
        {
            // determine final position for this frame
            Vector3 desiredPosition = transform.position + (shipVelocity * deltaTime);

            // incorporate dashing
            FindDashVelocity(deltaTime);
            desiredPosition.x += dashVelocity * deltaTime;

            // confine ship to level bounds
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, levelBounds.Left, levelBounds.Right);

            return desiredPosition;
        }

        private void FindDashVelocity(float deltaTime)
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
#endregion

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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.yellow;
            Handles.DrawSolidDisc(leftThruster.position, Vector3.forward, moveInputs.Left * 0.1f);
            Handles.DrawSolidDisc(rightThruster.position, Vector3.forward, moveInputs.Right * 0.1f);
        }
#endif
    }
}
