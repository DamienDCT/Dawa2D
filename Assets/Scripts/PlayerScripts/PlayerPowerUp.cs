using System;
using System.Collections;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    [SerializeField] private PlayerMovements playerMovement; // Reference to the player movements
    [SerializeField] private PlayerPhaseSelector playerPhaseSelector; // Reference to the player phase selector
    [SerializeField] private Transform gemPrefab; // Prefab of the gem that can spawn
    [SerializeField] private LayerMask groundLayerMask; // Ground layer mask

    [Header("Esquive Filoryn")]
    [SerializeField] private float dashDistanceX = 8f;       // combien le joueur avance
    [SerializeField] private float backgroundZ = 12f;       // profondeur max
    [SerializeField] private float arcHeight = 2f;           // hauteur max de la courbe
    [SerializeField] private float dashCooldown = 0.6f;      // durée totale
    public bool IsDashing { get; private set; }

    [Header("Ascension Filoryn")]
    [SerializeField] private float dashForceY = 22.5f; // Force of the filoryn ascension
    //[SerializeField] private float ascensionCooldown = 0.8f;

    [SerializeField] private bool isCasting = false; // Bool if the player is casting a filoryn gem powerup
    private float widthPlayerCollider; // Variable that stores the half of the collider
     
    [Header("Massue Filoryn")]
    [SerializeField] private Transform massuePrefab; // Prefab of what spawns when using filoryn hammer
    [SerializeField] private Transform spawnMassuePrefab; // Transform where the filoryn hammer spawns
    [SerializeField] private float hammerCooldown = 0.5f; // Cooldown (duration) of the hammer

    // Enemy informations
    private bool hasHitEnemy = false; // Variable if we hit enemy while dashing with filoryn gem
    private float positionX = 0f; // X positon of the enemy in the background
    private Rigidbody2D rb; // Rigidbody of the player
    [SerializeField] private DetectEnemyBackground detectEnemyBackground; // Script of detecting the background

    [Header("Checkbox of move feasible")] // Variables if we can use the gem 
    [SerializeField] private bool canUseAscension; 
    [SerializeField] private bool canUseDodge;
    [SerializeField] private bool canUseHammer;


    void Start()
    { 

        // Get the width of the half of the collider
        CapsuleCollider2D capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        widthPlayerCollider = capsuleCollider2D.size.x / 2;

        // Reference to RB
        rb = GetComponent<Rigidbody2D>();

        // We can use all of the useable powerups
        canUseAscension = true;
        canUseDodge = true;
        canUseHammer = true;
    }

    private void OnEnable()
    {
        // Subscribing to the event of spell
        InputController.Instance.OnPowerUpButtonPressed += UsePowerUp;
    }

    private void OnDisable()
    {
        InputController.Instance.OnPowerUpButtonPressed -= UsePowerUp;
    }

    private void Update()
    {
        if (playerPhaseSelector.IsChangingPhase)
            return;
        // Si on cast un spell et que notre vitesse linéaire y est proche du peak du saut
        if(isCasting && rb.linearVelocity.y < 0.1f && playerMovement.LastOnGroundTime < 0 && !IsDashing)
        {
            isCasting = false;
            Debug.Log("we can use a spell");
        }
    }

    public bool IsCasting()
    {
        return isCasting;
    }

    public void ResetPowerUpFeasibility()
    {
        canUseAscension = true;
        canUseDodge = true;
        canUseHammer = true;
    }

    private void UsePowerUp(object sender, PlayerPowerUpArgs args)
    {
        if (playerPhaseSelector.IsChangingPhase)
            return;

        switch (args.powerUpType)
        {
            case PowerUpType.ESQUIVE:
                FilorynDodge();
                break;
            case PowerUpType.MASSUE:
                FilorynHammer();
                break;
            case PowerUpType.ASCENSION:
                FilorynAscension();
                break;
        }
    }

    private void FilorynHammer()
    {
        if (isCasting)
            return;
        if (!canUseHammer)
            return;

        canUseHammer = false;
        isCasting = true;

        bool wasInAirWhenCast = false;
        if(playerMovement.LastOnGroundTime < 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            wasInAirWhenCast = true;
        }

        Transform instantiatedMassue = Instantiate(massuePrefab, spawnMassuePrefab.transform.position, Quaternion.identity, this.transform);
        StartCoroutine(FilorynHammerTransition(wasInAirWhenCast));
    }

    private IEnumerator FilorynHammerTransition(bool wasInAirWhenCast)
    {
        yield return new WaitForSeconds(hammerCooldown);
        isCasting = false;
        if(wasInAirWhenCast)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        if(playerMovement.LastOnGroundTime > 0)
        {
            canUseHammer = true;
        }
    }

    private void FilorynAscension()
    {
        if (isCasting) return;
        if (!canUseAscension) return;

        isCasting = true;
        Debug.Log(isCasting);
        canUseAscension = false;

        StartCoroutine(FilorynAscensionTransition());


    }

    private IEnumerator FilorynAscensionTransition()
    {

        yield return new WaitForSeconds(0.5f);
        playerMovement.Jump(dashForceY);
/*        yield return new WaitForSeconds(ascensionDuration);*/
    }

    private void FilorynDodge()
    {
        if (isCasting) return;
        if (!canUseDodge) return;

        canUseDodge = false;
        IsDashing = true;

        Vector3 start = transform.position;
        int dir = playerMovement.IsFacingRight ? 1 : -1;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir == 1 ? Vector3.right : Vector3.left, dashDistanceX, groundLayerMask);
        Debug.DrawRay(transform.position, dir == 1 ? Vector3.right * dashDistanceX : Vector3.left * dashDistanceX, Color.green, 3f);
        if (hit)
        {
            float distance = hit.point.x - dir * widthPlayerCollider;
           // Instantiate(gemPrefab, new Vector3(distance, hit.point.y, 0f), Quaternion.identity);
            Vector3 end = new Vector3(
                distance,
                        start.y,     // on revient à la même hauteur
                0f    // et au même plan
            );
            StartCoroutine(DashCurve(dir, end));
        }
        else
        {
            Vector3 end = new Vector3(
                start.x + dir * dashDistanceX,
                        start.y,     // on revient à la même hauteur
                0f      // et au même plan
            );
            StartCoroutine(DashCurve(dir, end));
        }

        // StartCoroutine(DashCurve());
    }

    private IEnumerator DashCurve(int dir, Vector3 end)
    {
        isCasting = true;
        detectEnemyBackground.hasToFixedZ = true;
        Vector3 start = transform.position;

        int isDashForeground = UnityEngine.Random.Range(0, 2);
        float _backgroundZ = backgroundZ;

        // Point de contrôle (milieu de la courbe)
        Vector3 control = new Vector3(
            (start.x + end.x) / 2f,
            start.y + arcHeight,
            _backgroundZ
        );

        Transform gemInstantiated = Instantiate(gemPrefab, new Vector3(control.x, end.y, 0f), Quaternion.identity);
        Destroy(gemInstantiated.gameObject, .5f);

        float elapsed = 0f;
        Vector3 targetEnd = end; // cible "finale" qui pourra changer si on touche un ennemi

        while (elapsed < dashCooldown)
        {
            float t = elapsed / dashCooldown;

            // Si on touche un ennemi, on met à jour la cible
            if (hasHitEnemy)
            {
                hasHitEnemy = false;

                // On s'aligne exactement sur l'ennemi en X
                float newX = positionX;

                // On garde la hauteur du dash (end.y), mais on force le joueur sur son plan Z
                Vector3 newEnd = new Vector3(newX, end.y, 0f);

                targetEnd = newEnd;
            }

            // end glisse petit à petit vers la targetEnd → transition douce
            end = Vector3.Lerp(end, targetEnd, Time.deltaTime * 10f);

            // Bézier quadratique avec le end courant
            Vector3 m1 = Vector3.Lerp(start, control, t);
            Vector3 m2 = Vector3.Lerp(control, end, t);
            transform.position = Vector3.Lerp(m1, m2, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

    //    transform.position = end;
        isCasting = false;
        IsDashing = false;
        detectEnemyBackground.hasToFixedZ = false;
        transform.position = new Vector3(end.x, end.y, end.z); // 0f si ton plan joueur est en Z=0
        
        if (playerMovement.LastOnGroundTime < 0)
        {
            yield return new WaitForSeconds(0.5f);
            canUseDodge = true;
        }
    }



    public void HitPlayer(Collider2D collider)
    {
        if(collider.CompareTag("Enemy") && isCasting)
        {
            //   Debug.Log(collider.name);
            hasHitEnemy = true;
            positionX = collider.transform.position.x;
            Debug.Log(positionX);
        }
    }

}

[System.Serializable]
public class PlayerPowerUpArgs : EventArgs
{
    public PowerUpType powerUpType;
}

[System.Serializable]
public enum PowerUpType
{
    ESQUIVE,
    MASSUE,
    ASCENSION,
}