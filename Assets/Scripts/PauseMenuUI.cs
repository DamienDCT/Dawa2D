using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private RectTransform parentControlButtons;


    public bool IsGamePaused { get; private set; }

    private void Awake()
    {
        IsGamePaused = false;
    }

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

    private void TogglePauseMenu(object sender, EventArgs args)
    {
        bool isActive = !menuUI.activeSelf;
        menuUI.SetActive(isActive);
        IsGamePaused = isActive;

        Time.timeScale = isActive ? 0f : 1f;
        if(!isActive)
        {
            RefreshDisplayedKeybinds();
        }
    }
}
