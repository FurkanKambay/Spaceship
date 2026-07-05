using FK.Common.Extensions;
using UnityEngine;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float offsetZ = -10;
        [SerializeField] private Vector2 offset;
        [SerializeField] private Vector2 followDecay;

        [Header("Debug")]
        [SerializeField] private Transform target;
        [SerializeField, ReadOnlyField] private Vector3 destination;

        internal void Inject(Transform newTarget) =>
            target = newTarget;

        private void LateUpdate()
        {
            if (!target)
                return;

            destination = target.position + offset.WithZ(offsetZ);;
            PanTowardsDestinationWithDecay(Time.deltaTime);
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
    }
}
