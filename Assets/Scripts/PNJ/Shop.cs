using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<ShopItem> itemShopList;

    public List<ShopItem> GetListItemShop => itemShopList;
}

[System.Serializable]
public struct ShopItem
{
    public ItemSO soldItem; 
    public int quantityRemaining;
    public int price;
    public bool isUniqueItem;
    public string itemLocalizationShopDescription;
}