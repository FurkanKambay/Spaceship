using FK.Spaceship.Input;
using UnityEngine;
using UnityEngine.Assertions;

namespace FK.Spaceship.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        [Header("References - Scene")]
        [SerializeField] private InputService inputService;

        [Header("Config")]
        [SerializeField] private ShipController shipPrefab;

        [Header("State")]
        [SerializeField] private ShipController ship;

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
        }
    }
}
