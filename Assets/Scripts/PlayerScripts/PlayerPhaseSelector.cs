using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPhaseSelector : MonoBehaviour
{
    [SerializeField] private PlayerPhaseManager phaseManager; // Reference to the phase manager
    [SerializeField] private PlayerMovements playerMovement; // Reference to the player movements script
    [SerializeField] private GameObject phaseMenuUI; // World space canvas
    [SerializeField] private Sprite[] phaseIcons;     // Phase icons

    [SerializeField] private Image phaseImage;

    [SerializeField] private ControlButton[] controlButton;

    public bool IsChangingPhase { get; private set; }

    private int currentIndex = 0; // Current index of the phase selected
    private int previousPhaseIndex = 0; // Previous phase index when released
    private bool isHolding = false; // Variable that stores if we hold the phase change button

    private void Start()
    {
        phaseMenuUI.SetActive(false);
        UpdateUI();
        previousPhaseIndex = currentIndex;
        IsChangingPhase = false;
    }

    private void Update()
    {
        if (!isHolding) return;

        // Boutons fixes : Q / D sur clavier
        if (Keyboard.current.aKey.wasPressedThisFrame)
            PreviousPhase();

        if (Keyboard.current.dKey.wasPressedThisFrame)
            NextPhase();

        // Boutons fixes : LB / RB sur manette
        if (Gamepad.current != null)
        {
            if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                PreviousPhase();

            if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                NextPhase();
        }

    }

    public void SetCanvasCorrectly(Vector3 currentScale)
    {
        phaseMenuUI.transform.localScale = currentScale;
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
            if (playerMovement.LastOnGroundTime < 0) return;
            if (playerMovement.RB.linearVelocity != Vector2.zero) return;
            isHolding = true;
            phaseMenuUI.SetActive(true);
            currentIndex = (int)phaseManager.GetCurrentPhase();
            IsChangingPhase = true;
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
            IsChangingPhase = false;
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
        phaseImage.sprite = phaseIcons[currentIndex];
    }
}
