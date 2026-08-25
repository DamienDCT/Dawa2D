using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

public static class InventorySaveManager
{
    private static string SavePath => Application.persistentDataPath + "/inventory.json";

    public static void Save(PlayerInventory inventory)
    {
        InventorySaveData data = new InventorySaveData();

        foreach (var item in inventory.GetStoredItems())
        {
            data.itemIDs.Add(item);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Inventaire sauvegardé : " + SavePath);
    }

    public static void Load(PlayerInventory inventory)
    {
        Debug.Log("coucou");
        if (!File.Exists(SavePath))
        {
            Debug.Log("Aucune sauvegarde trouvée.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        inventory.ClearInventory();

        foreach (var item in data.itemIDs)
        {
           //ItemSO item = ItemDatabase.Instance.GetItemByID(item.ID);
           
            inventory.AddItem(item.itemID, item.quantity);
        }

        Debug.Log("Inventaire chargé !");
    }
}

[Serializable]
public class InventorySaveData
{
    public List<SlotUI> itemIDs = new List<SlotUI>();
}