using UnityEngine;
using UnityEngine.UIElements;

public class BasedUITK : MonoBehaviour
{
    protected VisualElement menuElement;
    protected bool IsMenuOpened = false;

    public bool IsOpened => IsMenuOpened;

    public void Open()
    {
        if (IsMenuOpened) return;
        IsMenuOpened = true;
        OnMenuOpened();
        menuElement?.RemoveFromClassList("hidden");
    }

    public void Close()
    {
        if (!IsMenuOpened) return;
        IsMenuOpened = false;
        OnMenuClosed();

        if (menuElement == null) return;

        // Retire le focus AVANT de cacher, sinon on cache un élément
        // qui est encore en train d'être traité par le dispatcher d'events
        var focused = menuElement.focusController?.focusedElement as VisualElement;
        if (focused != null && menuElement.Contains(focused))
            focused.Blur();

        // On diffère le masquage réel au prochain repaint,
        // pour sortir du dispatch d'event courant (NavigationSubmitEvent, etc.)
        menuElement.schedule.Execute(() =>
        {
            menuElement.AddToClassList("hidden");
        }).ExecuteLater(0);
    }

    public void ToggleMenu()
    {
        if (IsMenuOpened) Close();
        else Open();
    }

    protected virtual void OnMenuOpened() { }
    protected virtual void OnMenuClosed() { }
}