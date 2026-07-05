using FK.Spaceship.Gameplay.Data;
using UnityEngine;

namespace FK.Spaceship.Gameplay
{
    // https://github.com/IronWarrior/UnityCameraShake
    public class CameraShake : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private CameraShakeProfileAsset profile;
        [SerializeField, Range(0, 1)] private float trauma;

        private float seed;

        private void OnEnable() => seed = Random.value;
        private void OnDisable() => KillShake();

        private void Update()
        {
            if (!profile)
                return;

            float shake = Mathf.Pow(trauma, profile.TraumaExponent);
            float perlinY = Time.time * profile.Frequency;

            if (profile.MaxShakeOffset != Vector3.zero)
            {
                var noise = new Vector3(
                    x: (Mathf.PerlinNoise(seed, perlinY) * 2) - 1,
                    y: (Mathf.PerlinNoise(seed + 1, perlinY) * 2) - 1,
                    z: (Mathf.PerlinNoise(seed + 2, perlinY) * 2) - 1
                );

                transform.localPosition = shake * Vector3.Scale(profile.MaxShakeOffset, noise);
            }

            if (profile.MaxShakeAngles != Vector3.zero)
            {
                var noise = new Vector3(
                    x: (Mathf.PerlinNoise(seed + 3, perlinY) * 2) - 1,
                    y: (Mathf.PerlinNoise(seed + 4, perlinY) * 2) - 1,
                    z: (Mathf.PerlinNoise(seed + 5, perlinY) * 2) - 1
                );

                transform.localRotation = Quaternion.Euler(shake * Vector3.Scale(profile.MaxShakeAngles, noise));
            }

            if (trauma > 0 && profile.RecoverySpeed > 0)
                trauma = Mathf.Clamp01(trauma - (profile.RecoverySpeed * Time.deltaTime));
        }

        public void SetProfile(CameraShakeProfileAsset profileAsset) =>
            this.profile = profileAsset;

        public void AddTrauma(float value) => trauma = Mathf.Clamp01(trauma + value);
        public void ClearTrauma() => trauma = 0;

        public void KillShake()
        {
            trauma = 0;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}
