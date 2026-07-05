using System;
using UnityEngine;

namespace FK.Spaceship.Gameplay.CameraEffects
{
    [Serializable]
    public class CameraZoomEffect : ICameraEffect
    {
        [SerializeField, Range(0, 5)] private float zoom;

        private Camera camera;
        private ShipController ship;
        private float initialCameraSize;
        private float zoomWhenSlow;
        private float zoomWhenFast;

        public CameraZoomEffect(Camera camera, ShipController ship, float zoomWhenSlow, float zoomWhenFast)
        {
            this.camera = camera ? camera : throw new ArgumentNullException(nameof(camera));
            this.ship = ship ? ship : throw new ArgumentNullException(nameof(ship));
            this.initialCameraSize = camera.orthographicSize;
            this.zoomWhenSlow = zoomWhenSlow;
            this.zoomWhenFast = zoomWhenFast;
            this.zoom = 1f;
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
