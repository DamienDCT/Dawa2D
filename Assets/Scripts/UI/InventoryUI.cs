using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<Slot> itemSlots;
    [SerializeField] private List<SpellSlot> spellSlots;

    [Header("Menus references")]
    [SerializeField] private RectTransform itemMenu;
    [SerializeField] private RectTransform spellMenu;

    // Seulement pour les spells
    private SpellCircleSlot spellCircleSlot;

    private void Awake()
    {
        spellCircleSlot = null;
    }

    private void OnEnable()
    {
        UpdateUI();
    }


    public void GoToItemMenu()
    {
        itemMenu.gameObject.SetActive(true);
        spellMenu.gameObject.SetActive(false);
        spellCircleSlot = null;
    }

    public void GoToSpellMenu()
    {
        itemMenu.gameObject.SetActive(false);
        spellMenu.gameObject.SetActive(true);
    }

    public void UpdateUI()
    {
        var items = PlayerInventory.Instance.GetStoredItems();

        // Items slots
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count)
                itemSlots[i].SetItemIcon(items[i]);
            else
                itemSlots[i].SetItemIcon(null); // vide
        }

        // 

        var spells = PlayerInventory.Instance.GetStoredSpells();

        // Items slots
        for (int i = 0; i < spellSlots.Count; i++)
        {
            if (i < spells.Count)
            {
                spellSlots[i].SetItemIcon(spells[i]);
                spellSlots[i].SetOutputOnClick(null);
            }
            else
                spellSlots[i].SetItemIcon(null); // vide
        }
    }

    public void FocusFirstSlot()
    {
        spellSlots[0].GetComponent<Selectable>().Select();
    }

    public void SetCurrentSlot(SpellCircleSlot slot)
    {
       // this.spellCircleSlot = slot;
        for (int i = 0; i < spellSlots.Count; i++)
        {
            if (spellSlots[i].GetCurrentItem() == null)
                continue;

            spellSlots[i].SetOutputOnClick(slot);
        }
    }

    public void ClearCurrentSlotOutput()
    {
        for (int i = 0; i < spellSlots.Count; i++)
        {
            spellSlots[i].SetOutputOnClick(null);
        }
    }
}
