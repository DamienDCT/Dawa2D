using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InventoryUITK : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _slotTemplateAsset;

    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private string itemTableLocalizationName;

    private VisualElement inventoryItemsContainer;
    private VisualElement root;
    private VisualElement inventoryElement;

    private bool IsInventoryShown = false;

    // Detail item/spell description
    private Label itemNameLabel;
    private Label itemDescLabel;
    private Image itemSprite;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            Debug.Log("we press tab");
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        IsInventoryShown = !IsInventoryShown;
        if (IsInventoryShown)
        {
            Debug.Log("we remove class");
            RefreshInventory(GetInventoryData());
            inventoryElement?.RemoveFromClassList("hidden");
        }
        else
        {
            Debug.Log("we add class");
            inventoryElement?.AddToClassList("hidden");
        }

    }

    private void OnEnable()
    {
        panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);

    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
    {
        root = rootElement;
        inventoryElement = root.Q<VisualElement>("Panel");
        // Ton initialisation d'UI ici
        var monBouton = root.Q<Button>("MonBouton");
        inventoryItemsContainer = root.Q<VisualElement>("InventoryContainer");

        // On initialise les variables du detail d'item à droite de la 
        itemNameLabel = root.Q<Label>("ItemTitle");
        itemDescLabel = root.Q<Label>("ItemDescription");
        itemSprite = root.Q<Image>("ItemSprite");

        RefreshInventory(GetInventoryData());
    }

    private IReadOnlyList<SlotUI> GetInventoryData()
    {
        return PlayerInventory.Instance.GetStoredItems();
    }

    private void RefreshInventory(IReadOnlyList<SlotUI> items)
    {
        inventoryItemsContainer.Clear();

        foreach (SlotUI item in items)
        {
            AddSlotToInventoryItem(item, inventoryItemsContainer);
        }
        for(int i = items.Count; i < PlayerInventory.INVENTORY_SIZE; i++)
        {
            AddSlotToInventoryItem(null, inventoryItemsContainer);
        }

    }

    private void AddSlotToInventoryItem(SlotUI? item, VisualElement container)
    {
        if (container == null)
            return;

        TemplateContainer slotInstance = null;
        if(item != null)
        {
            slotInstance = _slotTemplateAsset.Instantiate();
           // 2.Récupération des sous-éléments
            Image icon = slotInstance.Q<Image>("ItemIconImage");
            slotInstance.Q<Image>("SlotDotImage").enabledSelf = false;
            Label quantityLabel = slotInstance.Q<Label>("QuantityLabel");

            

            // 3. Injection des données
            if (item != null)
            {
                icon.sprite = item?.itemSprite;
                quantityLabel.text = item?.quantity > 1 ? item?.quantity.ToString() : string.Empty;
            }
            else
            {
                // Slot vide
                icon.style.backgroundImage = null;
                quantityLabel.text = string.Empty;
            }

            // 4. Register events pointer handler
            // --- Rendre le slot navigable/hoverable ---
            slotInstance.focusable = true;
            slotInstance.tabIndex = 0;

            // Souris : le hover déclenche le focus (unifie les deux inputs)
            slotInstance.RegisterCallback<PointerEnterEvent>(evt => slotInstance.Focus());

            // Focus (souris via hover OU clavier/gamepad via navigation) : mise à jour du détail
            slotInstance.RegisterCallback<FocusInEvent>(evt => UpdateDetailPanel(item));

            // 5. Ajout dans le conteneur principal
            container.Add(slotInstance);
        }
        else
        {
            slotInstance = _slotTemplateAsset.Instantiate();
        }

        if (slotInstance != null)
            container.Add(slotInstance);
    }

    private async void UpdateDetailPanel(SlotUI? item)
    {
        if (itemSprite == null || itemNameLabel == null || itemDescLabel == null)
            return;

        if (item == null)
        {
            itemSprite.sprite = null;
            itemNameLabel.text = string.Empty;
            itemDescLabel.text = string.Empty;
            return;
        }

        SlotUI value = item.Value;
        ItemSO itemData = GameDatabases.Instance.GetItemDatabase().GetItemByID(value.itemID);

        // On traduit dans la bonne langue le nom et la description de l'item
        string title = await LocalizationManager.Instance.GetTranslatedText(itemData.itemNameLocalization, itemTableLocalizationName);
        string description = await LocalizationManager.Instance.GetTranslatedText(itemData.descriptionLocalization, itemTableLocalizationName);


        itemSprite.sprite = value.itemSprite;
        itemNameLabel.text = title;  // adapte au nom réel du champ dans ton ItemSO
        itemDescLabel.text = description; // idem
    }

}
