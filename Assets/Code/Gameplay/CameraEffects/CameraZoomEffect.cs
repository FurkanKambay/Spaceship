using System;
using UnityEngine;

namespace FK.Spaceship.Gameplay.CameraEffects
{
    public interface ICameraZoomConfigProvider
    {
        CameraZoomEffect.ZoomConfig CameraZoomConfig { get; }
    }

    [Serializable]
    public struct CameraZoomEffect : ICameraEffect
    {
        [Serializable]
        public struct ZoomConfig
        {
            [Range(0, 5)] public float slowZoom;
            [Range(0, 5)] public float fastZoom;
            public Vector3 fastAngles;
        }

        [SerializeField, Range(0, 5)] private float zoom;

        private Camera camera;
        private ShipController ship;
        private ICameraZoomConfigProvider config;

        private float initialCameraSize;

        public CameraZoomEffect(Camera camera, ShipController ship, ICameraZoomConfigProvider configProvider)
        {
            this.camera = camera ? camera : throw new ArgumentNullException(nameof(camera));
            this.ship = ship ? ship : throw new ArgumentNullException(nameof(ship));
            this.config = configProvider ?? throw new ArgumentNullException(nameof(configProvider));

            if (config.CameraZoomConfig.slowZoom == 0 || config.CameraZoomConfig.fastZoom == 0)
                throw new ArgumentOutOfRangeException(nameof(configProvider));

            this.initialCameraSize = camera.orthographicSize;
            this.zoom = 1f;
        }

        public bool Tick()
        {
            if (!camera)
                return true;

            ZoomConfig zoomConfig = config.CameraZoomConfig;

            float t = easeInExpo(ship.TotalThrust / ship.MaxTotalThrust);
            zoom = Mathf.Lerp(zoomConfig.slowZoom, zoomConfig.fastZoom, t);
            camera.orthographicSize = initialCameraSize * zoom;

            var angles = Vector3.Lerp(Vector3.zero, zoomConfig.fastAngles, t);
            camera.transform.eulerAngles = angles;

            return false; // sustain the effect indefinitely

            static float easeInExpo(float x) => x == 0 ? 0 : Mathf.Pow(2, (10 * x) - 10);
        }

        public void Stop()
        {
            zoom = 1;
        }

        public void Cleanup()
        {
            Stop();
            camera.orthographicSize = initialCameraSize;
        }
    }
}
