using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Loads and stores the fish database from a JSON file in Resources.
/// Provides functions to get random fish by rarity and probability.
/// </summary>
public class FishDatabase
{
    private static FishDatabase _instance;
    public static FishDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FishDatabase();
                _instance.LoadDatabase();
            }
            return _instance;
        }
    }

    private List<FishData> _allFish = new List<FishData>();
    private Dictionary<FishRarity, List<FishData>> _fishByRarity = new Dictionary<FishRarity, List<FishData>>();

    // Rarity weights for the general catch function
    private const float COMMON_WEIGHT = 0.60f;
    private const float UNCOMMON_WEIGHT = 0.30f;
    private const float RARE_WEIGHT = 0.10f;

    /// <summary>All loaded fish.</summary>
    public IReadOnlyList<FishData> AllFish => _allFish.AsReadOnly();

    /// <summary>
    /// Loads the database from the JSON file located at Resources/Fish/FishDatabase.
    /// </summary>
    public void LoadDatabase()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Fish/FishDatabase");
        if (jsonFile == null)
        {
            Debug.LogError("[FishDatabase] Could not find Fish/FishDatabase.json in Resources.");
            return;
        }

        FishDataList dataList = JsonUtility.FromJson<FishDataList>(jsonFile.text);
        if (dataList == null || dataList.fish == null)
        {
            Debug.LogError("[FishDatabase] Error parsing the fish JSON.");
            return;
        }

        _allFish = dataList.fish;

        // Organize by rarity
        _fishByRarity.Clear();
        _fishByRarity[FishRarity.Common] = new List<FishData>();
        _fishByRarity[FishRarity.Uncommon] = new List<FishData>();
        _fishByRarity[FishRarity.Rare] = new List<FishData>();

        foreach (FishData fish in _allFish)
        {
            FishRarity rarity = fish.GetRarity();
            _fishByRarity[rarity].Add(fish);
        }

        Debug.Log($"[FishDatabase] Database loaded: {_allFish.Count} fish " +
                  $"(Common: {_fishByRarity[FishRarity.Common].Count}, " +
                  $"Uncommon: {_fishByRarity[FishRarity.Uncommon].Count}, " +
                  $"Rare: {_fishByRarity[FishRarity.Rare].Count})");
    }

    /// <summary>
    /// Reloads the database (useful if the JSON changed at runtime).
    /// </summary>
    public void ReloadDatabase()
    {
        _allFish.Clear();
        _fishByRarity.Clear();
        LoadDatabase();
    }

    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    //  RANDOM FISH BY RARITY
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    /// <summary>
    /// Returns a random Common fish, weighted by probability.
    /// </summary>
    public FishData GetRandomCommonFish()
    {
        return GetRandomFishByRarity(FishRarity.Common);
    }

    /// <summary>
    /// Returns a random Uncommon fish, weighted by probability.
    /// </summary>
    public FishData GetRandomUncommonFish()
    {
        return GetRandomFishByRarity(FishRarity.Uncommon);
    }

    /// <summary>
    /// Returns a random Rare fish, weighted by probability.
    /// </summary>
    public FishData GetRandomRareFish()
    {
        return GetRandomFishByRarity(FishRarity.Rare);
    }

    /// <summary>
    /// Returns a random fish of the specified rarity, weighted by its probability field.
    /// </summary>
    public FishData GetRandomFishByRarity(FishRarity rarity)
    {
        if (!_fishByRarity.ContainsKey(rarity) || _fishByRarity[rarity].Count == 0)
        {
            Debug.LogWarning($"[FishDatabase] No fish found for rarity {rarity}.");
            return null;
        }

        return GetWeightedRandom(_fishByRarity[rarity]);
    }

    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    //  GENERAL: RARITY + PROBABILITY
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    /// <summary>
    /// Returns a random fish by first rolling for rarity
    /// (60% Common, 30% Uncommon, 10% Rare) and then weighting
    /// by probability within that rarity tier.
    /// </summary>
    public FishData GetRandomFish()
    {
        // First determine the rarity
        float roll = Random.Range(0f, 1f);

        FishRarity selectedRarity;

        if (roll < RARE_WEIGHT)
        {
            selectedRarity = FishRarity.Rare;
        }
        else if (roll < RARE_WEIGHT + UNCOMMON_WEIGHT)
        {
            selectedRarity = FishRarity.Uncommon;
        }
        else
        {
            selectedRarity = FishRarity.Common;
        }

        // If the selected rarity has no fish, fallback
        if (!_fishByRarity.ContainsKey(selectedRarity) || _fishByRarity[selectedRarity].Count == 0)
        {
            Debug.LogWarning($"[FishDatabase] No fish found for rarity {selectedRarity}, using fallback.");
            // Try any rarity that has fish
            foreach (var kvp in _fishByRarity)
            {
                if (kvp.Value.Count > 0)
                    return GetWeightedRandom(kvp.Value);
            }
            return null;
        }

        return GetWeightedRandom(_fishByRarity[selectedRarity]);
    }

    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    //  UTILITIES
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    /// <summary>
    /// Loads a fish sprite using its spritePath from Resources.
    /// </summary>
    public Sprite LoadFishSprite(FishData fish)
    {
        if (fish == null || string.IsNullOrEmpty(fish.spritePath))
        {
            Debug.LogWarning("[FishDatabase] FishData is null or has no spritePath.");
            return null;
        }

        Sprite sprite = Resources.Load<Sprite>(fish.spritePath);
        if (sprite == null)
        {
            Debug.LogWarning($"[FishDatabase] Sprite not found at: {fish.spritePath}");
        }
        return sprite;
    }

    /// <summary>
    /// Gets all fish of a specific rarity.
    /// </summary>
    public List<FishData> GetFishByRarity(FishRarity rarity)
    {
        if (_fishByRarity.ContainsKey(rarity))
            return new List<FishData>(_fishByRarity[rarity]);

        return new List<FishData>();
    }

    /// <summary>
    /// Selects a random element from the list weighted by the probability field.
    /// Uses weighted random selection.
    /// </summary>
    private FishData GetWeightedRandom(List<FishData> fishList)
    {
        if (fishList == null || fishList.Count == 0) return null;

        // Sum all weights
        int totalWeight = 0;
        foreach (FishData fish in fishList)
        {
            totalWeight += Mathf.Max(1, fish.probability);
        }

        // Generate a random number between 0 and total weight
        int randomValue = Random.Range(0, totalWeight);

        // Select the corresponding fish
        int cumulative = 0;
        foreach (FishData fish in fishList)
        {
            cumulative += Mathf.Max(1, fish.probability);
            if (randomValue < cumulative)
                return fish;
        }

        // Fallback (should never reach here)
        return fishList[fishList.Count - 1];
    }
}
