using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private RectTransform parentControlButtons;

    private void OnEnable()
    {
        InputController.Instance.OnPausePressed += TogglePauseMenu;
    }

    private void OnDisable()
    {
        InputController.Instance.OnPausePressed -= TogglePauseMenu;
    }

    private void RefreshDisplayedKeybinds()
    {
        ControlButton[] buttons = parentControlButtons.GetComponentsInChildren<ControlButton>();

        foreach (ControlButton button in buttons)
        {
            if(button != null)
            {
                button.RefreshUI();
            }
        }
    }

    private void TogglePauseMenu(object sender, EventArgs e)
    {
        bool isActive = !menuUI.activeSelf;
        menuUI.SetActive(isActive);

        Time.timeScale = isActive ? 0f : 1f;
        if(!isActive)
        {
            RefreshDisplayedKeybinds();
        }
    }
}
