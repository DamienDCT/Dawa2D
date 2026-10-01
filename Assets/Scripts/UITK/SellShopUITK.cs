using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SellShopUITK : BasedUITK
{
    public static SellShopUITK Instance;

    // Template of one shop item
    [SerializeField] private VisualTreeAsset _shopItemTemplate;
    // Panel renderer to register events
    [SerializeField] private PanelRenderer panelRenderer;
    // itemTableLocalization to translate
    [SerializeField] private string itemTableLocalizationName;

    // Visual Elements
    private VisualElement shopItemsContainer;
    private VisualElement root;
    private Label shopNameLabel;
    private Label moneyLabel;

    private MoneyLabelAnimator moneyLabelAnimator;

    private int _uiVersion = -1;

    private VisualElement selectedVisualElement;
    private ItemSO selectedItemToBuy;

    private Shop currentShop;

    private void Awake()
    {
        panelRenderer = GetComponent<PanelRenderer>();
        Instance = this;
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
        moneyLabelAnimator = new MoneyLabelAnimator();
    }

    private void OnDestroy()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        // Avoiding reload if the UI is the same 
        if (_uiVersion == version)
            return;

        // We update the ui version
        _uiVersion = version;

        root = rootElement;
        menuElement = root.Q<VisualElement>("Panel");

        // Initialization of the container of shop's items
        shopItemsContainer = root.Q<VisualElement>("ShopItemList");

        // Initialization of the name's shop label
        shopNameLabel = root.Q<Label>("ShopNameLabel");

        // Initialization of the money label
        moneyLabel = root.Q<Label>("MoneyLabel");

        // On bind l'animator sur le label
        moneyLabelAnimator.Bind(moneyLabel);
        // On update l'argent avec "false" -> sans animation
        UpdateMoney(false);

        ShowItems();
    }

    private void UpdateMoney(bool animate = true)
    {
        if (moneyLabel == null)
            return;

        // Selon si on demande une animation, on fait une animation ou non.
        if (animate)
        {
            moneyLabelAnimator.AnimateTo(GetPlayerMoney());
        }
        else
        {
            moneyLabelAnimator.SetImmediate(GetPlayerMoney());
        }
    }

    private int GetPlayerMoney()
    {
        return GameManager.Instance.GetPlayerStats().Stats.CurrentMoney;
    }

    private void ShowItems()
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

        container.schedule.Execute(() =>
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

                var nameLabel = itemInstance.Q<Label>("ItemLabel");
                if (nameLabel != null) nameLabel.text = currentItemToSell.name;

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

    private void FocusFirstWhenReady(VisualElement element)
    {
        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            element.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            element.Focus();

            var focused = element.panel?.focusController?.focusedElement;
        }

        element.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
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

    private void SetupName()
    {
        if (currentShop == null)
            return;

        if (shopNameLabel != null)
            shopNameLabel.text = "Vendre à " + currentShop.GetShopName();
    }

    public void SetShop(Shop shop)
    {
        this.currentShop = shop;

        if (!IsMenuOpened)
        {
            ShowItems();
            SetupName();
        }

        //base.ToggleMenu();
    }

}
