using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image slotItemImage;

    [SerializeField] private Selectable selectable;

    [SerializeField] private Sprite transparentSprite;

    [SerializeField] private ItemInfo itemInfo;
    protected ItemSO currentItem;

    public void SetItemIcon(ItemSO item)
    {
        currentItem = item;
        if (item == null)
        {
            // rend le slot non sélectionnable
            selectable.enabled = false;

            // Désactivation du sprite
            slotItemImage.sprite = transparentSprite;
        }
        else
        {
            // Rend le slot sélectionable
            selectable.enabled = true;

            // Màj du sprite
            slotItemImage.enabled = true;
            slotItemImage.sprite = item.sprite;

            itemInfo.SetOverrideName(item);
        }
    }

    public ItemSO GetCurrentItem()
    {
        return this.currentItem;
    }

}
