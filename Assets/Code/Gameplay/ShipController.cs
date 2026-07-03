using FK.Common;
using FK.Common.Extensions;
using UnityEngine;
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
        [SerializeField, Min(0)] private float thrusterForce = 20f;
        [SerializeField, Min(0)] private float baseThrust = 1f;
        [SerializeField, Range(-180, 180)] private float minAngleZ = -45f;
        [SerializeField, Range(-180, 180)] private float maxAngleZ = 45f;

        [Header("Debug - Input")]
        [SerializeField, ReadOnlyField] private Duo moveInput;
        [SerializeField, ReadOnlyField] private Duo thrust;
        [SerializeField, ReadOnlyField] private float thrustPower;

        [Header("Debug - Turning")]
        [SerializeField, ReadOnlyField] private float yaw;
        // TODO: turn speed, decay, damping

        private void Awake()
        {
            leftInput.asset.Enable();
        }

        private void Update()
        {
            ReadInput();
            Move();
            Turn();
        }

        private void LateUpdate()
        {
            D.raw(new Shape.Arrow2D(leftThruster.position, leftThruster.up * moveInput.Left));
            D.raw(new Shape.Arrow2D(rightThruster.position, rightThruster.up * moveInput.Right));
        }

        private void ReadInput()
        {
            moveInput = new Duo(leftInput.action.ReadValue<float>(), rightInput.action.ReadValue<float>());
            if (invertControls)
                moveInput.Swap();
        }

        private void Move()
        {
            thrust = moveInput * thrusterForce;

            thrustPower = baseThrust + thrust.Left + thrust.Right;
            transform.Translate(0, thrustPower * Time.deltaTime, 0);
        }

        private void Turn()
        {
            float turn = moveInput.Left - moveInput.Right;
            yaw = turn.Remap(-1, 1).To(minAngleZ, maxAngleZ);

            transform.localEulerAngles = new Vector3(0, 0, yaw);
        }
    }
}
