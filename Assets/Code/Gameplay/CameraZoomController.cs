using UnityEngine;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay
{
    public class CameraZoomController : MonoBehaviour
    {
        [Header("Config - Zoom")]
        [SerializeField, Range(0, 5)] float zoomWhenSlow = 1f;
        [SerializeField, Range(0, 5)] float zoomWhenFast = 1.5f;

        [Header("Debug")]
        [SerializeField, ReadOnlyField] private ShipController ship;
        [SerializeField, Range(0, 5)] private float zoom = 1f;

        private Camera camera;
        private float initialCameraSize;

        internal void Inject(ShipController shipController) => ship = shipController;

        private void Awake()
        {
            camera = Camera.main;
        }

        private void OnEnable()
        {
            initialCameraSize = camera!.orthographicSize;
        }

        private void Update()
        {
            float t = Mathf.Pow(ship.TotalThrust / ship.MaxTotalThrust, 4);
            zoom = Mathf.Lerp(zoomWhenSlow, zoomWhenFast, t);

            camera.orthographicSize = initialCameraSize * zoom;
        }
    }
}
