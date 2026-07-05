using FK.Common.Extensions;
using UnityEngine;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [Header("Config - Follow")]
        [SerializeField] private float offsetZ = -10;
        [SerializeField] private Vector2 offset;
        [SerializeField] private Vector2 followDecay;

        [Header("Config - Zoom")]
        [SerializeField, Range(0, 5)] private float zoomWhenSlow = 1f;
        [SerializeField, Range(0, 5)] private float zoomWhenFast = 1.5f;

        [Header("Debug")]
        [SerializeField, ReadOnlyField] private ShipController ship;
        [SerializeField, ReadOnlyField] private Vector3 destination;
        [SerializeField, ReadOnlyField, Range(0, 5)] private float zoom = 1f;

        private Camera camera;
        private float initialCameraSize;

        internal void Inject(ShipController shipController) =>
            ship = shipController;

        private void Awake()
        {
            camera = Camera.main;
            initialCameraSize = camera!.orthographicSize;
        }

        private void LateUpdate()
        {
            if (!ship)
                return;

            destination = ship.transform.position + offset.WithZ(offsetZ);
            PanTowardsDestinationWithDecay(Time.deltaTime);

            AdjustZoom();
        }

        private void PanTowardsDestinationWithDecay(float deltaTime)
        {
            Vector3 current = transform.position;

            var position = new Vector3(
                x: current.x.ExpDecay(destination.x, followDecay.x, deltaTime),
                y: current.y.ExpDecay(destination.y, followDecay.y, deltaTime),
                z: offsetZ
            );

            transform.position = position;
        }

        private void AdjustZoom()
        {
            float t = easeInExpo(ship.TotalThrust / ship.MaxTotalThrust);
            zoom = Mathf.Lerp(zoomWhenSlow, zoomWhenFast, t);

            camera.orthographicSize = initialCameraSize * zoom;
            return;

            static float easeInExpo(float x) => x == 0 ? 0 : Mathf.Pow(2, (10 * x) - 10);
        }
    }
}
