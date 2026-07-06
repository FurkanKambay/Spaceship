using FK.Common;
using FK.Spaceship.Gameplay.CameraEffects;
using FK.Spaceship.Gameplay.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay
{
    public class ShipFeelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipController ship;

        [Header("Config - Follow")]
        [SerializeField] private Vector3 cameraFollowOffset;
        [SerializeField] private Vector2 cameraFollowDecay;

        [Header("Config - Zoom")]
        [SerializeField] private CameraZoomEffect.ZoomConfig cameraZoomConfig;

        [Header("Config - Max Thrust")]
        [SerializeField] private CameraShakeProfileAsset cameraShakeAtMaxThrust;
        [SerializeField, Min(0)] private float rumbleDelay;
        [SerializeField] private RumbleProfile rumbleAtMaxThrust;

        [Header("Config - Collision")]
        [SerializeField] private CameraShakeProfileAsset collisionShakeProfile;

        [Header("Debug")]
        [SerializeField, ReadOnlyField, Range(0, 1)] private float rumbleLowFrequency;
        [SerializeField, ReadOnlyField, Range(0, 1)] private float rumbleHighFrequency;

        [Header("Debug - Camera Effects")]
        [SerializeField, ReadOnlyField] private Camera camera;
        [SerializeField, ReadOnlyField] private CameraFollowEffect camFollow;
        [SerializeField, ReadOnlyField] private CameraZoomEffect camZoom;
        [SerializeField, ReadOnlyField] private CameraShakeEffect camShake;

        private float fullThrustTimer;

        private void Awake()
        {
            camera = Camera.main;
            Assert.IsNotNull(camera);

            Transform cameraPivot = camera.transform.parent;

            camFollow = new CameraFollowEffect(cameraPivot, ship.transform, cameraFollowOffset, cameraFollowDecay);
            camZoom = new CameraZoomEffect(camera, ship, cameraZoomConfig);
            camShake = new CameraShakeEffect(camera, cameraShakeAtMaxThrust);
        }

        private void OnDisable()
        {
            InputSystem.ResetHaptics();
        }

        private void LateUpdate()
        {
            CheckForMaxThrust();

            camFollow.Tick();
            camZoom.Tick();
            camShake.Tick();
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
            if (fullThrustTimer > rumbleDelay)
                ActivateMaxThrustEffects();
        }

        private void ActivateMaxThrustEffects()
        {
            // Camera Shake
            camShake.AddTrauma(1);

            // Rumble
            if (Gamepad.current?.IsActuated() ?? false)
            {
                rumbleLowFrequency = rumbleAtMaxThrust.lowFrequency;
                rumbleHighFrequency = rumbleAtMaxThrust.highFrequency;
                UpdateRumble();
            }
        }

        private void DeactivateMaxThrustEffects()
        {
            camShake.Stop();
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
