using FK.Common;
using FK.Common.Extensions;
using FK.Spaceship.Gameplay.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Vertx.Attributes;
using Vertx.Debugging;

namespace FK.Spaceship.Gameplay
{
    public class ShipController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference leftInput;
        [SerializeField] private InputActionReference rightInput;
        [SerializeField] private bool invertControls = true;

        [Header("Prefab References")]
        [SerializeField] private Transform leftThruster;
        [SerializeField] private Transform rightThruster;

        [Header("Config")]
        [SerializeField] private ShipStats stats;

        [Header("Debug - Input")]
        [SerializeField, ReadOnlyField] private Duo moveInputs;

        [Header("Debug - Thrust")]
        [SerializeField, ReadOnlyField] private float desiredThrust;
        [SerializeField, ReadOnlyField] private float totalThrust;

        [Header("Debug - Turning")]
        [SerializeField, ReadOnlyField] private float desiredYaw;
        [SerializeField, ReadOnlyField] private float yaw;
        // TODO: turn speed, decay, damping

        [Header("Debug - Level")]
        [SerializeField] private Duo levelBounds;

        internal void SetLevel(Duo bounds)
        {
            levelBounds = bounds;
        }

        private void Awake()
        {
            Assert.IsNotNull(stats);
            leftInput.asset.Enable();
        }

        private void Update()
        {
            ReadInput();
            Move(Time.deltaTime);
            Turn(Time.deltaTime);
        }

        private void ReadInput()
        {
            moveInputs = new Duo(leftInput.action.ReadValue<float>(), rightInput.action.ReadValue<float>());

            // TODO: move visualization out
            leftThruster.gameObject.SetActive(moveInputs.Right > 0.1f);
            rightThruster.gameObject.SetActive(moveInputs.Left > 0.1f);
        }

        private void Move(float deltaTime)
        {
            float manualThrust = (moveInputs.Left + moveInputs.Right) * stats.ThrusterForce;
            desiredThrust = Mathf.Min(stats.MaxTotalThrust, stats.BaseThrust + manualThrust);

            totalThrust = totalThrust.ExpDecay(desiredThrust, stats.Acceleration, deltaTime);

            // apply movement
            var localMovement = new Vector3(0, totalThrust * deltaTime, 0);
            Vector3 desiredPosition = transform.position + transform.TransformDirection(localMovement);

            // confine to level bounds
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, levelBounds.Left, levelBounds.Right);

            transform.position = desiredPosition;
        }

        private void Turn(float deltaTime)
        {
            float turn = (moveInputs.Left - moveInputs.Right) * (invertControls ? -1 : 1);
            desiredYaw = turn.Remap(-1, 1).To(stats.YawLimits.x, stats.YawLimits.y);

            yaw = yaw.ExpDecay(desiredYaw, stats.TurnSpeed, deltaTime);

            transform.localEulerAngles = new Vector3(0, 0, yaw);
        }

#if UNITY_EDITOR
        private void LateUpdate()
        {
            D.raw(new Shape.Arrow2D(leftThruster.position, leftThruster.up * moveInputs.Left));
            D.raw(new Shape.Arrow2D(rightThruster.position, rightThruster.up * moveInputs.Right));
        }
#endif
    }
}
