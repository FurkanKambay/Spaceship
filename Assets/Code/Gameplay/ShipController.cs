using UnityEngine;
using UnityEngine.InputSystem;
using Vertx.Debugging;

namespace FK.Spaceship.Gameplay
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
        [SerializeField, Range(-180, 180)] private float minAngleZ = -45f;
        [SerializeField, Range(-180, 180)] private float maxAngleZ = 45f;

        [Header("Debug")]
        [SerializeField, Pair("Left", "Right")] private Vector2 thrusterInput;

        [Header("Debug - Thrust")]
        [SerializeField, Pair("Left", "Right")] private Vector2 currentThrustForce;
        [SerializeField] private float totalThrustForce;

        [Header("Debug - Turn")]
        [SerializeField, Pair("Left", "Right")] private Vector2 currentTurnForce;
        [SerializeField] private float totalTurnForce;
        [SerializeField] private float currentAngleZ;

        private void Awake()
        {
            leftInput.asset.Enable();
        }

        private void Update()
        {
            Duo input = new Duo(leftInput.action.ReadValue<float>(), rightInput.action.ReadValue<float>());
            if (swapSides)
                input.Swap();

            Duo thrustForce = input * thrusterForce;
            Duo turnForce = input * thrusterTorque;

            Vector2 leftDirection = leftThruster.up * thrustForce.Left;
            Vector2 rightDirection = rightThruster.up * thrustForce.Right;

            (thrusterInput, currentThrustForce, currentTurnForce) = (input, thrustForce, turnForce);
            D.raw(new Shape.Arrow2D(leftThruster.position, leftDirection));
            D.raw(new Shape.Arrow2D(rightThruster.position, rightDirection));

            Move(thrustForce);
            Turn(turnForce);
        }

        private void Move(Duo thrusts)
        {
            totalThrustForce = thrusts.Left + thrusts.Right;
            transform.Translate(0, totalThrustForce * Time.deltaTime, 0, Space.Self);
        }

        private void Turn(Duo torques)
        {
            totalTurnForce = torques.Right - torques.Left;
            currentAngleZ = Mathf.Clamp(currentAngleZ + (totalTurnForce * Time.deltaTime), minAngleZ, maxAngleZ);
            transform.localEulerAngles = new Vector3(0, 0, currentAngleZ);
        }
    }
}
