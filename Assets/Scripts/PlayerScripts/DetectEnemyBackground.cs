using UnityEngine;

public class DetectEnemyBackground : MonoBehaviour
{
    [SerializeField] private PlayerPowerUp playerSpell;

    [SerializeField] private float fixedZ = 5f;

    public bool hasToFixedZ = false;

    void LateUpdate()
    {
        if (!hasToFixedZ)
            return;

        Vector3 pos = transform.position;
        pos.z = fixedZ;   // tu figes le Z peu importe le parent
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      
        //   Debug.Log(other.name);
      /*  if (other.CompareTag("Enemy"))
        {
            //Debug.Log("coucou");
            playerSpell.HitPlayer(other);

        }*/
    }
}
