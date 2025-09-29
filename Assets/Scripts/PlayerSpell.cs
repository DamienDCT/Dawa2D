using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    [SerializeField] private PlayerMovements playerMovement;
    [SerializeField] private Transform gemPrefab;
    [SerializeField] private LayerMask wallGroundLayerMask;

    [Header("Esquive Filoryn")]
    [SerializeField] private float dashDistanceX = 8f;       // combien le joueur avance
    [SerializeField] private float backgroundZ = 12f;       // profondeur max
    [SerializeField] private float arcHeight = 2f;           // hauteur max de la courbe
    [SerializeField] private float dashDuration = 0.6f;      // durée totale

    [Header("Ascension Filoryn")]
    [SerializeField] private float dashForceY = 22.5f;

    private bool isCasting = false;
    private float widthPlayerCollider;

    // Enemy informations
    private bool hasHitEnemy = false;
    private float positionX = 0f;

    [SerializeField] private DetectEnemyBackground detectEnemyBackground;

    void Start()
    {
        InputController.Instance.OnBasicSpellStarted += CastSpell;
        CapsuleCollider2D capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        widthPlayerCollider = capsuleCollider2D.size.x / 2;
    }

    private void CastSpell(object sender, PlayerSpellArgs args)
    {
        switch(args.spellType)
        {
            case SpellType.ESQUIVE:
                FilorynDodge();
                break;
            case SpellType.MASSUE:
                break;
            case SpellType.ASCENSION:
                FilorynAscension();
                break;
        }
    }

    private void FilorynAscension()
    {
        if (isCasting) return;

        isCasting = true;

        StartCoroutine(FilorynAscensionTransition());


    }

    private IEnumerator FilorynAscensionTransition()
    {

        yield return new WaitForSeconds(0.5f);
        playerMovement.Jump(dashForceY);
        isCasting = false;
    }

    private void FilorynDodge()
    {
        if (isCasting) return;
        Vector3 start = transform.position;
        int dir = playerMovement.IsFacingRight ? 1 : -1;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir == 1 ? Vector3.right : Vector3.left, dashDistanceX, wallGroundLayerMask);
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
        playerMovement.enabled = false; // désactive le contrôle joueur
        detectEnemyBackground.hasToFixedZ = true;
        Vector3 start = transform.position;

        // Fin du dash → plus loin en X
        /*        Vector3 end = new Vector3(
                    start.x + dir * dashDistanceX,
                    start.y,     // on revient à la même hauteur
                    start.z      // et au même plan
                );*/
        int isDashForeground = UnityEngine.Random.Range(0, 2);


        float _backgroundZ = backgroundZ;
 /*       if (isDashForeground == 1)
        {
            _backgroundZ *= -1;
        }        */

        // Point de contrôle (milieu de la courbe)
        Vector3 control = new Vector3(
            (start.x + end.x) / 2f,
        start.y + arcHeight,
        _backgroundZ
        );

        Transform gemInstantiated = Instantiate(gemPrefab, new Vector3(control.x, end.y, 0f), Quaternion.identity);
        Destroy(gemInstantiated.gameObject, .5f);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            float t = elapsed / dashDuration;
            // Bézier quadratique
            Vector3 pos = Mathf.Pow(1 - t, 2) * start
                        + 2 * (1 - t) * t * control
                        + Mathf.Pow(t, 2) * end;

            if(hasHitEnemy)
            {
                end = new Vector3(positionX, end.y, end.z);
                hasHitEnemy = false;
            }

            transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        playerMovement.enabled = true;
        isCasting = false;
        detectEnemyBackground.hasToFixedZ = false;
    }


    public void HitPlayer(Collider2D collider)
    {
        if(collider.CompareTag("Enemy") && isCasting)
        {
            //   Debug.Log(collider.name);
            hasHitEnemy = true;
            positionX = collider.transform.position.x;
        }
    }

}

[System.Serializable]
public class PlayerSpellArgs : EventArgs
{
    public SpellType spellType;
}

[System.Serializable]
public enum SpellType
{
    ESQUIVE,
    MASSUE,
    ASCENSION,
}