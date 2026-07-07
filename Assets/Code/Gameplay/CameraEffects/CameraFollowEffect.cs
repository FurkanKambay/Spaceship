using System;
using FK.Common.Extensions;
using UnityEngine;

namespace FK.Spaceship.Gameplay.CameraEffects
{
    /// <seealso cref="CameraFollowEffect"/>
    public interface ICameraFollowConfigProvider
    {
        ref readonly Vector3 CameraFollowOffset { get; }
        ref readonly Vector3 CameraFollowDecay { get; }
        ref readonly Vector3 CameraFollowAngleOffset { get; }
        ref readonly float CameraFollowAngleSpeed { get; }
    }

    /// <remarks>Modifies the camera pivot's Position, Rotation.</remarks>
    /// <seealso cref="ICameraFollowConfigProvider"/>
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

            this.destination = target.TransformPoint(config.CameraFollowOffset);
        }

        public bool Tick()
        {
            if (!target)
                return true;

            float deltaTime = Time.deltaTime;

            // Position
            Vector3 pivotPosition = cameraPivot.position;
            destination = target.TransformPoint(config.CameraFollowOffset);
            Vector3 newPosition = pivotPosition.ExpDecay(destination, config.CameraFollowDecay, deltaTime);

            // Rotation
            Quaternion pivotRotation = cameraPivot.rotation;
            Vector3 lookVector = target.position - newPosition;
            Quaternion lookRotation = Quaternion.LookRotation(lookVector);
            Vector3 targetAngles = lookRotation.eulerAngles + config.CameraFollowAngleOffset;
            Quaternion targetRotation = Quaternion.Euler(targetAngles);
            Quaternion newRotation = Quaternion.RotateTowards(in pivotRotation, in targetRotation, config.CameraFollowAngleSpeed);

            // cameraPivot.position = newPosition;
            cameraPivot.SetPositionAndRotation(newPosition, newRotation);

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
