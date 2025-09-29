using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    public static InputController Instance;

    private PlayerInput playerInput;

    [SerializeField] private PlayerMovements playerMovements;

    // Valeurs exposées
    public Vector2 MovementVector { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }

    public event EventHandler OnPausePressed;
    public event EventHandler<bool> OnPhaseSelectorChange;
    public event EventHandler<PlayerSpellArgs> OnBasicSpellStarted;
    public event EventHandler OnJumpPressed;
    public event EventHandler OnJumpReleased;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
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
            playerInput.actions["BasicSpell"].performed += BasicSpellCharge;
        }
    }

    private void BasicSpellCharge(InputAction.CallbackContext context)
    {
        if (MovementVector.y == 0)
        {
            OnBasicSpellStarted?.Invoke(this, new PlayerSpellArgs { spellType = SpellType.ESQUIVE });
        } else if(MovementVector.y == 1)
        {
            OnBasicSpellStarted?.Invoke(this, new PlayerSpellArgs { spellType = SpellType.ASCENSION });
        } else if(MovementVector.y == -1)
        {
            OnBasicSpellStarted?.Invoke(this, new PlayerSpellArgs { spellType = SpellType.MASSUE });
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
