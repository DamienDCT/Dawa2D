using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    public static InputController Instance;

    private PlayerInput playerInput; // Reference to the PlayerInput component

    [SerializeField] private PlayerMovements playerMovements; // Reference to the script who does player movements

    // Exposed values
    public Vector2 MovementVector { get; private set; } // Movement vector given by the InputController script
    public bool JumpPressed { get; private set; } // Boolean if the jump is pressed
  //  public bool JumpHeld { get; private set; } 

    public event EventHandler OnPausePressed; // Event fired when the pause button is pressed
    public event EventHandler<bool> OnPhaseSelectorChange; // Event fired when the phase selector is hold or released
    public event EventHandler<PlayerPowerUpArgs> OnPowerUpButtonPressed; // Event fired when the powerup button is pressed
    public event EventHandler OnAttackButtonPressed; // Event fired when the attack button is pressed
    public event EventHandler OnInteractButtonPressed; // Event fired when the interact button is pressed


    // UI ONLY
    public event EventHandler OnBackPressed; // Event fired when cancel button is pressed

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            // PLAYER ACTION MAP
            // Movement
            playerInput.actions["Move"].performed += ctx => MovementVector = ctx.ReadValue<Vector2>();
            playerInput.actions["Move"].canceled += ctx => MovementVector = Vector2.zero;

            // Jump
            playerInput.actions["Jump"].performed += ctx => { playerMovements.OnJumpInput(); };
            playerInput.actions["Jump"].canceled += ctx => { playerMovements.OnJumpUpInput(); };

            // Pause
            playerInput.actions["Pause"].performed += ctx => OnPausePressed?.Invoke(this, EventArgs.Empty);

            // Phases
            playerInput.actions["OpenPhaseMenu"].started += OpenPhaseMenu;
            playerInput.actions["OpenPhaseMenu"].canceled += OpenPhaseMenu;

            // Spells
            playerInput.actions["PowerUp"].performed += PowerUpEsquive;

            // Attack Input
            playerInput.actions["Attack"].performed += OnAttackPressed;

            // Interact Input
            playerInput.actions["Interact"].performed += OnInteractPressed;


            // UI ACTION MAP
            playerInput.actions["Cancel"].performed += ctx => OnBackPressed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            OnInteractButtonPressed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnAttackPressed(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            OnAttackButtonPressed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PowerUpEsquive(InputAction.CallbackContext context)
    {
        if (MovementVector.y == 0)
        {
            OnPowerUpButtonPressed?.Invoke(this, new PlayerPowerUpArgs { powerUpType = PowerUpType.ESQUIVE });
        } else if(MovementVector.y == 1)
        {
            OnPowerUpButtonPressed?.Invoke(this, new PlayerPowerUpArgs { powerUpType = PowerUpType.ASCENSION });
        } else if(MovementVector.y == -1)
        {
            OnPowerUpButtonPressed?.Invoke(this, new PlayerPowerUpArgs { powerUpType = PowerUpType.MASSUE });
        }
    }

    private void OpenPhaseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnPhaseSelectorChange?.Invoke(this, true);
        } else if(context.canceled)
        {
            OnPhaseSelectorChange?.Invoke(this, false);
        }
    }

    public void SwapInputActionMap(string nextActionMap)
    {
        playerInput.SwitchCurrentActionMap(nextActionMap);
    }

    private void LateUpdate()
    {
        // Reset JumpPressed à false chaque frame après avoir été lu
        JumpPressed = false;
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }
}
