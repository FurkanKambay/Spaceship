using System;
using FK.Spaceship.Gameplay.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FK.Spaceship.Gameplay.CameraEffects
{
    // https://github.com/IronWarrior/UnityCameraShake
    [Serializable]
    public class CameraShakeEffect : ICameraEffect
    {
        [SerializeField] private CameraShakeProfileAsset profile;
        [SerializeField, Range(0, 1)] private float trauma;

        private Camera camera;
        private float seed;

        public CameraShakeEffect(Camera camera, CameraShakeProfileAsset profile)
        {
            this.camera = camera ? camera : throw new ArgumentNullException(nameof(camera));
            this.profile = profile ? profile : throw new ArgumentNullException(nameof(profile));
            this.seed = Random.value;
            this.trauma = 0;
        }

        public void AddTrauma(float value) => trauma = Mathf.Clamp01(trauma + value);

        public bool Tick()
        {
            float shake = Mathf.Pow(trauma, profile.TraumaExponent);
            float perlinY = Time.time * profile.Frequency;

            if (profile.MaxShakeOffset != Vector3.zero)
            {
                var noise = new Vector3(
                    x: (Mathf.PerlinNoise(seed, perlinY) * 2) - 1,
                    y: (Mathf.PerlinNoise(seed + 1, perlinY) * 2) - 1,
                    z: (Mathf.PerlinNoise(seed + 2, perlinY) * 2) - 1
                );

                camera.transform.localPosition = shake * Vector3.Scale(profile.MaxShakeOffset, noise);
            }

            if (profile.MaxShakeAngles != Vector3.zero)
            {
                var noise = new Vector3(
                    x: (Mathf.PerlinNoise(seed + 3, perlinY) * 2) - 1,
                    y: (Mathf.PerlinNoise(seed + 4, perlinY) * 2) - 1,
                    z: (Mathf.PerlinNoise(seed + 5, perlinY) * 2) - 1
                );

                camera.transform.localRotation = Quaternion.Euler(shake * Vector3.Scale(profile.MaxShakeAngles, noise));
            }

            if (trauma > 0 && profile.RecoverySpeed > 0)
                trauma = Mathf.Clamp01(trauma - (profile.RecoverySpeed * Time.deltaTime));

            return trauma == 0;
        }

        public void Stop()
        {
            trauma = 0;
        }

        public void Cleanup()
        {
            Stop();
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;
        }
    }
}
