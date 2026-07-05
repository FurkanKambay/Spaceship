using FK.Common;
using UnityEngine;

namespace FK.Spaceship.Gameplay.Visual
{
    public class ShipVisualizer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipController ship;
        [SerializeField] private SpriteRenderer leftThruster;
        [SerializeField] private SpriteRenderer rightThruster;

        [Header("Config")]
        [SerializeField] private Sprite slowFlame;
        [SerializeField] private Sprite fastFlame;

        private void Update()
        {
            Duo input = ship.MoveInputs;
            leftThruster.color = new Color(1, 1, 1, input.Left);
            rightThruster.color = new Color(1, 1, 1, input.Right);

            bool goingMaxSpeed = ship.TotalThrust >= ship.MaxTotalThrust;
            SetBothSprites(goingMaxSpeed ? fastFlame : slowFlame);
        }

        private void SetBothSprites(Sprite sprite)
        {
            leftThruster.sprite = sprite;
            rightThruster.sprite = sprite;
        }
    }
}
