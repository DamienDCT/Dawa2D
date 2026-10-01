using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    public int ID;
    public Sprite sprite;
    public string itemNameLocalization;
    public string descriptionLocalization;
    public ItemType itemType;
    public int priceToSell;
    public bool canBeSold;
}

[System.Serializable]
public enum ItemType
{
    ITEM,
    SPELL,
}
