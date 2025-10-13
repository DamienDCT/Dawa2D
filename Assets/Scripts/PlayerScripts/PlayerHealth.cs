using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerHealthUI playerHealthUI; // Reference to the player health UI to update HUD

    private int currentHealth; // Current health of the player
    [SerializeField] private int maxHealth = 5; // Max amount of hearts

    private void Start()
    {
        currentHealth = maxHealth;

        //   PlayerHealthUI.Instance.InitialiazeUI(maxHealth);
        playerHealthUI.InitialiazeUI(maxHealth);
    }

   /* public void Update()
    {
        if(currentHealth > 0)
        {
            if(Input.GetKeyDown(KeyCode.U))
            {
                TakeDamage(1);
            } else if(Input.GetKeyDown(KeyCode.V))
            {
                Heal(1);
            }
        }
    }
*/

    public void TakeDamage(int amount)
    {
        // We update the health
        currentHealth = Mathf.Max(0, currentHealth - amount);

        // Die if we have less or equal 0 hearts
        if(currentHealth <= 0)
        {
            Die();
        }

        // We update the UI
        playerHealthUI.UpdateUI(currentHealth);

    }

    private void Die()
    {
        Debug.Log("we died");
    }

    public void Heal(int amount)
    {
        // We increase the currentHealth by the amount
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        // And update the UI after
        playerHealthUI.UpdateUI(currentHealth);
    }
}
