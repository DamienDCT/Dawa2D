using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private List<ShopItem> itemShopList;

    public List<ShopItem> GetListItemShop => itemShopList;

    public bool CanInteract => true;


    // Si on a besoin de dialogues
    [SerializeField] private bool hasDialogueBeforeShopOpens;
    [SerializeField] private DialogNode startDialogNode;


    public void Interact()
    {
        Debug.Log("interact");
        ShopUITK.Instance.SetShop(this);
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