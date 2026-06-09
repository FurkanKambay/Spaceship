using UnityEngine;
using UnityEngine.InputSystem;
using Vertx.Debugging;

namespace Spaceship
{
    public class ShipController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference leftInput;
        [SerializeField] private InputActionReference rightInput;
        [SerializeField] private bool swapSides = true;

        [Header("Prefab References")]
        [SerializeField] private Transform leftThruster;
        [SerializeField] private Transform rightThruster;

        [Header("Config")]
        [SerializeField, Min(0)] private float thrusterForce = 20f;
        [SerializeField, Min(0)] private float thrusterTorque = 20f;

        [Header("Debug")]
        [SerializeField] private Vector2 thrusterInput;
        [SerializeField] private Vector2 currentForces;
        [SerializeField] private Vector2 currentTorques;
        [SerializeField] private float currentTurnAmount;

        private void Awake()
        {
            leftInput.asset.Enable();
        }

        private void Update()
        {
            Duo input = new Duo(leftInput.action.ReadValue<float>(), rightInput.action.ReadValue<float>());
            if (swapSides)
                input.Swap();

            Duo force = input * thrusterForce;
            Duo torque = input * thrusterTorque;

            Vector2 leftDirection = leftThruster.up * force.Left;
            Vector2 rightDirection = rightThruster.up * force.Right;

            (thrusterInput, currentForces, currentTorques) = (input, force, torque);
            D.raw(new Shape.Arrow2D(leftThruster.position, leftDirection));
            D.raw(new Shape.Arrow2D(rightThruster.position, rightDirection));

            Move(force);
            Turn(torque);
        }

        private void Move(Duo thrusts)
        {
            float totalForce = thrusts.Left + thrusts.Right;

            transform.Translate(0, totalForce * Time.deltaTime, 0, Space.Self);
        }

        private void Turn(Duo torques)
        {
            float totalTorque = torques.Right - torques.Left;
            currentTurnAmount = totalTorque;

            transform.Rotate(0, 0, totalTorque * Time.deltaTime);
        }
    }
}
