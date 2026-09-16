using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BasedUITK : MonoBehaviour
{

    protected VisualElement menuElement;
    protected bool IsMenuOpened = false;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
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
