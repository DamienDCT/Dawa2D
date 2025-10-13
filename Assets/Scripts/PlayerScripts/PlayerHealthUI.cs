using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Transform heartPrefab;
    [SerializeField] private Transform heartsParent;
    private int maxHealth;
    private List<Image> hearts;

    private void Awake()
    {
        hearts = new List<Image>();
    }

    public void InitialiazeUI(int maxHealth)
    {
        this.maxHealth = maxHealth;
        for(int i = 0; i < maxHealth; i++)
        {
            Transform heartInstantiated = Instantiate(heartPrefab, heartsParent);
            Image img = heartInstantiated.GetComponent<Image>();
            if(img != null)
            {
                hearts.Add(img);
            }
        }
    }


    public void UpdateUI(int healthAmount)
    {
        for(int i = 0; i < maxHealth; i++)
        {
            if(i < healthAmount)
            {
                hearts[i].color = Color.red;
            } else
            {
                hearts[i].color = Color.grey;
            }
        }
    }
}
