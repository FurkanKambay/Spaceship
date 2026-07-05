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
        [SerializeField, ReadOnlyField] private CameraShakeEffect cameraShake;
        [SerializeField, Range(0, 1), ReadOnlyField] private float rumbleLowFrequency;
        [SerializeField, Range(0, 1), ReadOnlyField] private float rumbleHighFrequency;

        private Camera camera;
        private float fullThrustTimer;

        private void Awake()
        {
            camera = Camera.main;
            cameraShake = new CameraShakeEffect(camera, maxThrustCameraShake);
        }

        private void OnDisable()
        {
            InputSystem.ResetHaptics();
        }

        private void Update()
        {
            cameraShake?.Tick();
            CheckForMaxThrust();
        }

#region Max Thrust
        private void CheckForMaxThrust()
        {
            if (!ship)
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
            cameraShake?.AddTrauma(1);

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
            cameraShake?.Stop();
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
