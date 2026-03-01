using System;
using System.Collections.Generic;

/// <summary>
/// Fish rarity categories.
/// </summary>
public enum FishRarity
{
    Common,
    Uncommon,
    Rare
}

/// <summary>
/// Data for an individual fish. Serialized from JSON.
/// </summary>
[Serializable]
public class FishData
{
    /// <summary>Name of the fish.</summary>
    public string name;

    /// <summary>Rarity of the fish (Common, Uncommon, Rare).</summary>
    public string rarity;

    /// <summary>
    /// Relative probability within its rarity tier (1-10).
    /// A higher value means more likely to appear.
    /// </summary>
    public int probability;

    /// <summary>
    /// Sprite path inside Resources (without extension).
    /// Example: "Fish/Sprites/Bass"
    /// </summary>
    public string spritePath;

    /// <summary>Parses the rarity string to an enum value.</summary>
    public FishRarity GetRarity()
    {
        if (Enum.TryParse(rarity, true, out FishRarity result))
            return result;

        return FishRarity.Common;
    }
}

/// <summary>
/// Wrapper to deserialize the fish list from JSON.
/// </summary>
[Serializable]
public class FishDataList
{
    public List<FishData> fish;
}
