using System;
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
        [SerializeField] private Sprite[] flameSprites;

        private void Update()
        {
            Duo input = ship.MoveInputs;
            leftThruster.color = new Color(1, 1, 1, input.Left);
            rightThruster.color = new Color(1, 1, 1, input.Right);

            float thrustRatio = ship.TotalThrust / ship.MaxTotalThrust;
            int spriteIndex = (int)Mathf.Lerp(0, flameSprites.Length - 1, thrustRatio);
            SetBothSprites(spriteIndex);
        }

        private void SetBothSprites(int spriteIndex)
        {
            if (spriteIndex < 0 || spriteIndex >= flameSprites.Length)
                throw new ArgumentOutOfRangeException(nameof(spriteIndex));

            Sprite sprite = flameSprites[spriteIndex];
            leftThruster.sprite = sprite;
            rightThruster.sprite = sprite;
        }
    }
}
