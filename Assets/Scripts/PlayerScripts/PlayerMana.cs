using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [SerializeField] private PlayerManaUI playerManaUI;
    [SerializeField] private float maxManaAmount = 100f;
    private float currentManaAmount;

    private void Awake()
    {
        currentManaAmount = maxManaAmount;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            UseMana(10f);
        } else if(Input.GetKeyDown (KeyCode.J)) 
        {
            RegainMana(10f);
        }
    }

    public void UseMana(float amount)
    {
        currentManaAmount = Mathf.Clamp(currentManaAmount - amount, 0f, maxManaAmount);
        playerManaUI.UpdateUI(currentManaAmount / maxManaAmount);
    }

    public void RegainMana(float amount)
    {
        currentManaAmount = Mathf.Clamp(currentManaAmount + amount, 0f, maxManaAmount);
        playerManaUI.UpdateUI(currentManaAmount / maxManaAmount);
    }
}
