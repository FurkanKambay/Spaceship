using FK.Common;
using UnityEngine;

namespace FK.Spaceship.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipController ship;

        [Header("Config")]
        [SerializeField] private Duo levelBounds;

        private void Awake()
        {
            ship.SetLevel(levelBounds);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(new Vector3(levelBounds.Left, -50, 0), Vector3.up * 100);
            Gizmos.DrawRay(new Vector3(levelBounds.Right, -50, 0), Vector3.up * 100);
        }
    }
}
