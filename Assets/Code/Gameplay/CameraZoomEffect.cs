using System;
using UnityEngine;

namespace FK.Spaceship.Gameplay
{
    [Serializable]
    public class CameraZoomEffect : ICameraEffect
    {
        [SerializeField] private Camera camera;
        [SerializeField, Range(0, 5)] private float zoom = 1f;

        [Header("Config - Zoom")]
        [SerializeField, Range(0, 5)] private float zoomWhenSlow;
        [SerializeField, Range(0, 5)] private float zoomWhenFast;

        private float initialCameraSize;
        private ShipController ship;

        public CameraZoomEffect(Camera camera, ShipController ship, float zoomWhenSlow, float zoomWhenFast)
        {
            this.camera = camera ? camera : throw new ArgumentNullException(nameof(camera));
            this.ship = ship ? ship : throw new ArgumentNullException(nameof(ship));
            this.initialCameraSize = camera.orthographicSize;
            this.zoom = 1f;

            this.zoomWhenSlow = zoomWhenSlow;
            this.zoomWhenFast = zoomWhenFast;
        }

        public bool Tick()
        {
            float t = easeInExpo(ship.TotalThrust / ship.MaxTotalThrust);
            zoom = Mathf.Lerp(zoomWhenSlow, zoomWhenFast, t);

            camera.orthographicSize = initialCameraSize * zoom;
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
