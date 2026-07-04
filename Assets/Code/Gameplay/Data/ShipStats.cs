using UnityEngine;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay.Data
{
    [CreateAssetMenu(menuName = "Spaceship/Ship Stats")]
    public class ShipStats : ScriptableObject
    {
        [SerializeField, Min(0)] private float thrusterForce = 20f;
        [SerializeField, Min(0)] private float baseThrust = 1f;
        [SerializeField, MinMax(-180, 180)] private Vector2 yawLimits = new(-45, 45);
        [SerializeField, Min(0)] private float turnSpeed = 20f;

        public float ThrusterForce => thrusterForce;
        public float BaseThrust => baseThrust;
        public Vector2 YawLimits => yawLimits;
        public float TurnSpeed => turnSpeed;
    }
}
