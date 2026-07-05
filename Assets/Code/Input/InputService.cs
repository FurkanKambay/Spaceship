using FK.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FK.Spaceship.Input
{
    [DefaultExecutionOrder(-10)]
    public sealed class InputService : MonoBehaviour
    {
        public Controls Controls { get; private set; }

        private void Awake() => Controls = new Controls();
        private void Start() => ActivatePlayerControls();

        private void OnDestroy() => Controls.Dispose();

        private void OnEnable() => InputSystem.onDeviceChange += InputSystem_DeviceChanged;
        private void OnDisable() => InputSystem.onDeviceChange -= InputSystem_DeviceChanged;

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

        private void InputSystem_DeviceChanged(InputDevice device, InputDeviceChange change)
        {
            if (device is Mouse or Keyboard) return;
            if (change is InputDeviceChange.Removed or InputDeviceChange.Added) return;

            Log.Info($"[Input] {change}: {device.displayName}");
        }
    }
}
