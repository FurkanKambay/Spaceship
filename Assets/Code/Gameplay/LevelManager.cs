using FK.Common;
using FK.Spaceship.Input;
using UnityEngine;
using UnityEngine.Assertions;
using Vertx.Attributes;

namespace FK.Spaceship.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        [Header("References - Scene")]
        [SerializeField] private InputService inputService;

        [Header("Config")]
        [SerializeField] private ShipController shipPrefab;
        [SerializeField] private Duo levelBounds;

        [Header("State")]
        [SerializeField, ReadOnlyField] private ShipController ship;

        private void Awake()
        {
            Assert.IsNotNull(inputService);

            if (!ship)
            {
                ship = Instantiate(shipPrefab);
                ship.name = "🚀 Player Ship";
            }

            InjectDependencies();
        }

        private void InjectDependencies()
        {
            if (!ship)
                throw new UnassignedReferenceException(nameof(ship));

            inputService.Controls.Player.SetCallbacks(ship);

            ship.SetLevel(levelBounds);
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
