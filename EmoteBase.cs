using UnityEngine;

[System.Serializable]
public class EmoteBase
{
    public string emoteName;
    public Sprite emoteIcon;
    public EmoteType type;
    public EmoteRarity rarity;
    
    [TextArea(3, 5)]
    public string description;
    
    // Emote properties that affect fusion results
    public float powerFactor = 1.0f;
    public float speedFactor = 1.0f;
    public float rangeFactor = 1.0f;
    public float luckFactor = 1.0f;
    
    // Special properties
    public bool hasExplosiveProperty = false;
    public bool hasPoisonProperty = false;
    public bool hasChainProperty = false;
    public bool hasHealProperty = false;
    
    public enum EmoteType
    {
        Kappa,
        PogChamp,
        LUL,
        MonkaS,
        FeelsGoodMan,
        FeelsBadMan,
        ResidentSleeper,
        BabyRage,
        TriHard,
        FrankerZ
    }
    
    public enum EmoteRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}