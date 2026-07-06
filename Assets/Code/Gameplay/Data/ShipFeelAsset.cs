using FK.Common;
using FK.Spaceship.Gameplay.CameraEffects;
using UnityEngine;

namespace FK.Spaceship.Gameplay.Data
{
    [CreateAssetMenu(menuName = "Spaceship/Ship Feel")]
    public class ShipFeelAsset : ScriptableObject, ICameraZoomConfigProvider, ICameraFollowConfigProvider
    {
        [Header("Config - Follow")]
        [SerializeField] private Vector3 cameraFollowOffset;
        [SerializeField] private Vector2 cameraFollowDecay;

        [Header("Config - Zoom")]
        [SerializeField] private CameraZoomEffect.ZoomConfig cameraZoomConfig;

        [Header("Config - Max Thrust")]
        [SerializeField] private CameraShakeProfileAsset cameraShakeAtMaxThrust;
        [SerializeField, Min(0)] private float rumbleDelay;
        [SerializeField] private RumbleProfile rumbleAtMaxThrust;

        [Header("Config - Collision")]
        [SerializeField] private CameraShakeProfileAsset collisionShakeProfile;

        public Vector3 CameraFollowOffset => cameraFollowOffset;
        public Vector2 CameraFollowDecay => cameraFollowDecay;

        public CameraZoomEffect.ZoomConfig CameraZoomConfig => cameraZoomConfig;

        public CameraShakeProfileAsset CameraShakeAtMaxThrust => cameraShakeAtMaxThrust;
        public float RumbleDelay => rumbleDelay;
        public RumbleProfile RumbleAtMaxThrust => rumbleAtMaxThrust;

        public CameraShakeProfileAsset CollisionShakeProfile => collisionShakeProfile;
    }
}
