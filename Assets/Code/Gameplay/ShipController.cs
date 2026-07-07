using FK.Common;
using FK.Common.Extensions;
using FK.Spaceship.Gameplay.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Vertx.Attributes;
using Vertx.Debugging;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;
using IPlayerActions = FK.Spaceship.Controls.IPlayerActions;

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
        [SerializeField, ReadOnlyField] private Duo thrusterInput;
        [SerializeField, ReadOnlyField] private float thrustInputTotal;
        [SerializeField, ReadOnlyField] private float desiredThrust;
        [SerializeField, ReadOnlyField] private float thrust;
        [SerializeField, ReadOnlyField] private Vector3 dashVelocity;

        [Header("Debug - Turning")]
        [SerializeField, ReadOnlyField, Range(-1, 1)] private float turnInput;
        [SerializeField, ReadOnlyField] private float desiredTorque;
        [SerializeField, ReadOnlyField] private float torque;
        [SerializeField, ReadOnlyField] private float roll;

        [Header("Debug - Rot/Vel")]
        [SerializeField, ReadOnlyField] private Vector3 shipForward;
        [SerializeField, ReadOnlyField] private Quaternion shipRotation;
        [SerializeField, ReadOnlyField] private Vector3 shipVelocity;
        [SerializeField, ReadOnlyField] private Vector3 shipVelocityTotal;

        public Duo ThrusterInput => thrusterInput;
        public float Thrust => thrust;
        public float MaxTotalThrust => stats.MaxTotalThrust;

        private void Awake()
        {
            Assert.IsNotNull(stats);
            shipForward = transform.forward;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // Gather inputs
            thrusterInput = invertControls ? moveInputs.Swapped : moveInputs;
            turnInput = thrusterInput.Left - thrusterInput.Right;
            thrustInputTotal = moveInputs.Left + moveInputs.Right;

            // Determine desired values
            float manualThrust = thrustInputTotal * stats.ThrustSpeed;
            desiredTorque = -turnInput * stats.TurnSpeed;
            desiredThrust = Mathf.Min(stats.MaxTotalThrust, stats.BaseThrust + manualThrust);

            // Move toward desired values
            // torque = desiredTorque;
            // totalThrust = desiredThrust;
            torque = torque.ExpDecay(desiredTorque, stats.TurnDecay, deltaTime);
            thrust = thrust.ExpDecay(desiredThrust, stats.ThrustDecay, deltaTime);
            roll = torque.Remap(-stats.TurnSpeed, +stats.TurnSpeed).To(stats.RollLimits.x, stats.RollLimits.y);

            // Snap values when near target
            if (Mathf.Abs(desiredTorque - torque) < 0.001)
                torque = desiredTorque;
            if (Mathf.Abs(desiredThrust - thrust) < 0.01)
                thrust = desiredThrust;

            // Find new orientation
            shipForward = Quaternion.AngleAxis(-torque * deltaTime, Vector3.up) * shipForward;
            shipRotation = Quaternion.LookRotation(shipForward);
            // shipEulerAngles = shipRotation;

            // Find new position
            FindDashVelocity(deltaTime);
            shipVelocity = shipForward * thrust;
            shipVelocityTotal = shipVelocity + (shipRotation * dashVelocity);
            Vector3 finalPosition = transform.position + (shipVelocityTotal * deltaTime);

            // Move and rotate the ship
            var finalRotation = Quaternion.Euler(shipRotation.eulerAngles.With(z: roll));
            transform.SetLocalPositionAndRotation(finalPosition, finalRotation);

            D.raw(new Shape.Arrow(transform.position, shipVelocity), Color.green);
            D.raw(new Shape.Arrow(transform.position, shipRotation), Color.softYellow);
        }

        private void FindDashVelocity(float deltaTime)
        {
            if (Mathf.Abs(dashVelocity.sqrMagnitude) > 0)
            {
                dashVelocity = dashVelocity.ExpDecay(Vector3.zero, stats.DashDecay, deltaTime);
                if (Mathf.Abs(dashVelocity.sqrMagnitude) < 0.01)
                    dashVelocity = Vector3.zero;
            }
            else
            {
                float dashDirection = dashInputs.Right - dashInputs.Left;
                if (dashDirection != 0)
                    dashVelocity = Vector3.right * (dashDirection * stats.DashForce);
            }
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
