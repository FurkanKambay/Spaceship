using UnityEngine;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay.Data
{
    [CreateAssetMenu(menuName = "Spaceship/Ship Stats")]
    public class ShipStats : ScriptableObject
    {
        [Header("Thrusters")]
        [SerializeField, Min(0)] private float baseThrust = 1f;
        [SerializeField, Min(0)] private float thrusterForce = 20f;
        [SerializeField, Min(0)] private float acceleration = 5f;
        [SerializeField, Min(0)] private float maxTotalThrust = 20f;

        [Header("Turning")]
        [SerializeField, Min(0)] private float turnSpeed = 20f;
        [SerializeField, MinMax(-180, 180)] private Vector2 yawLimits = new(-45, 45);

        [Header("Dash")]
        [SerializeField, Min(0)] private float dashForce = 20f;
        [SerializeField, Min(0)] private float dashDecay = 10f;

        public float BaseThrust => baseThrust;
        public float ThrusterForce => thrusterForce;
        public float Acceleration => acceleration;
        public float MaxTotalThrust => maxTotalThrust;

        public Vector2 YawLimits => yawLimits;
        public float TurnSpeed => turnSpeed;

        public float DashForce => dashForce;
        public float DashDecay => dashDecay;
    }
}
