using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CurrentActiveDeviceManager : MonoBehaviour
{
    public static CurrentActiveDeviceManager Instance;

    public ActiveDevice CurrentDevice;

    [SerializeField] private PlayerInput playerInput;

    public event EventHandler<ActiveDevice> OnActiveDeviceChanged;

    public enum ActiveDevice
    {
        Keyboard,
        Gamepad,
    }

    private void Awake()
    {
        Instance = this;

        // Initialize the device right away
        UpdateActiveDevice(force: true);
    }

    private void Update()
    {
        UpdateActiveDevice();
    }

    private void UpdateActiveDevice(bool force = false)
    {
        if (playerInput == null) return;

        ActiveDevice newDevice = playerInput.currentControlScheme switch
        {
            "Gamepad" => ActiveDevice.Gamepad,
            _ => ActiveDevice.Keyboard, // default fallback
        };

        // Fire event only if device changed or we force first update
        if (force || newDevice != CurrentDevice)
        {
            CurrentDevice = newDevice;
            OnActiveDeviceChanged?.Invoke(this, CurrentDevice);
        }
    }
}