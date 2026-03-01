using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Player inventory for storing caught fish.
/// Connects to FishDatabase to obtain random fish.
/// </summary>
public class FishInventory : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _debugMode = false;

    private List<FishData> _caughtFish = new List<FishData>();
    private FishDatabase _database;

    /// <summary>Event fired when a fish is added to the inventory.</summary>
    public event Action<FishData> OnFishAdded;

    /// <summary>Event fired when a fish is removed from the inventory.</summary>
    public event Action<FishData> OnFishRemoved;

    /// <summary>List of caught fish (read-only).</summary>
    public IReadOnlyList<FishData> CaughtFish => _caughtFish.AsReadOnly();

    /// <summary>Total number of fish in the inventory.</summary>
    public int Count => _caughtFish.Count;

    private void Awake()
    {
        _database = FishDatabase.Instance;
    }

    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    //  INVENTORY MANAGEMENT
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    /// <summary>
    /// Adds a fish to the inventory.
    /// </summary>
    public void AddFish(FishData fish)
    {
        if (fish == null)
        {
            Debug.LogWarning("[FishInventory] Attempted to add a null fish.");
            return;
        }

        _caughtFish.Add(fish);
        OnFishAdded?.Invoke(fish);

        if (_debugMode)
            Debug.Log($"[FishInventory] Added: {fish.name} ({fish.rarity}) - Total: {_caughtFish.Count}");
    }

    /// <summary>
    /// Removes a fish from the inventory by reference.
    /// </summary>
    public bool RemoveFish(FishData fish)
    {
        bool removed = _caughtFish.Remove(fish);
        if (removed)
        {
            OnFishRemoved?.Invoke(fish);
            if (_debugMode)
                Debug.Log($"[FishInventory] Removed: {fish.name} ({fish.rarity}) - Total: {_caughtFish.Count}");
        }
        return removed;
    }

    /// <summary>
    /// Removes a fish from the inventory by name (first match).
    /// </summary>
    public bool RemoveFishByName(string fishName)
    {
        FishData fish = _caughtFish.FirstOrDefault(f => f.name == fishName);
        if (fish != null)
            return RemoveFish(fish);
        return false;
    }

    /// <summary>
    /// Returns a random fish from the inventory (without removing it).
    /// </summary>
    public FishData GetRandomFishFromInventory()
    {
        if (_caughtFish.Count == 0)
        {
            Debug.LogWarning("[FishInventory] Inventory is empty.");
            return null;
        }

        int index = UnityEngine.Random.Range(0, _caughtFish.Count);
        return _caughtFish[index];
    }

    /// <summary>
    /// Returns and removes a random fish from the inventory.
    /// </summary>
    public FishData TakeRandomFishFromInventory()
    {
        FishData fish = GetRandomFishFromInventory();
        if (fish != null)
            RemoveFish(fish);
        return fish;
    }

    /// <summary>
    /// Clears the entire inventory.
    /// </summary>
    public void ClearInventory()
    {
        _caughtFish.Clear();
        if (_debugMode)
            Debug.Log("[FishInventory] Inventory cleared.");
    }

    /// <summary>
    /// Gets all fish in the inventory of a specific rarity.
    /// </summary>
    public List<FishData> GetFishFromInventoryByRarity(FishRarity rarity)
    {
        return _caughtFish.Where(f => f.GetRarity() == rarity).ToList();
    }

    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    //  FISHING: CATCH NEW RANDOM FISH
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    /// <summary>
    /// Catches a random fish weighted by rarity (60% Common, 30% Uncommon, 10% Rare)
    /// and probability within that rarity tier. Automatically adds it to the inventory.
    /// </summary>
    public FishData CatchRandomFish()
    {
        FishData fish = _database.GetRandomFish();
        if (fish != null)
            AddFish(fish);
        return fish;
    }

    /// <summary>
    /// Catches a random Common fish and adds it to the inventory.
    /// </summary>
    public FishData CatchCommonFish()
    {
        FishData fish = _database.GetRandomCommonFish();
        if (fish != null)
            AddFish(fish);
        return fish;
    }

    /// <summary>
    /// Catches a random Uncommon fish and adds it to the inventory.
    /// </summary>
    public FishData CatchUncommonFish()
    {
        FishData fish = _database.GetRandomUncommonFish();
        if (fish != null)
            AddFish(fish);
        return fish;
    }

    /// <summary>
    /// Catches a random Rare fish and adds it to the inventory.
    /// </summary>
    public FishData CatchRareFish()
    {
        FishData fish = _database.GetRandomRareFish();
        if (fish != null)
            AddFish(fish);
        return fish;
    }

    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    //  UTILITIES
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    /// <summary>
    /// Loads the sprite for a fish in the inventory.
    /// </summary>
    public Sprite LoadSprite(FishData fish)
    {
        return _database.LoadFishSprite(fish);
    }

    /// <summary>
    /// Inventory summary for debugging.
    /// </summary>
    public string GetInventorySummary()
    {
        int common = _caughtFish.Count(f => f.GetRarity() == FishRarity.Common);
        int uncommon = _caughtFish.Count(f => f.GetRarity() == FishRarity.Uncommon);
        int rare = _caughtFish.Count(f => f.GetRarity() == FishRarity.Rare);

        return $"Inventory: {_caughtFish.Count} fish (Common: {common}, Uncommon: {uncommon}, Rare: {rare})";
    }
}
