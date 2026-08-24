using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggler : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryUI; // référence au GameObject InventoryUI
    [SerializeField] private PauseMenuUI pauseMenuUI;
    private bool isOpen = false;

    private void Update()
    {
        // Ici tu peux utiliser le nouveau Input System, ou Input.GetKeyDown()
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (pauseMenuUI.IsGamePaused)
            return;
        isOpen = !isOpen;
        inventoryUI.gameObject.SetActive(isOpen);
    }
}
