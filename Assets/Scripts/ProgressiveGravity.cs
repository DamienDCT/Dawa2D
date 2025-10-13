using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProgressiveGravity : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float maxFalldownSpeed;
    [SerializeField] private float gravityScale;
    [SerializeField] private float fallGravityMult;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(rb.linearVelocity.y < 0f)
        {
            //Higher gravity if falling
            SetGravityScale(gravityScale * fallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFalldownSpeed));
        }
    }

    private void SetGravityScale(float scale)
    {
        this.rb.gravityScale = scale;
    }
}
