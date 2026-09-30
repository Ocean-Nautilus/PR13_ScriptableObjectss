using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject — центральная база предметов.
/// Создание ассета: Create → PR13 → Item Database
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "PR13/Item Database", order = 3)]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> allItems = new List<ItemData>();

    public int Count => allItems.Count;

    public ItemData GetByName(string itemName)
    {
        foreach (ItemData item in allItems)
        {
            if (item != null && item.itemName == itemName)
                return item;
        }
        return null;
    }

    public List<ItemData> GetByCategory(ItemCategory category)
    {
        List<ItemData> result = new List<ItemData>();
        foreach (ItemData item in allItems)
        {
            if (item != null && item.category == category)
                result.Add(item);
        }
        return result;
    }

    public List<ItemData> GetByRarity(ItemRarity rarity)
    {
        List<ItemData> result = new List<ItemData>();
        foreach (ItemData item in allItems)
        {
            if (item != null && item.rarity == rarity)
                result.Add(item);
        }
        return result;
    }

    public ItemData GetRandom()
    {
        if (allItems.Count == 0) return null;
        return allItems[UnityEngine.Random.Range(0, allItems.Count)];
    }
}
