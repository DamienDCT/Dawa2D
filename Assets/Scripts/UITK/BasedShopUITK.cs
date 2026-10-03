using UnityEngine;
using UnityEngine.UIElements;

public abstract class BasedShopUITK : BasedUITK
{
    // Template of one shop item
    [SerializeField] protected VisualTreeAsset _shopItemTemplate;
    // Panel renderer to register events
    [SerializeField] protected PanelRenderer panelRenderer;
    // itemTableLocalization to translate
    [SerializeField] protected string itemTableLocalizationName;

    // Visual Elements
    protected VisualElement shopItemsContainer;
    protected VisualElement root;
    protected Label shopNameLabel;
    protected Label moneyLabel;
    protected Label itemDescriptionLabel;

    protected int _uiVersion = -1;

    protected MoneyLabelAnimator moneyLabelAnimator = new MoneyLabelAnimator();


    protected Shop currentShop;

    protected virtual void Awake()
    {
        panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
        moneyLabelAnimator = new MoneyLabelAnimator();
    }

    protected virtual void OnDestroy()
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

        // Initialization of the description part
        itemDescriptionLabel = root.Q<Label>("DescriptionLabel");

        // On bind l'animator sur le label
        moneyLabelAnimator.Bind(moneyLabel);
        // On update l'argent avec "false" -> sans animation
        //UpdateMoney(false);

        ShowItemsInside();
    }

    protected abstract void ShowItemsInside();

    protected void UpdateMoney(bool animate = true)
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

    protected void FocusFirstWhenReady(VisualElement element)
    {
        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            element.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            element.Focus();

            var focused = element.panel?.focusController?.focusedElement;
        }

        element.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    protected void UpdateQuantity(VisualElement element, ShopItem shopItem)
    {
        if (currentShop == null)
            return;

        int quantity = currentShop.GetQuantityRemaining(shopItem);

        Label qtyLabel = element.Q<Label>("QuantityAvailableLabel");
        qtyLabel.text = quantity <= 0 ? "Épuisé" : "x" + quantity;



    }

    protected abstract void SetupName();

    protected int GetPlayerMoney()
    {
        return GameManager.Instance.GetPlayerStats().Stats.CurrentMoney;
    }

    public void SetShop(Shop shop)
    {
        this.currentShop = shop;

        if (!IsMenuOpened)
        {
            ShowItemsInside();
            SetupName();
        }
    }
}
