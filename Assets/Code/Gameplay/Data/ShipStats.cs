using UnityEngine;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay.Data
{
    [CreateAssetMenu(menuName = "Spaceship/Ship Stats")]
    public class ShipStats : ScriptableObject
    {
        [Header("Thrusters")]
        [SerializeField, Min(0)] private float thrustSpeed = 20f;
        [SerializeField, Min(0)] private float thrustAcceleration = 5f;
        [SerializeField, Min(0)] private float maxTotalThrust = 20f;

        [Header("Turning")]
        [SerializeField, Range(0, 720)] private float turnSpeed = 360f;
        [SerializeField, Range(0, 720)] private float turnDecay = 10f;
        [SerializeField, MinMax(-180, 180)] private Vector2 rollLimits = new(-45, 45);

        [Header("Dash")]
        [SerializeField, Min(0)] private float dashForce = 20f;
        [SerializeField, Min(0)] private float dashDecay = 10f;

        public float ThrustSpeed => thrustSpeed;
        public float ThrustAcceleration => thrustAcceleration;
        public float MaxTotalThrust => maxTotalThrust;

        public float TurnSpeed => turnSpeed;
        public float TurnDecay => turnDecay;
        public Vector2 RollLimits => rollLimits;

        public float DashForce => dashForce;
        public float DashDecay => dashDecay;
    }
}
