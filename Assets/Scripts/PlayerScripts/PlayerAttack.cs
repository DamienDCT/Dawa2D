using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class PlayerAttack : MonoBehaviour
{


    [Header("Arc Settings")]
    public float radius = 2f;       // Rayon extérieur
    public float thickness = 0.5f;  // Épaisseur de l'arc
    public float startAngle = -90f; // Angle de début (degrés)
    public float endAngle = 90f;    // Angle de fin (degrés)
    public int segments = 12;       // Plus c'est élevé, plus l'arc est lisse

    [SerializeField] private float attackCooldown = 1.5f;

    [SerializeField] private float damageValue = 2f;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask enemyLayerMask;

    //private float attackTimer = 0f;
    private bool canAttack = true;

    private PolygonCollider2D poly;
    private ContactFilter2D enemyContactFilter;
    private void Awake()
    {
        InputController.Instance.OnAttackButtonPressed += Attack;

        poly = GetComponent<PolygonCollider2D>();
        GenerateArc();
        poly.enabled = false;
        enemyContactFilter = new ContactFilter2D();
        enemyContactFilter.SetLayerMask(enemyLayerMask);
        enemyContactFilter.useLayerMask = true;
    }


    private void Attack(object sender, EventArgs args)
    {
        if (!canAttack)
            return;

        StartCoroutine(DetectEnemies());
    }

    /*private IEnumerator DetectEnemies()
    {
        poly.enabled = true;
        canAttack = false;


        var enemies = poly.GetContactColliders(enemyContactFilter);

        foreach( var enemy in enemies )
        {
            Destructable destructable = enemy.GetComponent<Destructable>();
            if(destructable == null)
            {
                Debug.Log("destructable = null");
            }
            destructable?.Hit(damageValue);
        }

        yield return new WaitForSeconds(attackCooldown);
        poly.enabled = false;
        canAttack = true;
    }*/
    private IEnumerator DetectEnemies()
    {
        poly.enabled = true;
        canAttack = false;

        Collider2D[] results = new Collider2D[16];
        int count = Physics2D.OverlapCollider(poly, enemyContactFilter, results);

        for (int i = 0; i < count; i++)
        {
            Destructable destructable = results[i].GetComponent<Destructable>();
            if (destructable == null)
            {
                Debug.Log("destructable = null");
                continue;
            }

            // ClosestPoint only needs the ENEMY collider (circle = convex)
            // to be well-defined — it doesn't care that our poly is concave.
            Vector2 contactPoint = results[i].ClosestPoint(transform.position);

            Debug.Log(contactPoint);
            destructable.Hit(damageValue, contactPoint);
        }

        yield return new WaitForSeconds(attackCooldown);
        poly.enabled = false;
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (poly == null) poly = GetComponent<PolygonCollider2D>();
        GenerateArc();
    }
#endif
    private void GenerateArc()
    {
        if (segments < 2) segments = 2;

        // Points totaux : segments extérieurs + segments intérieurs
        Vector2[] points = new Vector2[(segments + 1) * 2];

        // On trace l’arc extérieur
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
            points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        // On trace l’arc intérieur (dans l’autre sens pour fermer le polygone)
        for (int i = 0; i <= segments; i++)
        {
            float t = (segments - i) / (float)segments; // inverse pour fermer
            float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
            points[segments + 1 + i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (radius - thickness);
        }

        poly.pathCount = 1;
        poly.SetPath(0, points);
    }
}