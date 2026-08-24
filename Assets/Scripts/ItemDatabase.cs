using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Databases/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemSO> allItems;

    public void Init()
    {
        Instance = this;
    }

    public ItemSO GetItemByID(int id)
    {
        return allItems.Find(item => item.ID == id);
    }
}