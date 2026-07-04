using FK.Common.Extensions;
using UnityEngine;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target;

        [Header("Config")]
        [SerializeField] private float offsetZ = -10;
        [SerializeField] Vector2 offset;
        [SerializeField] Vector2 followDecay;

        [Header("Debug")]
        [SerializeField, ReadOnlyField] private Vector3 destination;

        internal void InjectTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (!target)
                return;

            destination = target.position + (Vector3)offset;
            PanTowardsDestinationWithDecay(Time.deltaTime);
        }

        private void PanTowardsDestinationWithDecay(float deltaTime)
        {
            Vector3 pos = transform.position;
            float x = pos.x.ExpDecay(destination.x, followDecay.x, deltaTime);
            float y = pos.y.ExpDecay(destination.y, followDecay.y, deltaTime);
            transform.position = new Vector3(x, y, offsetZ);
        }
    }
}
