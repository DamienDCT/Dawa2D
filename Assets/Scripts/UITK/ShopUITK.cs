using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ShopUITK : BasedShopUITK
{
    public static ShopUITK Instance;

 


    // Selected item variables
    private ShopItem selectedItemToBuy;
    private VisualElement selectedVisualElement;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }



    //private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    //{
    //    // Si la version est identique, on évite de reload les valeurs
    //    if (_uiVersion == version)
    //        return;

    //    // On màj la version du UI
    //    _uiVersion = version;

    //    root = rootElement;
    //    menuElement = root.Q<VisualElement>("Panel");

    //    // Initialization of the container of shop's items
    //    shopItemsContainer = root.Q<VisualElement>("ShopItemList");

    //    // Setup le nom du shop
    //    shopNameLabel = root.Q<Label>("ShopNameLabel");

    //    // Initialization of the money label
    //    moneyLabel = root.Q<Label>("MoneyLabel");

    //    // Initialization of the description part
    //    itemDescriptionLabel = root.Q<Label>("DescriptionLabel");

    //    // On bind l'animator sur le label
    //    moneyLabelAnimator.Bind(moneyLabel);
    //    // On update l'argent avec "false" -> sans animation
    //    UpdateMoney(false);

    //    ShowShopItems();
    //}

    //private int GetPlayerMoney()
    //{
    //    return GameManager.Instance.GetPlayerStats().Stats.CurrentMoney;
    //}

    protected override async void ShowItemsInside()
    {
        if (currentShop == null || shopItemsContainer == null)
            return;

        List<ShopItem> items = currentShop.GetListItemShop;

        List<VisualElement> elements = new List<VisualElement>();

        // TODO : IMPORTANT /!\ : Faire en sorte de pas mettre les unique items déjà achetés dans le shop
        // Ou alors : les affichés comme acheté (peut être comme ça c'est mieux)

        // On diffère la reconstruction pour ne jamais la faire
        // pendant un dispatch d'event / repaint en cours
        shopItemsContainer.schedule.Execute(async () =>
        {
            shopItemsContainer.Clear();

            int currentInventoryItem = 0;

            foreach (ShopItem shopItem in items)
            {
                var itemInstance = _shopItemTemplate.Instantiate();

                itemInstance.focusable = true;
                itemInstance.tabIndex = 0;

                int quantityRemaining = shopItem.quantityRemaining;

                Image itemIcon = itemInstance.Q<Image>("ItemIcon");
                if (itemIcon != null)
                    itemIcon.sprite = shopItem.soldItem.sprite;

                Label itemName = itemInstance.Q<Label>("ItemName");
                if (itemName != null)
                {
                    string localizationName = await LocalizationManager.Instance.GetTranslatedText(shopItem.soldItem.itemNameLocalization, itemTableLocalizationName);

                    itemName.text = localizationName;
                }

                Label itemPrice = itemInstance.Q<Label>("ItemCost");
                if (itemPrice != null)
                    itemPrice.text = shopItem.price.ToString();

                Label quantityLabel = itemInstance.Q<Label>("QuantityAvailableLabel");
                if (quantityLabel != null) 
                    quantityLabel.text = "x" + quantityRemaining;

                shopItemsContainer.Add(itemInstance);

                // Add elements focus & navigation events
                itemInstance.RegisterCallback<FocusOutEvent>((evt) => itemInstance.RemoveFromClassList("selected"));
                itemInstance.RegisterCallback<FocusInEvent>((evt) => FocusInShopElement(itemInstance, shopItem));

                itemInstance.RegisterCallback<NavigationSubmitEvent>((evt) => BuyItem(itemInstance, shopItem));

                elements.Add(itemInstance);

                currentInventoryItem++;
            }

            selectedVisualElement = elements[0];
            selectedItemToBuy = items[0];

            // On attend que le layout du premier élément soit réellement calculé
            // avant de lui donner le focus (Focus() peut échouer silencieusement
            // si canGrabFocus() est encore false).
            FocusFirstWhenReady(elements[0]);


        }).ExecuteLater(0);


    }

    // Make the player buying the item
    private void BuyItem(VisualElement element, ShopItem item)
    {
        if (currentShop == null)
            return;

        if(currentShop.Buy(item))
        {
            Debug.Log($"We bought {item.soldItem.itemNameLocalization}");
            UpdateMoney();
            UpdateQuantity(element, item);
        } else
        {
            Debug.LogWarning($"We couldn't buy this item { item.soldItem.itemNameLocalization}");
        }

    }

    private async void FocusInShopElement(VisualElement element, ShopItem shopItem)
    {
        selectedVisualElement = element;
        selectedItemToBuy = shopItem;

        element.AddToClassList("selected");

        string translatedText = await LocalizationManager.Instance.GetTranslatedText(shopItem.itemLocalizationShopDescription, itemTableLocalizationName);
        itemDescriptionLabel.text = translatedText;
    }

    protected override void SetupName()
    {
        if (currentShop == null)
            return;

        if (shopNameLabel != null)
            shopNameLabel.text = currentShop.GetShopName();
    }

    protected override void OnMenuClosed()
    {
        currentShop.SetCanInteract(true);
        currentShop = null;
    }
}
