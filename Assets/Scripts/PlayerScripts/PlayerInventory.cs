using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
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

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            InventorySaveManager.Save(this);
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            InventorySaveManager.Load(this);
        }

        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
        {
            int i = 0;
            foreach(SlotUI? slot in selectedSpells)
            {
                if (slot != null)
                    Debug.Log("slot plein = " + slot.Value.itemID);
                else
                    Debug.Log(" slot vide  " + i);

                i++;
            }
        }

    }



    public IReadOnlyList<SlotUI> GetStoredItems() => storedItems;
    public IReadOnlyList<SlotUI> GetStoredSpells() => storedSpells;
    public SlotUI?[] GetSelectedSpells() => selectedSpells;

    public void AddItem(int ID, int quantity)
    {
        if (storedItems.Count >= INVENTORY_SIZE)
        {
            Debug.Log("Inventaire plein !");
            return;
        }
        ItemSO item = GameDatabases.Instance.GetItemDatabase().GetItemByID(ID);
        Debug.Log(item.name);
        Debug.Log(item.sprite.name);
        Sprite sprite = null;
        if(item != null)
        {
            sprite = item.sprite;
        }
        storedItems.Add(new SlotUI(ID, quantity, sprite, item.itemNameLocalization, item.descriptionLocalization));
    }

    public void AddSpell(ItemSO item)
    {
        if (storedSpells.Count >= SPELL_INVENTORY_SIZE)
        {
            Debug.Log("Inventaire de sorts plein !");
            return;
        }

        storedSpells.Add(new SlotUI(item.ID, -1, item.sprite, item.itemNameLocalization, item.descriptionLocalization));
    }

    public void RemoveItem(int itemID, int quantity)
    {
        SlotUI item = storedItems.Where(t => t.itemID == itemID).First();
        item.quantity -= quantity;
        if(item.quantity <= 0)
        {
            storedItems.Remove(item);
        }
       // storedItems.Remove(item);
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