using UnityEngine;
using System;
using System.Collections.Generic;

public class GatherManager : MonoBehaviour
{
    [SerializeField] private ItemData[] gatherableItems;
    [SerializeField] private RarityDropConfig rarityConfig;
    [SerializeField] private UIManager uiManager;

    // Event triggered when a valid item is gathered
    public event Action<ItemData, int> OnItemGathered;

    private Func<int> getTotalLifetimeValue;
    private Func<int> getCurrentWeight;
    private Func<int> getMaxWeight;

    public void Initialize(Func<int> getValue, Func<int> getWeight, Func<int> getMaxWeight)
    {
        getTotalLifetimeValue = getValue;
        getCurrentWeight = getWeight;
        this.getMaxWeight = getMaxWeight;
    }

    public void Gather()
    {
        if (getTotalLifetimeValue == null || getCurrentWeight == null || getMaxWeight == null)
            return;

        int totalLifetimeValue = getTotalLifetimeValue();
        int currentWeight = getCurrentWeight();
        int maxWeight = getMaxWeight();

        int itemsToGather = UnityEngine.Random.Range(3, 6); // 3 to 5 items

        for (int i = 0; i < itemsToGather; i++)
        {
            ItemData item = GetRandomItemBasedOnValue(totalLifetimeValue, out _);
            if (item == null) continue;

            int quantity = item.maxStack == 1 ? 1 : UnityEngine.Random.Range(1, 6) * 10;

            int weightToAdd = item.weight * quantity;

            if (currentWeight + weightToAdd > maxWeight)
            {
                uiManager.ShowPopup(StringConstants.MAX_WEIGHT_LIMIT_REACHED_POPUP);
                return;
            }

            currentWeight += weightToAdd;
            SoundManager.Instance.PlayUIClick();
            OnItemGathered?.Invoke(item, quantity);
        }
    }

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

        ItemData selectedItem = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        quantity = selectedItem.maxStack > 1 ? 10 : 1;
        return selectedItem;
    }

    private float GetChanceFromValue(int value)
    {
        foreach (var threshold in rarityConfig.valueThresholds)
        {
            if (value <= threshold.valueCap)
                return UnityEngine.Random.Range(threshold.chanceRange.x, threshold.chanceRange.y);
        }

        var last = rarityConfig.valueThresholds[^1];
        return UnityEngine.Random.Range(last.chanceRange.x, last.chanceRange.y);
    }

    private Rarity GetRarityFromChance(float chance)
    {
        foreach (var rarityChance in rarityConfig.rarityChances)
        {
            if (chance < rarityChance.maxChance)
                return rarityChance.rarity;
        }

        return rarityConfig.rarityChances[^1].rarity;
    }
}