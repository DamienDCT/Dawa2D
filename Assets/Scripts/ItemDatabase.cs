using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Databases/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemSO> allItems = new();

    public ItemSO GetItemByID(int id)
    {
        if(id < allItems.Count)
        {
            return allItems[id];
        }
        return null;
    }
}