using System;
using FK.Common.Extensions;
using UnityEngine;

namespace FK.Spaceship.Gameplay.CameraEffects
{
    public interface ICameraFollowConfigProvider
    {
        ref readonly Vector3 CameraFollowOffset { get; }
        ref readonly Vector3 CameraFollowDecay { get; }
    }

    [Serializable]
    public struct CameraFollowEffect : ICameraEffect
    {
        [SerializeField] private Transform target;

        private Transform cameraPivot;
        private ICameraFollowConfigProvider config;

        private Vector3 destination;

        public CameraFollowEffect(Transform cameraPivot, Transform target, ICameraFollowConfigProvider configProvider)
        {
            this.cameraPivot = cameraPivot ? cameraPivot : throw new ArgumentNullException(nameof(cameraPivot));
            this.target = target ? target : throw new ArgumentNullException(nameof(target));
            this.config = configProvider ?? throw new ArgumentNullException(nameof(configProvider));

            this.destination = target.position + config.CameraFollowOffset;
        }

        public bool Tick()
        {
            if (!target)
                return true;

            destination = target.position + config.CameraFollowOffset;
            Vector3 current = cameraPivot.transform.position;

            cameraPivot.transform.position = new Vector3(
                x: current.x.ExpDecay(destination.x, config.CameraFollowDecay.x, Time.deltaTime),
                y: current.y.ExpDecay(destination.y, config.CameraFollowDecay.y, Time.deltaTime),
                z: current.z.ExpDecay(destination.z, config.CameraFollowDecay.z, Time.deltaTime)
            );

            // sustain effect indefinitely
            return false;
        }

        public void Stop()
        {
        }

        public void Cleanup()
        {
            Stop();
        }
    }
}
