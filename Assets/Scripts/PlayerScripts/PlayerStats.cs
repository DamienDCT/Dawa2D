using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public SaveData Data = new SaveData();
    public SaveStats Stats;

    private void Awake()
    {
        Stats = new SaveStats(300);
    }

    private void Update()
    {
        if (Stats != null)
        {
            Debug.Log(Stats.CurrentMoney);
        }
    }

    public void RemoveMoney(int price)
    {
        Stats.CurrentMoney -= Mathf.Max(0, price);
    }

    public void AddMoney(int price)
    {
        Stats.CurrentMoney += price;
    }
}


[System.Serializable]
public class SaveData
{
    public int CurrentHealth; // Current health of the player when he quits the game
    public int MaxHealth; // Max health (in case he earns some hearts definitively)
    public int CurrentMana; // Current mana when player quits
    public List<string> AbilitiesUnlocked; // Abilities unlocked in his save
    public HashSet<string> BossesDefeated; // Boss defeated (to make them not respawn)
}

[System.Serializable]
public class SaveStats
{
    public int CurrentMoney; // Current money of the player when he quits the game

    public SaveStats(int currentMoney)
    {
        this.CurrentMoney = currentMoney;
    }
}