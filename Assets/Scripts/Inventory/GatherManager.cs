using UnityEngine;
using System.Collections.Generic;

public class GatherManager : MonoBehaviour
{
    [SerializeField] private ItemData[] gatherableItems;
    [SerializeField] private RarityDropConfig rarityConfig;

    public ItemData GetRandomItemBasedOnValue(int totalValue, out int quantity)
    {
        float rarityChance = GetChanceFromValue(totalValue);
        Rarity targetRarity = GetRarityFromChance(rarityChance);

        List<ItemData> candidates = new();
        foreach (var item in gatherableItems)
        {
            if (item != null && item.rarity == targetRarity)
                candidates.Add(item);
        }

        if (candidates.Count == 0)
        {
            quantity = 0;
            return null;
        }

        ItemData selectedItem = candidates[Random.Range(0, candidates.Count)];
        quantity = selectedItem.maxStack > 1 ? 10 : 1;
        return selectedItem;
    }

    private float GetChanceFromValue(int value)
    {
        foreach (var threshold in rarityConfig.valueThresholds)
        {
            if (value <= threshold.valueCap)
                return Random.Range(threshold.chanceRange.x, threshold.chanceRange.y);
        }

        // If above all caps, use highest range
        var last = rarityConfig.valueThresholds[^1];
        return Random.Range(last.chanceRange.x, last.chanceRange.y);
    }

    private Rarity GetRarityFromChance(float chance)
    {
        foreach (var rarityChance in rarityConfig.rarityChances)
        {
            if (chance < rarityChance.maxChance)
                return rarityChance.rarity;
        }

        // Fallback to highest rarity
        return rarityConfig.rarityChances[^1].rarity;
    }
}
