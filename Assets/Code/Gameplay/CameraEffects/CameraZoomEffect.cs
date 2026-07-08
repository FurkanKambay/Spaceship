using System;
using UnityEngine;

namespace FK.Spaceship.Gameplay.CameraEffects
{
    /// <seealso cref="CameraZoomEffect"/>
    public interface ICameraZoomConfigProvider
    {
        ref readonly CameraZoomEffect.ZoomConfig CameraZoomConfig { get; }
    }

    /// <remarks>Modifies the camera's Local Rotation, FOV.</remarks>
    /// <seealso cref="ICameraZoomConfigProvider"/>
    [Serializable]
    public struct CameraZoomEffect : ICameraEffect
    {
        [Serializable]
        public struct ZoomConfig
        {
            [Range(10, 100)] public float slowFOV;
            [Range(10, 100)] public float fastFOV;
            public Vector3 fastAngles;
        }

        [SerializeField, Range(10, 100)] private float fov;

        private Camera camera;
        private ShipController ship;
        private ICameraZoomConfigProvider config;

        private float initialCameraFOV;

        public CameraZoomEffect(Camera camera, ShipController ship, ICameraZoomConfigProvider configProvider)
        {
            this.camera = camera ? camera : throw new ArgumentNullException(nameof(camera));
            this.ship = ship ? ship : throw new ArgumentNullException(nameof(ship));
            this.config = configProvider ?? throw new ArgumentNullException(nameof(configProvider));

            if (config.CameraZoomConfig.slowFOV == 0 || config.CameraZoomConfig.fastFOV == 0)
                throw new ArgumentOutOfRangeException(nameof(configProvider));

            this.initialCameraFOV = camera.fieldOfView;
            this.fov = initialCameraFOV;
        }

        public bool Tick()
        {
            if (!camera)
                return true;

            ref readonly ZoomConfig zoomConfig = ref config.CameraZoomConfig;

            float t = easeInExpo(ship.ThrustRatio);
            fov = Mathf.Lerp(zoomConfig.slowFOV, zoomConfig.fastFOV, t);
            camera.fieldOfView = fov;

            var angles = Vector3.Lerp(Vector3.zero, zoomConfig.fastAngles, t);
            camera.transform.localEulerAngles = angles;

            return false; // sustain the effect indefinitely

            static float easeInExpo(float x) => x == 0 ? 0 : Mathf.Pow(2, (10 * x) - 10);
        }

        public void Stop()
        {
            fov = initialCameraFOV;
        }

        public void Cleanup()
        {
            Stop();
            camera.fieldOfView = initialCameraFOV;
        }
    }
}
