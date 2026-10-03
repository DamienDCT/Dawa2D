using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SellShopUITK : BasedShopUITK
{
    public static SellShopUITK Instance;
    private VisualElement selectedVisualElement;
    private ItemSO selectedItemToBuy;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected override void ShowItemsInside()
    {
        if (currentShop == null)
        {
            Debug.LogWarning("current shop est null");
            return;
        }

        if(shopItemsContainer == null)
        {
            Debug.LogWarning("shop container est null");
            return;
        }

        var container = shopItemsContainer; // capture locale
        List<SoldableItem> itemsToSell = PlayerInventory.Instance.GetSoldableItems();

        container.schedule.Execute(async () =>
        {
            // Le container a été détruit / remplacé entre-temps (reload du UI)
            if (container.panel == null || container != shopItemsContainer)
                return;

            container.Clear();
            selectedVisualElement = null;
            selectedItemToBuy = null;

            var elements = new List<VisualElement>();

            foreach (SoldableItem soldableItem in itemsToSell)
            {
                var itemInstance = _shopItemTemplate.Instantiate();
                itemInstance.focusable = true;
                itemInstance.tabIndex = 0;

                ItemSO currentItemToSell = soldableItem.item;
                int quantityAvailable = soldableItem.quantity;

                // Setup the line
                var icon = itemInstance.Q<Image>("ItemIcon");
                if (icon != null) icon.sprite = currentItemToSell.sprite;

                Label itemName = itemInstance.Q<Label>("ItemName");
                if (itemName != null)
                {
                    string localizationName = await LocalizationManager.Instance.GetTranslatedText(soldableItem.item.itemNameLocalization, itemTableLocalizationName);

                    itemName.text = localizationName;
                }

                var priceLabel = itemInstance.Q<Label>("ItemCost");
                if (priceLabel != null) priceLabel.text = currentItemToSell.priceToSell.ToString();

                var quantityLabel = itemInstance.Q<Label>("QuantityAvailableLabel");
                if (quantityLabel != null) quantityLabel.text = "x" + quantityAvailable; 

                // Registering the events of the items
                itemInstance.RegisterCallback<FocusOutEvent>(_ => itemInstance.RemoveFromClassList("selected"));
                itemInstance.RegisterCallback<FocusInEvent>(_ => FocusInShopElement(itemInstance, currentItemToSell));
                itemInstance.RegisterCallback<NavigationSubmitEvent>(_ => SellItem(currentItemToSell, itemInstance));

                container.Add(itemInstance);
                elements.Add(itemInstance);
            }

            if (elements.Count == 0)
                return;

            selectedVisualElement = elements[0];
            selectedItemToBuy = itemsToSell[0].item;

            FocusFirstWhenReady(elements[0]);
        }).ExecuteLater(0);
    }

    // Make the player buying the item
    private void SellItem(ItemSO item, VisualElement row)
    {
        if (currentShop == null || !currentShop.Sell(item))
            return;

        UpdateMoney();

        int remaining = PlayerInventory.Instance.GetQuantity(item.ID);

        if (remaining > 0)
        {
            row.Q<Label>("QuantityAvailableLabel").text = "x" + remaining;
            return;
        }

        // Plus d'exemplaires : on déplace le focus avant de retirer la ligne
        var parent = row.parent;
        int index = parent.IndexOf(row);
        VisualElement next = parent.childCount > 1
            ? parent[index < parent.childCount - 1 ? index + 1 : index - 1]
            : null;

        next?.Focus();
        row.RemoveFromHierarchy();
    }

    private void FocusInShopElement(VisualElement element, ItemSO shopItem)
    {
        selectedVisualElement = element;
        selectedItemToBuy = shopItem;

        element.AddToClassList("selected");
    }

    protected override void OnMenuClosed()
    {
        currentShop.SetCanInteract(true);
        currentShop = null;
    }

    protected override void SetupName()
    {
        if (currentShop == null)
            return;

        if (shopNameLabel != null)
            shopNameLabel.text = "Vendre à " + currentShop.GetShopName();
    }

    //public void SetShop(Shop shop)
    //{
    //    this.currentShop = shop;

    //    if (!IsMenuOpened)
    //    {
    //        ShowItems();
    //        SetupName();
    //    }

    //    //base.ToggleMenu();
    //}

}
