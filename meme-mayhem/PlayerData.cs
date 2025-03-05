using UnityEngine;
using System.Collections.Generic;

public static class PlayerData
{
    // Persistent data
    public static int totalRuns = 0;
    public static int bestScore = 0;
    public static int totalDeaths = 0;
    public static int totalBossesDefeated = 0;
    
    // Unlocks
    public static List<string> unlockedCharacters = new List<string> { "Classic Pepe" };
    public static List<string> unlockedItems = new List<string>();
    public static List<EmoteFusionRecipe> discoveredFusions = new List<EmoteFusionRecipe>();
    
    // Currency for meta-progression
    public static int memeCurrency = 0;
    
    // Stats
    public static Dictionary<string, int> enemiesDefeated = new Dictionary<string, int>();
    public static Dictionary<string, int> itemsCollected = new Dictionary<string, int>();
    
    // Save data
    public static void SaveData()
    {
        // In a full implementation, this would serialize data to disk
        // For prototype, just log data
        Debug.Log("Data saved!");
    }
    
    // Load data
    public static void LoadData()
    {
        // In a full implementation, this would deserialize data from disk
        Debug.Log("Data loaded!");
    }
    
    // Add discovered fusion
    public static void AddDiscoveredFusion(EmoteFusionRecipe fusion)
    {
        if (!discoveredFusions.Contains(fusion))
        {
            discoveredFusions.Add(fusion);
            SaveData();
        }
    }
    
    // Add multiple discovered fusions
    public static void AddDiscoveredFusions(List<EmoteFusionRecipe> fusions)
    {
        bool changed = false;
        foreach (var fusion in fusions)
        {
            if (!discoveredFusions.Contains(fusion))
            {
                discoveredFusions.Add(fusion);
                changed = true;
            }
        }
        
        if (changed)
        {
            SaveData();
        }
    }
    
    // Update best score
    public static void UpdateBestScore(int score)
    {
        if (score > bestScore)
        {
            bestScore = score;
            SaveData();
        }
    }
    
    // Add meme currency
    public static void AddMemeCurrency(int amount)
    {
        memeCurrency += amount;
        SaveData();
    }
}