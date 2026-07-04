using UnityEngine;

namespace FK.Spaceship.Input
{
    [DefaultExecutionOrder(-10)]
    public sealed class InputService : MonoBehaviour
    {
        public Controls Controls { get; private set; }

        private void Awake() => Controls = new Controls();
        private void Start() => ActivatePlayerControls();

        private void OnDestroy() => Controls.Dispose();

        private void ActivatePlayerControls()
        {
            Controls.Player.Enable();
            Controls.UI.Disable();
        }

        private void ActivateUIControls()
        {
            Controls.Player.Disable();
            Controls.UI.Enable();
        }
    }
}
