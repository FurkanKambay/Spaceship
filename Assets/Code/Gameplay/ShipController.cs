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
        [SerializeField, ReadOnlyField] private Duo thrustLevers;
        [SerializeField, ReadOnlyField] private Duo desiredThrusts;
        [SerializeField, ReadOnlyField] private float totalThrust;
        [SerializeField, ReadOnlyField] private Vector3 dashVelocity;

        [Header("Debug - Turning")]
        [SerializeField, ReadOnlyField, Range(-1, 1)] private float turnDirection;
        [SerializeField, ReadOnlyField] private float desiredTorque;
        [SerializeField, ReadOnlyField] private float torque;
        [SerializeField, ReadOnlyField] private float roll;

        [Header("Debug - Final Vectors")]
        [SerializeField, ReadOnlyField] private Vector3 shipForward;
        [SerializeField, ReadOnlyField] private Vector3 shipVelocity;

        public Duo ThrustLevers => thrustLevers;
        public bool AtFullThrust => totalThrust >= stats.MaxTotalThrust;
        public float ThrustRatio => totalThrust / stats.MaxTotalThrust;

        private void Awake()
        {
            Assert.IsNotNull(stats);
            shipForward = transform.forward;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // Gather inputs
            thrustLevers = invertControls ? moveInputs.Swapped : moveInputs;
            turnDirection = thrustLevers.Left - thrustLevers.Right;

            // Determine desired values
            desiredThrusts = thrustLevers * stats.EnginePower;
            desiredTorque = -turnDirection * stats.TurnSpeed;

            // Move toward desired values
            torque = torque.ExpDecay(desiredTorque, stats.TurnDecay, deltaTime);
            torque = torque.Snap(desiredTorque, 1f);

            float targetThrust = Mathf.Min(desiredThrusts.Sum, stats.MaxTotalThrust);
            bool isDecelerating = Mathf.Sign(targetThrust - totalThrust) < 0;
            float velocityChange = isDecelerating ? stats.ThrustDeceleration : stats.ThrustAcceleration;
            totalThrust = totalThrust.MoveTowards(targetThrust, velocityChange * deltaTime);

            // Find new orientation
            shipForward = Quaternion.AngleAxis(-torque * deltaTime, Vector3.up) * shipForward;
            var shipRotation = Quaternion.LookRotation(shipForward);

            // Find velocity
            Vector3 moveVelocity = shipForward * totalThrust;
            FindDashVelocity(deltaTime);
            shipVelocity = moveVelocity + (shipRotation * dashVelocity);

            // Find final values
            Vector3 finalPosition = transform.position + (shipVelocity * deltaTime);
            roll = torque.Remap(-stats.TurnSpeed, +stats.TurnSpeed).To(stats.RollLimits.x, stats.RollLimits.y);
            var finalRotation = Quaternion.Euler(shipRotation.eulerAngles.With(z: roll));

            // Move and rotate the ship
            transform.SetLocalPositionAndRotation(finalPosition, finalRotation);

            D.raw(new Shape.Arrow(transform.position, moveVelocity), Color.green);
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
