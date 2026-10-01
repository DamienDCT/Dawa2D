using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private string shopName;
    [SerializeField] private List<ShopItem> itemShopList;

    public List<ShopItem> GetListItemShop => itemShopList;

    public bool CanInteract => canInteract;

    private bool canInteract = true;


    // Dialogue de choix avant d'ouvrir
    [SerializeField] private DialogNode startDialogNode;

    public string GetShopName()
    {
        return this.shopName;
    }

    public void Interact()
    {
        DialogBoxUITK.Instance.StartDialog(startDialogNode);

        // Setup both inventories
        ShopUITK.Instance.SetShop(this);
        SellShopUITK.Instance.SetShop(this);

        canInteract = false;
    }

    public bool Buy(ShopItem shopItem)
    {
        int index = itemShopList.FindIndex(i => i.soldItem.ID == shopItem.soldItem.ID);
        if (index < 0) return false;
        ShopItem itemOnList = itemShopList[index];

        if (itemOnList.quantityRemaining == 0)
            return false;

        // Check-up player money
        if(!PlayerInventory.Instance.CanBuyItem(itemOnList))
        {
            Debug.Log("we don't have enough money");
            return false;
        }


        // Add item to inventory
        if(!PlayerInventory.Instance.BuyObject(shopItem.soldItem))
            return false;


        // Decrease quantity
        itemOnList.quantityRemaining--;
        itemShopList[index] = itemOnList; // réécriture, sinon la modif est perdue
        GameManager.Instance.GetPlayerStats()?.RemoveMoney(shopItem.price);

        return true;
    }

    public bool Sell(ItemSO itemToSell)
    {
        int amountMoney = itemToSell.priceToSell;

        if (!PlayerInventory.Instance.RemoveItem(itemToSell.ID, 1))
            return false;

        // Item sold -> gained money
        GameManager.Instance.GetPlayerStats()?.AddMoney(amountMoney);

        return true;
    }

    public void SetCanInteract(bool canInteract)
    {
        this.canInteract = canInteract;
    }
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