using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class SpellCircleUI : MonoBehaviour
{
    [SerializeField] private List<SpellCircleSlot> allSpellCircleSlots; // on change en SpellCircleSlot, qu’on va créer
    private ItemSO itemPendingSelection; // sort en attente d'assignation
    private bool selectionMode = false;

    [SerializeField] private Button deleteButton;
    [SerializeField] private InventoryUI inventoryUI;

    private void Awake()
    {
        foreach (var slot in allSpellCircleSlots)
        {
            slot.Initialize(this);
        }
        DisableSlots();
    }

    private void DisableSlots()
    {
        foreach (var slot in allSpellCircleSlots)
        {
            slot.DisableSlot();
        }
    }

    // Appelé quand on clique sur un SpellSlot avec un item
    public void StartSelection(ItemSO item)
    {
        itemPendingSelection = item;
        selectionMode = true;

        Debug.Log($"Mode sélection activé pour {item.itemNameLocalization}");

        // Active visuellement les slots du cercle
        foreach (var slot in allSpellCircleSlots)
        {
            slot.EnableSlotForSelection(this);
        }
    }

    public void SetItemFromInventory(SpellCircleSlot slot, ItemSO item)
    {
        itemPendingSelection = item;
        selectionMode = true;
        AssignToSlot(slot);
    }

    // Appelé quand on clique sur un slot du cercle
    public void AssignToSlot(SpellCircleSlot circleSlot)
    {
        if (!selectionMode || itemPendingSelection == null)
            return;

        if (HasAlreadyItem(itemPendingSelection, out SpellCircleSlot _circleSlot))
        {
            // Echange de l'item
            _circleSlot.SetSpell(null);
            if(circleSlot.GetCurrentSpell() != null)
            {
                ItemSO previousItem = circleSlot.GetCurrentSpell();
                circleSlot.SetSpell(itemPendingSelection);
                _circleSlot.SetSpell(previousItem);

                // On sort du mode sélection
                selectionMode = false;
                itemPendingSelection = null;

                // Désactive les autres slots
                foreach (var slot in allSpellCircleSlots)
                {
                    slot.DisableSlot();
                }
                return;
            }
        }

        circleSlot.SetSpell(itemPendingSelection);
        Debug.Log($"Sort {itemPendingSelection.itemNameLocalization} assigné à {circleSlot.name}");

        // On sort du mode sélection
        selectionMode = false;
        itemPendingSelection = null;

        // Désactive les autres slots
        foreach (var slot in allSpellCircleSlots)
        {
            slot.DisableSlot();
        }
    }

    private bool HasAlreadyItem(ItemSO item, out SpellCircleSlot circleSlot)
    {
        foreach(var slot in allSpellCircleSlots)
        {
            if (slot.GetCurrentSpell() == null)
                continue;

            if(slot.GetCurrentSpell().ID == item.ID)
            {
                circleSlot = slot;
                return true;
            }
        }
        circleSlot = null;
        return false;
    }

    public void ShowDeleteButton(SpellCircleSlot circleSlot)
    {
        if (circleSlot.GetCurrentSpell() == null)
            return;

        deleteButton.gameObject.SetActive(true);

        deleteButton.onClick.RemoveAllListeners();

        deleteButton.onClick.AddListener(() =>
        {
            circleSlot.SetSpell(null);
            inventoryUI.FocusFirstSlot();
            deleteButton.gameObject.SetActive(false);
        });
    }

    public void StartSelectionOnInventory(SpellCircleSlot slot)
    {
        inventoryUI.FocusFirstSlot();
        inventoryUI.SetCurrentSlot(slot);
    }
}