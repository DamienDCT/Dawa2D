using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPhaseSelector : MonoBehaviour
{
    [SerializeField] private PlayerPhaseManager phaseManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject phaseMenuUI; // Canvas worldspace/screenspace
    [SerializeField] private Image[] phaseIcons;     // icônes des phases
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    private int currentIndex = 0;
    private int previousPhaseIndex = 0;
    private bool isHolding = false;

    private void Start()
    {
        phaseMenuUI.SetActive(false);
        UpdateUI();
        previousPhaseIndex = currentIndex;
    }

    private void Update()
    {
        if (!isHolding) return;

        var input = InputController.Instance.GetPlayerInput();

        // Navigation gauche/droite avec LB / RB
        if (input.actions["PreviousPhase"].WasPressedThisFrame())
        {
            PreviousPhase();
        }
        if (input.actions["NextPhase"].WasPressedThisFrame())
        {
            NextPhase();
        }

    }

    private void OnEnable()
    {
        InputController.Instance.OnPhaseSelectorChange += HandlePhaseMenu;
    }

    private void OnDisable()
    {
        InputController.Instance.OnPhaseSelectorChange -= HandlePhaseMenu;
    }

    private void HandlePhaseMenu(object sender, bool hasToOpen)
    {
        if (hasToOpen)
        {
            if (!playerMovement.IsGrounded()) return; 
            isHolding = true;
            phaseMenuUI.SetActive(true);
            currentIndex = (int)phaseManager.GetCurrentPhase();
        }
        else
        {
            if (!isHolding) return;
            isHolding = false;
            phaseMenuUI.SetActive(false);
            if(previousPhaseIndex != currentIndex)
            {
                phaseManager.ChangePhase((PlayerPhase)currentIndex);
                Debug.Log("we change phase");
                previousPhaseIndex = currentIndex;
            }
        }
    }

    private void NextPhase()
    {
        currentIndex = (currentIndex + 1) % phaseIcons.Length;
        UpdateUI();
    }

    private void PreviousPhase()
    {
        currentIndex = (currentIndex - 1 + phaseIcons.Length) % phaseIcons.Length;
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < phaseIcons.Length; i++)
        {
            phaseIcons[i].color = (i == currentIndex) ? selectedColor : normalColor;
        }
    }
}
