using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using Unity.VisualScripting;
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

    // Button swap items / spells
    private Button itemButton;
    private Button spellButton;

    // Visual elements selected spells / all items
    private VisualElement selectedSpellsContainer;
    private VisualElement specialItemsContainer;

    private VisualElement[] selectedSlots;

    // Anchors for the spells slots
    private static readonly (string name, Vector2 anchor)[] Slots = {
        ("slot-top",    new Vector2(47f, 11f)),  // haut
        ("slot-right",  new Vector2(82f, 47f)),  // droite
        ("slot-bottom", new Vector2(47f, 78f)),  // bas
        ("slot-left",   new Vector2(11f, 47f)),  // gauche
    };

    // Spells selections
    // Suivi du mode courant (items ou spells)
    private bool _isSpellModeActive = false;

    // Sélection en cours pour l'équipement
    private SlotUI? _spellToEquip = null;
    private VisualElement _selectedInventorySlot = null;

    [SerializeField] private Scale slotImageSize;
    [SerializeField] private Scale slotDotSize;

    private VisualElement[] _ringSlotElements = new VisualElement[4];

    private float lastNavigationTime = 0.0f;
    private const float NAVIGATION_DELAY = 0.2f;

    private void Reset()
    {
        slotImageSize = new Scale(new Vector2(2.5f, 2.5f));
        slotDotSize = new Scale(Vector2.one);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        IsInventoryShown = !IsInventoryShown;
        if (IsInventoryShown)
        {
            ShowItems(null);
            inventoryElement?.RemoveFromClassList("hidden");
        }
        else
        {
            inventoryElement?.AddToClassList("hidden");
        }

    }

    private void OnEnable()
    {
        panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);

        // Set the values of the scale
        slotImageSize = new Scale(new Vector2(2.5f, 2.5f));
        slotDotSize = new Scale(Vector2.one);
        selectedSlots = new VisualElement[4];
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
    {
        root = rootElement;
        inventoryElement = root.Q<VisualElement>("Panel");
        // Ton initialisation d'UI ici
        inventoryItemsContainer = root.Q<VisualElement>("InventoryContainer");

        // On initialise les variables du detail d'item à droite du menu
        itemNameLabel = root.Q<Label>("ItemTitle");
        itemDescLabel = root.Q<Label>("ItemDescription");
        itemSprite = root.Q<Image>("ItemSprite");

        // On initialise les variables des boutons du menu à gauche
        itemButton = root.Q<Button>("ItemButton");
        spellButton = root.Q<Button>("SpellButton");
        // On initialise les évents onclick
        itemButton.RegisterCallback<ClickEvent>(ShowItems);
        spellButton.RegisterCallback<ClickEvent>(ShowSpells);

        // On initialise les deux parties des spells uniques/objets uniques
        specialItemsContainer = root.Q<VisualElement>("UniqueItemsInventory");
        selectedSpellsContainer = root.Q<VisualElement>("SpellInventory");

        ShowItems(null);

    }

    private void ShowSpells(ClickEvent evt)
    {
        _isSpellModeActive = true;
        selectedSpellsContainer.style.display = DisplayStyle.Flex;
        specialItemsContainer.style.display = DisplayStyle.None;

        RefreshSelectedSpells();
        RefreshInventory(GetSpellsInInventory());
    }

    private void ShowItems(ClickEvent evt)
    {
        _isSpellModeActive = false;
        selectedSpellsContainer.style.display = DisplayStyle.None;
        specialItemsContainer.style.display = DisplayStyle.Flex;
        RefreshInventory(GetItemsInInventory());

    }


    private IReadOnlyList<SlotUI> GetItemsInInventory()
    {
        return PlayerInventory.Instance.GetStoredItems();
    }

    private IReadOnlyList<SlotUI> GetSpellsInInventory()
    {
        return PlayerInventory.Instance.GetStoredSpells();
    }

    private void RefreshSelectedSpells()
    {
        var ringContainer = selectedSpellsContainer.Q<Image>("BackgroundSpellImage");
        // Permet de voir l'overflow en dessous
        ringContainer.style.overflow = Overflow.Visible;

        ringContainer.Clear();
        int i = 0;
        SlotUI?[] selectedSpells = PlayerInventory.Instance.GetSelectedSpells();

       // VisualElement[] createdSlots = new VisualElement[4];

        foreach (var (name, anchor) in Slots)
        {
            var instance = _slotTemplateAsset.Instantiate();
            var slot = instance[0];

            instance.Q<Label>("QuantityLabel").text = String.Empty;
            slot.style.position = Position.Absolute;

            float clickZoneSize = 64f;
            // Setup visuels des slots
            slot.style.width = clickZoneSize;
            slot.style.height = clickZoneSize;
            slot.style.flexGrow = 0;
            slot.style.flexShrink = 0;
            slot.style.justifyContent = Justify.Center;
            slot.style.alignItems = Align.Center;
            slot.style.left = new Length(anchor.x, LengthUnit.Percent);
            slot.style.top = new Length(anchor.y, LengthUnit.Percent);
            slot.style.translate = new Translate(
                new Length(-50f, LengthUnit.Percent),
                new Length(-50f, LengthUnit.Percent));

            // On setup le nom pour les récupérer derrière
            string slotIndex = i.ToString();
            slot.name = slotIndex;


            // Navigation manette/clavier
            slot.focusable = true;
            slot.tabIndex = 0;

            slot.pickingMode = PickingMode.Position;

            int capturedIndex = i; // copie locale propre à cette itération


            // Souris -> focus (curseur unifié)
            slot.RegisterCallback<PointerEnterEvent>(evt => slot.Focus());
            slot.RegisterCallback<FocusInEvent>(evt =>
            {
                slot.AddToClassList("slot-focused");
                Debug.Log("we focus");
            });
            slot.RegisterCallback<FocusOutEvent>(evt =>
            {
                slot.RemoveFromClassList("slot-focused");
                Debug.Log("we stop focusing ");
            });

            // Activation : clic souris OU Submit manette/clavier
            slot.RegisterCallback<ClickEvent>(evt => OnRingSlotClicked(slotIndex, slot));
            slot.RegisterCallback<NavigationSubmitEvent>(evt => OnRingSlotClicked(slotIndex, slot));

            slot.RegisterCallback<NavigationMoveEvent>(evt => NavigateSelectedSpells(capturedIndex, slot, evt));

            //_ringSlotElements[i - 1] = slot;
            Image icon = instance.Q<Image>("SlotDotImage");
            ApplyRingSlotVisual(selectedSpells[capturedIndex], icon);

            _ringSlotElements[capturedIndex] = slot;

            selectedSlots[capturedIndex] = slot;
            ringContainer.Add(slot);
            i++;

        }

    }

    // Mapping direction -> index du slot ciblé (voir l'ordre dans Slots : 0=haut, 1=droite, 2=bas, 3=gauche)
    private static readonly Dictionary<NavigationMoveEvent.Direction, int> DirectionToSlotIndex = new()
    {
        { NavigationMoveEvent.Direction.Up,    0 }, // slot-top
        { NavigationMoveEvent.Direction.Right, 1 }, // slot-right
        { NavigationMoveEvent.Direction.Down,  2 }, // slot-bottom
        { NavigationMoveEvent.Direction.Left,  3 }, // slot-left
    };

    private void NavigateSelectedSpells(int index, VisualElement slot, NavigationMoveEvent evt)
    {
        // Anti-spam navigation (joystick maintenu, etc.)
        if (lastNavigationTime + NAVIGATION_DELAY >= Time.time)
            return;

        // Direction non gérée (None, ou diagonale si jamais ça existe) -> on laisse faire
        if (!DirectionToSlotIndex.TryGetValue(evt.direction, out int targetIndex))
            return;

        // On est déjà sur le slot correspondant à cette direction -> on ignore complètement
        // (empêche par ex. "gauche" depuis le slot gauche de faire quoi que ce soit)
        if (targetIndex == index)
            return;

        lastNavigationTime = Time.time;

        selectedSlots[targetIndex].Focus();

        evt.StopPropagation();
        slot.panel.focusController.IgnoreEvent(evt); // remplace PreventDefault()
    }

    private void ApplyRingSlotVisual(SlotUI? slot, Image icon)
    {
        if(slot != null)
        {
            icon.sprite = slot.Value.itemSprite;
            icon.style.scale = slotImageSize;
        }
    }

    private void OnRingSlotClicked(string slotName, VisualElement slotElement)
    {
        // Aucun spell sélectionné en bas -> on ne fait rien
        if (_spellToEquip == null)
            return;

        Debug.Log(slotElement.name);



       // _equippedSpells[slotName] = _spellToEquip;

        // Mise à jour visuelle du slot du ring
        Image icon = slotElement.Q<Image>("SlotDotImage");
        icon.style.scale = slotImageSize;
        if (icon != null)
            icon.sprite = _spellToEquip.Value.itemSprite;

        // TODO: équiper réellement le spell côté gameplay 

         PlayerInventory.Instance.EquipSpell(slotName, _spellToEquip.Value);
        RefreshSelectedSpells();

        // Reset de la sélection
        _selectedInventorySlot?.RemoveFromClassList("slot-selected");
        _selectedInventorySlot = null;
        _spellToEquip = null;
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
        if (item != null)
        {
            slotInstance = _slotTemplateAsset.Instantiate();
            Image icon = slotInstance.Q<Image>("SlotDotImage");
         //   slotInstance.Q<Image>("SlotDotImage").enabledSelf = false;
            Label quantityLabel = slotInstance.Q<Label>("QuantityLabel");

            icon.sprite = item?.itemSprite;
            quantityLabel.text = item?.quantity > 1 ? item?.quantity.ToString() : "";
            icon.style.scale = slotImageSize;

            slotInstance.focusable = true;
            slotInstance.tabIndex = 0;

            // Souris : hover -> focus (unifie souris et navigation clavier/manette)
            slotInstance.RegisterCallback<PointerEnterEvent>(evt => slotInstance.Focus());

            // Curseur visuel (souris OU clavier/manette, peu importe la source)
            slotInstance.RegisterCallback<FocusInEvent>(evt =>
            {
                UpdateDetailPanel(item);
                slotInstance.AddToClassList("slot-focused");
            });
            slotInstance.RegisterCallback<FocusOutEvent>(evt =>
                slotInstance.RemoveFromClassList("slot-focused"));

            // Activation : clic souris OU bouton Submit manette/clavier
            slotInstance.RegisterCallback<ClickEvent>(evt => ActivateInventorySlot(item, slotInstance));
            slotInstance.RegisterCallback<NavigationSubmitEvent>(evt => ActivateInventorySlot(item, slotInstance));

            container.Add(slotInstance);
        }
        else
        {
            slotInstance = _slotTemplateAsset.Instantiate();
        }

        if (slotInstance != null)
            container.Add(slotInstance);
    }

    private void ActivateInventorySlot(SlotUI? item, VisualElement slotElement)
    {
        if (!_isSpellModeActive || item == null)
            return;

      //  ClearRingSelection();

        _selectedInventorySlot?.RemoveFromClassList("slot-selected");
        _spellToEquip = item;
        _selectedInventorySlot = slotElement;
        slotElement.AddToClassList("slot-selected");

        // On envoie le focus sur un slot du ring pour enchaîner la navigation manette
        //VisualElement targetRingSlot = _selectedRingIndex.HasValue
        //    ? _ringSlotElements[_selectedRingIndex.Value]
        //    : _ringSlotElements[0];

        // Force le curseur à sauter dans l'anneau de sorts en ciblant le slot du bas
        if (_ringSlotElements[2] != null)
        {
            _ringSlotElements[2].Focus();
        }

        //targetRingSlot?.Focus();
    }

    private void OnInventorySlotClicked(SlotUI? item, VisualElement slotElement)
    {
        // On ne gère la sélection que si on est en mode "spells" et qu'il y a bien un item
        if (!_isSpellModeActive || item == null)
            return;

        // On retire la classe "sélectionné" de l'ancien slot choisi
        _selectedInventorySlot?.RemoveFromClassList("slot-selected");

        _spellToEquip = item;
        _selectedInventorySlot = slotElement;
        slotElement.AddToClassList("slot-selected"); // à définir dans ton USS pour un feedback visuel
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
