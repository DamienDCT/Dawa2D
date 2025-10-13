using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private PlayerMovements playerMovements;

    [Header("Animation Timings")]
    [SerializeField] private float longIdleTimer = 20f;

    private void Update()
    {
        animator.SetFloat("timeAFK", playerMovements.TimeNoMove);
        animator.SetFloat("velocity", playerMovements.Velocity);
    }

    private void LateUpdate()
    {
        if (playerMovements.TimeNoMove > (longIdleTimer + 0.1f))
        {
            playerMovements.TimeNoMove = 0f;
        }
    }

    /*    public void ResetTimerAfk()
        {
            playerMovements.TimeNoMove = 0f;
        }*/
}
