using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    public static InputController Instance;

    private PlayerInput playerInput;

    // Valeurs exposées
    public Vector2 MovementVector { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }

    public event EventHandler OnPausePressed;

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
            playerInput.actions["Jump"].performed += ctx => { JumpPressed = true; JumpHeld = true; };
            playerInput.actions["Jump"].canceled += ctx => JumpHeld = false;

            // Pause
            playerInput.actions["Pause"].performed += ctx => OnPausePressed?.Invoke(this, EventArgs.Empty);
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
