using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BasedUITK : MonoBehaviour
{

    protected VisualElement menuElement;
    protected bool IsMenuOpened = false;
    public bool hasToOpen;

    private void Update()
    {
        if (hasToOpen && Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    protected void ToggleMenu()
    {
        IsMenuOpened = !IsMenuOpened;
        if (IsMenuOpened)
        {
            OnMenuOpened();
            menuElement?.RemoveFromClassList("hidden");
        }
        else
        {
            OnMenuClosed();
            menuElement?.AddToClassList("hidden");
        }

    }

    protected virtual void OnMenuOpened() { }
    protected virtual void OnMenuClosed() { }
}
