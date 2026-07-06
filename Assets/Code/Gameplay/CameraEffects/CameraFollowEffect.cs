using System;
using FK.Common.Extensions;
using UnityEngine;

namespace FK.Spaceship.Gameplay.CameraEffects
{
    [Serializable]
    public struct CameraFollowEffect : ICameraEffect
    {
        [SerializeField] private Transform target;

        private Transform cameraPivot;
        private Vector3 offset;
        private Vector2 followDecay;
        private Vector3 destination;

        public CameraFollowEffect(Transform cameraPivot, Transform target, Vector3 constantOffset, Vector2 followDecay)
        {
            this.cameraPivot = cameraPivot ? cameraPivot : throw new ArgumentNullException(nameof(cameraPivot));
            this.target = target ? target : throw new ArgumentNullException(nameof(target));
            this.offset = constantOffset;
            this.followDecay = followDecay;
            this.destination = target.position + constantOffset;
        }

        public bool Tick()
        {
            if (!target)
                return true;

            destination = target.position + offset;
            Vector3 current = cameraPivot.transform.position;

            cameraPivot.transform.position = new Vector3(
                x: current.x.ExpDecay(destination.x, followDecay.x, Time.deltaTime),
                y: current.y.ExpDecay(destination.y, followDecay.y, Time.deltaTime),
                z: destination.z
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
