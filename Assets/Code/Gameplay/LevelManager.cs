using FK.Common;
using UnityEngine;
using UnityEngine.Assertions;

namespace FK.Spaceship.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private CameraFollow cameraFollow;
        [SerializeField] private ShipController ship;

        [Header("Config")]
        [SerializeField] private ShipController shipPrefab;
        [SerializeField] private Duo levelBounds;

        private void Awake()
        {
            Assert.IsNotNull(cameraFollow);

            if (!ship)
                ship = Instantiate(shipPrefab);

            InjectDependencies();
        }

        private void InjectDependencies()
        {
            if (!ship)
                throw new UnassignedReferenceException(nameof(ship));

            ship.SetLevel(levelBounds);
            cameraFollow.InjectTarget(ship.transform);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(new Vector3(levelBounds.Left, -50, 0), Vector3.up * 100);
            Gizmos.DrawRay(new Vector3(levelBounds.Right, -50, 0), Vector3.up * 100);
        }

        private void OnValidate()
        {
            levelBounds.Right = Mathf.Max(levelBounds.Right, levelBounds.Left + 5f);
        }
#endif
    }
}
