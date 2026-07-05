using FK.Spaceship.Gameplay.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay
{
    public class ShipFeelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipController ship;

        [Header("Config - Max Thrust")]
        [SerializeField] private CameraShakeProfileAsset maxThrustCameraShake;
        [SerializeField] private float maxThrustDuration = 1f;
        [SerializeField] private float maxThrustRumbleLow;
        [SerializeField] private float maxThrustRumbleHigh;

        [Header("Config - Collision")]
        [SerializeField] private CameraShakeProfileAsset collisionShakeProfile;

        [Header("Debug")]
        [SerializeField, ReadOnlyField] private CameraShake cameraShake;
        [SerializeField, Range(0, 1), ReadOnlyField] private float rumbleLowFrequency;
        [SerializeField, Range(0, 1), ReadOnlyField] private float rumbleHighFrequency;

        private float fullThrustTimer;

        internal void Inject(CameraShake newCameraShake) =>
            cameraShake = newCameraShake;

        private void OnDisable()
        {
            InputSystem.ResetHaptics();
        }

        private void Update()
        {
            CheckForMaxThrust();
        }

#region Max Thrust
        private void CheckForMaxThrust()
        {
            if (!ship || !cameraShake)
                return;

            if (ship.TotalThrust < ship.MaxTotalThrust)
            {
                if (fullThrustTimer > 0)
                    DeactivateMaxThrustEffects();

                fullThrustTimer = 0;
                return;
            }

            fullThrustTimer += Time.deltaTime;
            if (fullThrustTimer > maxThrustDuration)
                ActivateMaxThrustEffects();
        }

        private void ActivateMaxThrustEffects()
        {
            // Camera Shake
            cameraShake.SetProfile(maxThrustCameraShake);
            cameraShake.AddTrauma(1);

            // Rumble
            if (Gamepad.current?.IsActuated() ?? false)
            {
                rumbleLowFrequency = maxThrustRumbleLow;
                rumbleHighFrequency = maxThrustRumbleHigh;
                UpdateRumble();
            }
        }

        private void DeactivateMaxThrustEffects()
        {
            cameraShake.ClearTrauma();
            StopRumble();
        }
#endregion

#region Gamepad Rumble
        private void UpdateRumble() =>
            Gamepad.current?.SetMotorSpeeds(rumbleLowFrequency, rumbleHighFrequency);

        private void StopRumble()
        {
            rumbleLowFrequency = 0;
            rumbleHighFrequency = 0;
            UpdateRumble();
        }
#endregion
    }
}
