using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public const int INVENTORY_SIZE = 40;
    public const int SPELL_INVENTORY_SIZE = 40;
    public const int SELECTED_SPELLS_SIZE = 4;

    [SerializeField] private List<SlotUI> storedItems = new List<SlotUI>();
    [SerializeField] private List<SlotUI> storedSpells = new List<SlotUI>();

    public SlotUI?[] selectedSpells;

    private void Awake()
    {
        Instance = this;

        selectedSpells = new SlotUI?[SELECTED_SPELLS_SIZE];
    }

    private void Start()
    {
        AddItem(0, 10);
        AddItem(1, 5);
        AddSpell(GameDatabases.Instance.GetSpellDatabase().GetItemByID(0));
        AddSpell(GameDatabases.Instance.GetSpellDatabase().GetItemByID(1));
    }

    //private void Update()
    //{
    //    if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
    //    {
    //        InventorySaveManager.Save(this);
    //    }

    //    if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
    //    {
    //        InventorySaveManager.Load(this);
    //    }

    //    if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
    //    {
    //        int i = 0;
    //        foreach(SlotUI? slot in selectedSpells)
    //        {
    //            if (slot != null)
    //                Debug.Log("slot plein = " + slot.Value.itemID);
    //            else
    //                Debug.Log(" slot vide  " + i);

    //            i++;
    //        }
    //    }

    //}

    // Return the list of the soldable items
    public List<SoldableItem> GetSoldableItems()
    {
        var result = new List<SoldableItem>(storedItems.Count);

        foreach (SlotUI slot in storedItems)
        {
            ItemSO item = GameDatabases.Instance.GetItemDatabase().GetItemByID(slot.itemID);
            if (item.canBeSold)
                result.Add(new SoldableItem(item, slot.quantity));
        }

        return result;
    }

    // Return the quantity of the item from an ID
    public int GetQuantity(int itemID)
    {
        foreach (SlotUI slot in storedItems)
            if (slot.itemID == itemID)
                return slot.quantity;
        return 0;
    }

    // Return the list of the item type (null if non-stored type)
    private List<SlotUI> GetListFor(ItemType type)
    {
        switch (type)
        {
            case ItemType.ITEM: return storedItems;
            case ItemType.SPELL: return storedSpells;
            default: return null;
        }
    }

    private int GetMaxSizeFor(ItemType type)
    {
        switch (type)
        {
            case ItemType.ITEM: return INVENTORY_SIZE;
            case ItemType.SPELL: return SPELL_INVENTORY_SIZE;
            default: return 0;
        }
    }

    // Buy item
    public bool BuyObject(ItemSO item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("BuyObject : item null");
            return false;
        }

        List<SlotUI> list = GetListFor(item.itemType);
        if (list == null)
        {
            Debug.LogWarning($"BuyObject : type non géré {item.itemType}");
            return false;
        }

        bool stackable = item.itemType != ItemType.SPELL;

        int index = list.FindIndex(s => s.itemID == item.ID);
        if (index >= 0)
        {
            if (!stackable) return false; // sort déjà appris

            SlotUI slot = list[index]; // struct : copie, donc on réécrit
            slot.quantity += quantity;
            list[index] = slot;
            return true;
        }

        if (list.Count >= GetMaxSizeFor(item.itemType))
        {
            Debug.Log("Inventaire plein !");
            return false;
        }

        list.Add(new SlotUI(item.ID, stackable ? quantity : -1, item.sprite,
                            item.itemNameLocalization, item.descriptionLocalization));
        return true;
    }

    public IReadOnlyList<SlotUI> GetStoredItems() => storedItems;
    public IReadOnlyList<SlotUI> GetStoredSpells() => storedSpells;
    public SlotUI?[] GetSelectedSpells() => selectedSpells;


    // Fonction spéciale de test (pour le start)
    public void AddItem(int ID, int quantity) =>
        BuyObject(GameDatabases.Instance.GetItemDatabase().GetItemByID(ID), quantity);

    public void AddSpell(ItemSO item) => BuyObject(item);

    public bool RemoveItem(int itemID, int quantity)
    {
        int index = storedItems.FindIndex(s => s.itemID == itemID);
        if (index < 0) return false;

        SlotUI slot = storedItems[index];
        if (slot.quantity < quantity) return false;

        slot.quantity -= quantity;

        if (slot.quantity <= 0)
            storedItems.RemoveAt(index);
        else
            storedItems[index] = slot; // réécriture, comme dans BuyObject

        return true;
    }

    public void EquipSpell(string slotName, SlotUI? slotUI)
    {
        int arrayIndex = int.Parse(slotName);

        // Si le spell est déjà équipé
        if(IsSpellAlreadyEquipped(slotUI.Value.itemID, out int equippedIndex))
        {
            // Deux cas : Soit le slot où on clique a déjà un item, soit non.
            // Si déjà un item : On trade les deux
            // Sinon on passe l'item sur l'autre spell
            if (selectedSpells[arrayIndex] == null)
            {
                selectedSpells[arrayIndex] = slotUI;
                selectedSpells[equippedIndex] = null;
            } else
            {
                // Echanger les deux 
                (selectedSpells[arrayIndex], selectedSpells[equippedIndex]) = (selectedSpells[equippedIndex], selectedSpells[arrayIndex]);
            }
        } else
        {
            selectedSpells[arrayIndex] = slotUI;
        }


    }

    public bool CanBuyItem(ShopItem shopItem)
    {
        SaveStats saveStats = GameManager.Instance.GetPlayerStats().Stats;

        return saveStats.CurrentMoney >= shopItem.price;
    }

    private bool IsSpellAlreadyEquipped(int spellID, out int equippedIndex)
    {
        for (int i = 0; i < selectedSpells.Length; i++)
        {
            if (selectedSpells[i] == null)
                continue;

            if (selectedSpells[i].Value.itemID == spellID)
            {
                equippedIndex = i;
                return true;
            }
        }
        equippedIndex = -1;
        return false;

    }

    public void RemoveSpell(int itemID)
    {
        SlotUI item = storedItems.Where(t => t.itemID == itemID).First();

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

[System.Serializable]
public struct SlotUI
{
    public int itemID;
    public int quantity;
    [System.NonSerialized] public Sprite itemSprite;
    [System.NonSerialized] public string itemNameLocalization;
    [System.NonSerialized] public string descriptionLocalization;

    public SlotUI(int itemID, int quantity, Sprite sprite, string itemNameLoc, string descNameLoc)
    {
        this.itemID = itemID;
        this.quantity = quantity;
        this.itemSprite = sprite;

        this.itemNameLocalization = itemNameLoc;
        this.descriptionLocalization = descNameLoc;
    }
}

public readonly struct SoldableItem
{
    public readonly ItemSO item;
    public readonly int quantity;

    public SoldableItem(ItemSO item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}