using System.Collections.Generic;
using UnityEngine;

public enum UIId
{
    Inventory,
    Map,
    BuyShop,
    SellShop,
    Dialog
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [System.Serializable]
    public struct UIEntry
    {
        public UIId id;
        public BasedUITK ui;
    }

    [SerializeField] private List<UIEntry> uiEntries;

    private Dictionary<UIId, BasedUITK> uiMap;

    private void Awake()
    {
        Instance = this;
        uiMap = new Dictionary<UIId, BasedUITK>();
        foreach (var entry in uiEntries)
            uiMap[entry.id] = entry.ui;
    }

    public void Open(UIId id)
    {
        if (uiMap.TryGetValue(id, out var ui))
            ui.Open();
        else
            Debug.LogWarning($"UIManager: aucune UI enregistrée pour {id}");
    }

    public void Close(UIId id)
    {
        if (uiMap.TryGetValue(id, out var ui))
            ui.Close();
    }

    public void Toggle(UIId id)
    {
        if (uiMap.TryGetValue(id, out var ui))
            ui.ToggleMenu();
    }
}