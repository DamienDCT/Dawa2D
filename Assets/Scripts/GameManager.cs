using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SaveData Data = new SaveData();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    // TODO : some stuff to save and load data when starting a save
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
