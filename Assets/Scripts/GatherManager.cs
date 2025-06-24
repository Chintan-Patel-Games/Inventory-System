using UnityEngine;
using System.Collections.Generic;

public class GatherManager : MonoBehaviour
{
    [SerializeField] private ItemData[] gatherableItems;

    public ItemData GetRandomItemBasedOnValue(int totalValue, out int quantity)
    {
        // 1. Determine chance range based on total inventory value
        float rarityChance = totalValue switch
        {
            <= 100 => Random.Range(0f, 0.2f),
            <= 500 => Random.Range(0f, 0.4f),
            <= 1000 => Random.Range(0f, 0.7f),
            <= 2500 => Random.Range(0f, 0.9f),
            <= 5000 => Random.Range(0f, 1.0f),
            _ => Random.Range(0.85f, 1.0f)
        };

        // 2. Decide rarity from chance
        Rarity targetRarity = rarityChance switch
        {
            < 0.2f => Rarity.VeryCommon,
            < 0.4f => Rarity.Common,
            < 0.7f => Rarity.Rare,
            < 0.9f => Rarity.Epic,
            < 1.0f => Rarity.Legendary,
            _ => Rarity.Legendary
        };

        // 3. Manually filter matching rarity items
        List<ItemData> candidates = new();

        foreach (var item in gatherableItems)
        {
            if (item.rarity == targetRarity)
                candidates.Add(item);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"No items found for rarity: {targetRarity}");
            quantity = 0;
            return null;
        }

        // 4. Pick random item from candidates
        ItemData selectedItem = candidates[Random.Range(0, candidates.Count)];

        // 5. Determine quantity if stackable
        quantity = selectedItem.maxStack > 1 ? 10 : 1;

        return selectedItem;
    }
}
