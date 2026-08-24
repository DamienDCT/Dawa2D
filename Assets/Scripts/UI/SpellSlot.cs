using UnityEngine;
using UnityEngine.EventSystems;

public class SpellSlot : Slot, IPointerClickHandler
{
    [SerializeField] private SpellCircleUI spellCircleUI;
    [SerializeField] private InventoryUI inventoryUI;

    private SpellCircleSlot spellCircleSlot;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null)
            return;

        Debug.Log("we have this item " + currentItem.itemNameLocalization);
        if(spellCircleSlot != null)
        {
            
            spellCircleUI.SetItemFromInventory(spellCircleSlot, currentItem);
            inventoryUI.ClearCurrentSlotOutput();
        } else
        {
            spellCircleUI.StartSelection(currentItem);
        }
    }

    public void SetOutputOnClick(SpellCircleSlot slot)
    {
        this.spellCircleSlot = slot;
    }
}
