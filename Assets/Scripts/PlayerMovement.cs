using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 5f;
    private bool canPlayerMove = true;

    [Header("Ground Check")]
    [SerializeField] private Transform groundPoint;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance;

    [Header("Wall Jump")]
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;
    public float wallJumpForce = 10f;
    public Vector2 wallJumpDirection = new Vector2(1, 1.5f);
    public float jumpCooldown = 0.2f;
    public float wallSlideSpeed = -2f;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpLockTime = 0.2f; // temps où on lock le mouvement après un wall jump

    [Header("Double Jump")]
    [SerializeField] private int amountJumps;
    private const int MAX_JUMPS = 2;

    private Rigidbody2D rb;
    [SerializeField] private bool isGrounded;
    [SerializeField] private int lastWallSide = 0; // -1 = gauche, 1 = droite
    private float lastJumpTime;
    private bool isWallSliding;

    [Header("Sprite renderer + animations")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool facingRight = true; 

    private InputController input;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        input = InputController.Instance;
        wallJumpDirection.Normalize();
        isWallJumping = false;
        amountJumps = 0;

        // InputController
        InputController.Instance.OnPhaseSelectorChange += InputController_OnPhaseSelectorChange;
    }

    private void InputController_OnPhaseSelectorChange(object sender, bool isMenuOpen)
    {
        this.canPlayerMove = !isMenuOpen;
        // Reset velocity 
        rb.linearVelocity = Vector3.zero;
    }

    private void Update()
    {
        CheckGround();
        CheckWallSlide();

        if (input.JumpPressed)
        {
            TryJump();
            // Debug.Log("coucou je jump");
        }
    }

    private void FixedUpdate()
    {
        PerformMovement();
    }

    public bool IsGrounded()
    {
        return this.isGrounded;
    }

    private bool CanDoubleJump()
    {
        return amountJumps < (MAX_JUMPS - 1);
    }

    private void TryJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            amountJumps++;
        }
        else
        {
            // ⛔ Pas de double jump si on est en train de sortir d'un wall jump
            if (!isWallSliding && CanDoubleJump())
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                amountJumps++;
            }

            TryWallJump();
        }
    }

    private void TryWallJump()
    {
        bool wallLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
        bool wallRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);

        int currentWallSide = 0;
        if (wallLeft) currentWallSide = -1;
        else if (wallRight) currentWallSide = 1;

        // ✅ Condition : être collé à un mur + cooldown + ne pas sauter deux fois du même mur
        if (currentWallSide != 0
            && currentWallSide != lastWallSide  // ⬅ empêche le spam sur le même mur
            && Time.time > lastJumpTime + jumpCooldown)
        {
            // direction opposée au mur
            Vector2 jumpDir = new Vector2(-currentWallSide * wallJumpDirection.x, wallJumpDirection.y);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(jumpDir * wallJumpForce, ForceMode2D.Impulse);

            lastWallSide = currentWallSide;   // on mémorise le mur utilisé
            lastJumpTime = Time.time;

            // lock input horizontal un petit temps pour éviter le "bounce"
            isWallJumping = true;
            amountJumps = 0;
            Invoke(nameof(UnlockWallJump), wallJumpLockTime);
        }
    }

    private void UnlockWallJump()
    {
        isWallJumping = false;
    }

    private void PerformMovement()
    {
        if (!canPlayerMove) return;
        if (isWallJumping) return; // ✅ on empêche le joueur de "casser" son wall jump

        Vector2 move = input.MovementVector;

        // 👉 Met à jour la direction seulement si on appuie vraiment
        if (move.x > 0)
            facingRight = true;
        else if (move.x < 0)
            facingRight = false;

        spriteRenderer.flipX = !facingRight;

        // ✅ NOUVELLE LOGIQUE : empêcher le mouvement horizontal vers un mur si pas en wall slide
        if (!isGrounded && !isWallSliding)
        {
            // Vérifier s'il y a un mur dans la direction du mouvement
            bool blockMovement = false;

            if (move.x < 0) // mouvement vers la gauche
            {
                RaycastHit2D wallLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
                if (wallLeft.collider != null)
                    blockMovement = true;
            }
            else if (move.x > 0) // mouvement vers la droite
            {
                RaycastHit2D wallRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
                if (wallRight.collider != null)
                    blockMovement = true;
            }

            // Si on doit bloquer le mouvement vers le mur, on garde seulement la vélocité Y
            if (blockMovement)
            {
                Debug.Log("coucou1");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, wallSlideSpeed));
                return;
            }
        }

        rb.linearVelocity = new Vector2(move.x * speed, rb.linearVelocity.y);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.BoxCast(groundPoint.position, boxSize, 0, -transform.up, castDistance, groundLayerMask);

        if (isGrounded)
        {
            lastWallSide = 0; // ✅ reset quand on touche le sol
            amountJumps = 0;
        }
    }

    private void CheckWallSlide()
    {
        RaycastHit2D wallLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D wallRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);

        bool isLeftHit = wallLeft.collider != null;
        bool isRightHit = wallRight.collider != null;

        int currentWallSide = isLeftHit ? -1 : (isRightHit ? 1 : 0);

        // ✅ Wall slide si : pas au sol + collé à un mur + tombe
        isWallSliding = !isGrounded && currentWallSide != 0 && rb.linearVelocity.y < 0;

        if (isWallSliding)
        {
            // clamp la chute (que tu presses une touche ou non)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, wallSlideSpeed));
            Debug.Log("coucou");
            amountJumps = 1;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundPoint.position - transform.up * castDistance, boxSize);

        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
    }

    public bool IsFacingRight()
    {
        return facingRight;
    }
}