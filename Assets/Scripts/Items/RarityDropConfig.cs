using UnityEngine;

[CreateAssetMenu(fileName = "RarityDropConfig", menuName = "Inventory/Rarity Drop Config")]
public class RarityDropConfig : ScriptableObject
{
    [System.Serializable]
    public struct RarityThreshold
    {
        public int valueCap;             // max inventory value for this range
        public Vector2 chanceRange;      // range for Random.Range
    }

    [System.Serializable]
    public struct RarityProbability
    {
        public Rarity rarity;
        [Range(0f, 1f)] public float maxChance; // cumulative threshold
    }

    [Header("Rarity Thresholds Based on Total Value")]
    public RarityThreshold[] valueThresholds;

    [Header("Rarity Probabilities (ordered by priority)")]
    public RarityProbability[] rarityChances;
}