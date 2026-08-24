using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public const int INVENTORY_SIZE = 40;
    public const int SPELL_INVENTORY_SIZE = 40;
    public const int SELECTED_SPELLS_SIZE = 4;

    [SerializeField] private List<ItemSO> storedItems = new List<ItemSO>();
    [SerializeField] private List<ItemSO> storedSpells = new List<ItemSO>();

    public ItemSO[] selectedSpells;

    private void Awake()
    {
        Instance = this;

        selectedSpells = new ItemSO[SELECTED_SPELLS_SIZE];
    }

    private void Start()
    {
        AddItem(GameDatabases.Instance.GetItemDatabase().GetItemByID(0));
        AddItem(GameDatabases.Instance.GetItemDatabase().GetItemByID(1));
        AddSpell(GameDatabases.Instance.GetSpellDatabase().GetItemByID(0));
        AddSpell(GameDatabases.Instance.GetSpellDatabase().GetItemByID(1));
    }



    public IReadOnlyList<ItemSO> GetStoredItems() => storedItems;
    public IReadOnlyList<ItemSO> GetStoredSpells() => storedSpells;

    public void AddItem(ItemSO item)
    {
        if (storedItems.Count >= INVENTORY_SIZE)
        {
            Debug.Log("Inventaire plein !");
            return;
        }

        storedItems.Add(item);
    }

    public void AddSpell(ItemSO item)
    {
        if (storedSpells.Count >= SPELL_INVENTORY_SIZE)
        {
            Debug.Log("Inventaire de sorts plein !");
            return;
        }

        storedSpells.Add(item);
    }

    public void RemoveItem(ItemSO item)
    {
        storedItems.Remove(item);
    }

    public void RemoveSpell(ItemSO item)
    {
        storedSpells.Remove(item);
    }

    public void ClearInventory()
    {
        storedItems.Clear();
    }

    public void ClearSpells()
    {
        storedSpells.Clear();
    }
}