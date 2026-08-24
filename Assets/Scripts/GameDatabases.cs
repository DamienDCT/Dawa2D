using UnityEngine;

public class GameDatabases : MonoBehaviour
{
    public static GameDatabases Instance;

    [SerializeField] private ItemDatabase ItemDatabase;
    [SerializeField] private ItemDatabase SpellDatabase;

    private void Awake()
    {
        Instance = this;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        ItemDatabase = Resources.Load<ItemDatabase>("ItemDatabase");
        SpellDatabase = Resources.Load<ItemDatabase>("SpellDatabase");
    }

    public ItemDatabase GetItemDatabase()
    {
        return this.ItemDatabase;
    }

    public ItemDatabase GetSpellDatabase()
    {
        return this.SpellDatabase;
    }

    // Enemy database etc...
}