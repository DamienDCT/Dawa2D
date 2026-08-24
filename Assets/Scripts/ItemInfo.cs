using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public string itemNameLocalization;

    public string itemDescriptionLocalization;
    public Sprite itemSprite;



    public void SetOverrideName(ItemSO item)
    {
        this.itemNameLocalization = item.itemNameLocalization;
        this.itemDescriptionLocalization = item.descriptionLocalization;
        this.itemSprite = item.sprite;
    }
}
