using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Mono.Cecil.Cil;

public class ShopUITK : BasedUITK
{
    public static ShopUITK Instance;

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

    private int _uiVersion = -1;

    private MoneyLabelAnimator moneyLabelAnimator = new MoneyLabelAnimator();


    private Shop currentShop;


    // Selected item variables
    private ShopItem selectedItemToBuy;
    private VisualElement selectedVisualElement;

    private void Awake()
    {
        panelRenderer = GetComponent<PanelRenderer>();
        Instance = this;
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDestroy()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
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

        // Setup le nom du shop
        shopNameLabel = root.Q<Label>("ShopNameLabel");

        // Initialization of the money label
        moneyLabel = root.Q<Label>("MoneyLabel");

        // On bind l'animator sur le label
        moneyLabelAnimator.Bind(moneyLabel);
        // On update l'argent avec "false" -> sans animation
        UpdateMoney(false);

        ShowShopItems();
    }

    private void UpdateMoney(bool animate = true)
    {
        if (moneyLabel == null)
            return;

        // Selon si on demande une animation, on fait une animation ou non.
        if(animate)
        {
            moneyLabelAnimator.AnimateTo(GetPlayerMoney());
        } else
        {
            moneyLabelAnimator.SetImmediate(GetPlayerMoney());
        }
    }

    private int GetPlayerMoney()
    {
        return GameManager.Instance.GetPlayerStats().Stats.CurrentMoney;
    }

    private void ShowShopItems()
    {
        if (currentShop == null || shopItemsContainer == null)
            return;

        List<ShopItem> items = currentShop.GetListItemShop;

        List<VisualElement> elements = new List<VisualElement>();

        // TODO : IMPORTANT /!\ : Faire en sorte de pas mettre les unique items déjà achetés dans le shop
        // Ou alors : les affichés comme acheté (peut être comme ça c'est mieux)

        // On diffère la reconstruction pour ne jamais la faire
        // pendant un dispatch d'event / repaint en cours
        shopItemsContainer.schedule.Execute(() =>
        {
            shopItemsContainer.Clear();

            int currentInventoryItem = 0;

            foreach (ShopItem shopItem in items)
            {
                var itemInstance = _shopItemTemplate.Instantiate();

                itemInstance.focusable = true;
                itemInstance.tabIndex = 0;

                Image itemIcon = itemInstance.Q<Image>("ItemIcon");
                if (itemIcon != null)
                    itemIcon.sprite = shopItem.soldItem.sprite;

                Label itemName = itemInstance.Q<Label>("ItemLabel");
                if (itemName != null)
                    itemName.text = shopItem.soldItem.name;

                Label itemPrice = itemInstance.Q<Label>("ItemCost");
                if (itemPrice != null)
                    itemPrice.text = shopItem.price.ToString();

                shopItemsContainer.Add(itemInstance);

                // Add elements focus & navigation events
                itemInstance.RegisterCallback<FocusOutEvent>((evt) => itemInstance.RemoveFromClassList("selected"));
                itemInstance.RegisterCallback<FocusInEvent>((evt) => FocusInShopElement(itemInstance, shopItem));

                itemInstance.RegisterCallback<NavigationSubmitEvent>((evt) => BuyItem(shopItem));

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
    private void BuyItem(ShopItem item)
    {
        if (currentShop == null)
            return;

        if(currentShop.Buy(item))
        {
            Debug.Log($"We bought {item.soldItem.itemNameLocalization}");
            UpdateMoney();
        } else
        {
            Debug.LogWarning($"We couldn't buy this item { item.soldItem.itemNameLocalization}");
        }

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

    private void FocusInShopElement(VisualElement element, ShopItem shopItem)
    {
        selectedVisualElement = element;
        selectedItemToBuy = shopItem;

        element.AddToClassList("selected");
    }

    private void SetupName()
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

    public void SetShop(Shop shop)
    {
        this.currentShop = shop;

        if (!IsMenuOpened)
        {
            ShowShopItems();
            SetupName();
        }

        //base.ToggleMenu();
    }
}
