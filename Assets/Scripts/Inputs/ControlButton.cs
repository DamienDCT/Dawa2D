using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image displayedIcon; // Image where icon keybind is displayed

    [Space(1)]
    [Header("Sprite References")]
    [SerializeField] private Sprite keyboardIcon; // Keyboard icon of that button
    [SerializeField] private Sprite gamepadIcon; // Gamepad icon of that button;

    [Header("Optional Settings")]
    [SerializeField] private string actionName; // e.g. "Jump" or "Attack"
    [SerializeField] private InputIconDatabase inputIconDatabase; // optional custom icons
    [SerializeField] private InputActionReference inputAction;

/*    private void Awake()
    {
        if(inputAction != null)
        {
            Debug.Log(inputAction.)
        }
    }*/

    private void OnEnable()
    {
        if (CurrentActiveDeviceManager.Instance != null)
        {
            CurrentActiveDeviceManager.Instance.OnActiveDeviceChanged += OnDeviceChanged;
            UpdateIcon(CurrentActiveDeviceManager.Instance.CurrentDevice);
        }
        else
        {
            Debug.LogWarning("No CurrentActiveDeviceManager instance found.");
        }
    }

    private void OnDisable()
    {
        if (CurrentActiveDeviceManager.Instance != null)
            CurrentActiveDeviceManager.Instance.OnActiveDeviceChanged -= OnDeviceChanged;
    }

    private void OnDeviceChanged(object sender, CurrentActiveDeviceManager.ActiveDevice device)
    {
        if(inputAction == null)
        {
            displayedIcon.sprite = device == CurrentActiveDeviceManager.ActiveDevice.Gamepad ? gamepadIcon : keyboardIcon;
        }
        else
        {
            UpdateIcon(device);
        }
    }

    public void RefreshUI()
    {
        if(inputAction == null)
        {
            
            switch (CurrentActiveDeviceManager.Instance.CurrentDevice)
            {
                case CurrentActiveDeviceManager.ActiveDevice.Keyboard:
                    displayedIcon.sprite = keyboardIcon;
                    break;
                case CurrentActiveDeviceManager.ActiveDevice.Gamepad:
                    displayedIcon.sprite = gamepadIcon;
                    break;
                default:
                    displayedIcon.sprite = keyboardIcon;
                    break;
            }
        } else
        {
            UpdateIcon(CurrentActiveDeviceManager.Instance.CurrentDevice);
        }
    }

    private void UpdateIcon(CurrentActiveDeviceManager.ActiveDevice device)
    {
        var action = inputAction?.action;
        if (action == null)
        {
            switch (CurrentActiveDeviceManager.Instance.CurrentDevice)
            {
                case CurrentActiveDeviceManager.ActiveDevice.Keyboard:
                    displayedIcon.sprite = keyboardIcon;
                    break;
                case CurrentActiveDeviceManager.ActiveDevice.Gamepad:
                    displayedIcon.sprite = gamepadIcon;
                    break;
                default:
                    displayedIcon.sprite = keyboardIcon;
                    break;
            }
            return;
        }

        // Détermine le groupe ciblé (ex: "Gamepad" ou "Keyboard")
        string targetGroup = device == CurrentActiveDeviceManager.ActiveDevice.Gamepad ? "Gamepad" : "Keyboard";

        // Parcours ReadOnlyArray<InputBinding>
        InputBinding? found = null;
        foreach (var b in action.bindings)
        {
            if (b.isPartOfComposite) continue;          // ignore composite parts (Up/Down/Left/Right)
            if (string.IsNullOrEmpty(b.effectivePath)) continue; // ignore bindings vides
            if (string.IsNullOrEmpty(b.groups))
            {
                // si pas de groups, on peut l'accepter en fallback
                found = b;
                break;
            }

            // groups peut contenir plusieurs groupes (ex: "Keyboard&Mouse"), donc on cherche la substring
            if (b.groups.IndexOf(targetGroup, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                found = b;
                break;
            }
        }

        // Si rien trouvé pour le groupe ciblé, on tente un fallback (premier binding utile)
        if (found == null)
        {
            foreach (var b in action.bindings)
            {
                if (b.isPartOfComposite) continue;
                if (string.IsNullOrEmpty(b.effectivePath)) continue;
                found = b;
                break;
            }
        }

        if (found == null)
            return;

        // Récupère le path complet (ex: "Keyboard/e" ou "Gamepad/buttonSouth")
        string controlPath = found.Value.effectivePath;

        // Récupère l'icône correspondante via ta DB
        Sprite icon = inputIconDatabase?.GetIconForControlPath(controlPath);

        // Applique le sprite (fallback)
        if (icon != null)
            displayedIcon.sprite = icon;
    }
}
