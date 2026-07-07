using FK.Common;
using FK.Spaceship.Gameplay.CameraEffects;
using UnityEngine;

namespace FK.Spaceship.Gameplay.Data
{
    /// <seealso cref="ShipFeelController"/>
    [CreateAssetMenu(menuName = "Spaceship/Ship Feel")]
    public class ShipFeelAsset : ScriptableObject, ICameraZoomConfigProvider, ICameraFollowConfigProvider
    {
        [Header("Config - Follow")]
        [SerializeField] private Vector3 cameraFollowOffset;
        [SerializeField] private Vector3 cameraFollowDecay;
        [SerializeField] private Vector3 cameraFollowAngleOffset;
        [SerializeField] private float cameraFollowAngleSpeed;

        [Header("Config - Zoom")]
        [SerializeField] private CameraZoomEffect.ZoomConfig cameraZoomConfig;

        [Header("Config - Max Thrust")]
        [SerializeField] private CameraShakeProfileAsset cameraShakeAtMaxThrust;
        [SerializeField, Min(0)] private float rumbleDelay;
        [SerializeField] private RumbleProfile rumbleAtMaxThrust;

        [Header("Config - Collision")]
        [SerializeField] private CameraShakeProfileAsset collisionShakeProfile;

        public ref readonly Vector3 CameraFollowOffset => ref cameraFollowOffset;
        public ref readonly Vector3 CameraFollowDecay => ref cameraFollowDecay;
        public ref readonly Vector3 CameraFollowAngleOffset => ref cameraFollowAngleOffset;
        public ref readonly float CameraFollowAngleSpeed => ref cameraFollowAngleSpeed;

        public ref readonly CameraZoomEffect.ZoomConfig CameraZoomConfig => ref cameraZoomConfig;

        public ref readonly CameraShakeProfileAsset CameraShakeAtMaxThrust => ref cameraShakeAtMaxThrust;
        public ref readonly float RumbleDelay => ref rumbleDelay;
        public ref readonly RumbleProfile RumbleAtMaxThrust => ref rumbleAtMaxThrust;

        public ref readonly CameraShakeProfileAsset CollisionShakeProfile => ref collisionShakeProfile;
    }
}
