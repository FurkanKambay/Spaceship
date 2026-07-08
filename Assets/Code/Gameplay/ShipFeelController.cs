using FK.Spaceship.Gameplay.CameraEffects;
using FK.Spaceship.Gameplay.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay
{
    /// <seealso cref="ShipFeelAsset"/>
    public class ShipFeelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipController ship;

        [Header("Config")]
        [SerializeField] private ShipFeelAsset shipFeel;

        [Header("Debug - Gamepad Rumble")]
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

            camFollow = new CameraFollowEffect(cameraPivot, ship.transform, shipFeel);
            camZoom = new CameraZoomEffect(camera, ship, shipFeel);
            camShake = new CameraShakeEffect(camera, shipFeel.CameraShakeAtMaxThrust);
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

            if (!ship.AtFullThrust)
            {
                if (fullThrustTimer > 0)
                    DeactivateMaxThrustEffects();

                fullThrustTimer = 0;
                return;
            }

            fullThrustTimer += Time.deltaTime;
            if (fullThrustTimer > shipFeel.RumbleDelay)
                ActivateMaxThrustEffects();
        }

        private void ActivateMaxThrustEffects()
        {
            // Camera Shake
            camShake.AddTrauma(1);

            // Rumble
            if (Gamepad.current?.IsActuated() ?? false)
            {
                rumbleLowFrequency = shipFeel.RumbleAtMaxThrust.lowFrequency;
                rumbleHighFrequency = shipFeel.RumbleAtMaxThrust.highFrequency;
                UpdateRumble();
            }
        }

        private void DeactivateMaxThrustEffects()
        {
            camShake.Reset();
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
