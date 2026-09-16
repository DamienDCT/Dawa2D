using UnityEngine;

// Spécialise Destructable pour le joueur : la seule chose qui change,
// c'est ce qu'on fait quand il est touché (mise à jour de l'UI).
// Toute la logique de dégâts, blink, mort, screen shake reste dans
// Destructable et n'a pas besoin d'être dupliquée ici.
public class PlayerDestructable : Destructable
{
    [Header("Player only")]
    [SerializeField] private PlayerHealthUI playerHealth;

    protected override void Awake()
    {
        base.Awake();
        if(playerHealth != null)
        {
            playerHealth.InitializeUI((int)healthAmount);
        }
    }

    protected override void OnDamaged(float damageAmount, Vector2 hitPosition)
    {
        // TODO : passer les dégâts en int GLOBAUX, de manière uniforme.
        if (playerHealth != null)
            playerHealth.UpdateUI((int)healthAmount);

        Debug.Log("we hit the player");
    }

    protected override void OnDeath()
    {
        // Logique spécifique à la mort du joueur : game over, respawn,
        // écran noir, etc. Vide pour l'instant.
    }
}