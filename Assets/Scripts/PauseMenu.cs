using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;

    private void Start()
    {
        InputController.Instance.OnPausePressed += TogglePauseMenu;
    }

/*    private void OnDisable()
    {
        InputController.Instance.OnPausePressed -= TogglePauseMenu;
    }*/

    private void TogglePauseMenu(object sender, EventArgs e)
    {
        bool isActive = !menuUI.activeSelf;
        menuUI.SetActive(isActive);

        Time.timeScale = isActive ? 0f : 1f;
    }
}
