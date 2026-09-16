using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ShopUITK : BasedUITK
{
    // Template of one shop item
    [SerializeField] private VisualTreeAsset _shopItemTemplate;
    // Panel renderer to register events
    [SerializeField] private PanelRenderer panelRenderer;
    // itemTableLocalization to translate
    [SerializeField] private string itemTableLocalizationName;

    // Visual Elements
    private VisualElement shopItemsContainer;


    private VisualElement root;

    private int _uiVersion = -1;

    private Shop currentShop;

    private void OnEnable()
    {
        panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);


    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        // Si la version est identique, on évite de reload les valeurs
        if (_uiVersion == version)
            return;

        // On màj la version du UI
        _uiVersion = version;

        root = rootElement;
        menuElement = root.Q<VisualElement>("Panel");

        // Initialization of the container of shop's items
        shopItemsContainer = root.Q<VisualElement>("ShopItemList");

        ShowShopItems();
    }

    private void ShowShopItems()
    {
        shopItemsContainer.Clear();

        List<ShopItem> items = currentShop.GetListItemShop;

        TemplateContainer itemInstance = null;
        foreach (ShopItem shopItem in items)
        {
            itemInstance = _shopItemTemplate.Instantiate();

            // On associe le sprite à la bonne ligne
            Image itemIcon = itemInstance.Q<Image>("ItemIcon");
            if (itemIcon != null)
                itemIcon.sprite = shopItem.soldItem.sprite;

            // On associe le bon nom d'item | TODO : Faire avec la localization
            Label itemName = itemInstance.Q<Label>("ItemLabel");
            if(itemName != null)
                itemName.text = shopItem.soldItem.name;

            // On associe le bon prix de l'item
            Label itemPrice = itemInstance.Q<Label>("ItemCost");
            if (itemPrice != null)
                itemPrice.text = shopItem.price.ToString();

            // TODO : Hover effect -> apply description

            shopItemsContainer.Add(itemInstance);
        }
    }

    protected override void OnMenuOpened()
    {
        
    }
}
