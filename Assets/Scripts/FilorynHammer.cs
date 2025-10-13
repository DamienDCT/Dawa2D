using UnityEngine;

public class FilorynHammer : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("BreakableGround"))
        {
            // TODO : VFX/SFX        
            Destroy(this.gameObject);
            return;
        }

        Destroy(gameObject);

    }
}
