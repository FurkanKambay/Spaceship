using UnityEngine;

namespace FK.Spaceship.Gameplay.Data
{
    [CreateAssetMenu(menuName = "Spaceship/Camera Shake Profile")]
    public class CameraShakeProfileAsset : ScriptableObject
    {
        [Header("Config - Shake")]
        [SerializeField, Min(0)] private float frequency = 25f;
        [SerializeField] private Vector3 maxShakeOffset = Vector3.one;
        [SerializeField] private Vector3 maxShakeAngles;

        [Header("Config - Trauma")]
        [SerializeField, Range(0, 20)] private float traumaExponent = 2f;
        [SerializeField, Range(0, 10)] private float recoverySpeed = 1.5f;

        public float Frequency => frequency;
        public ref readonly Vector3 MaxShakeOffset => ref maxShakeOffset;
        public ref readonly Vector3 MaxShakeAngles => ref maxShakeAngles;

        public float TraumaExponent => traumaExponent;
        public float RecoverySpeed => recoverySpeed;
    }
}
