using UnityEngine;
using System.Collections.Generic;

public class EmoteFusionSystem : MonoBehaviour
{
    public static EmoteFusionSystem Instance { get; private set; }
    
    [SerializeField] private List<EmoteFusionRecipe> knownFusions = new List<EmoteFusionRecipe>();
    
    // Store discovered fusions for this run
    private List<EmoteFusionRecipe> discoveredFusions = new List<EmoteFusionRecipe>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public FusionResult FuseEmotes(List<EmoteBase> emotesToFuse)
    {
        if (emotesToFuse.Count < 2)
            return null;
        
        // First check if this is a known recipe
        EmoteFusionRecipe knownRecipe = FindKnownRecipe(emotesToFuse);
        if (knownRecipe != null)
        {
            // Add to discovered fusions if it's new
            if (!discoveredFusions.Contains(knownRecipe))
            {
                discoveredFusions.Add(knownRecipe);
            }
            
            return knownRecipe.result;
        }
        
        // Generate a procedural fusion if no known recipe exists
        return GenerateProceduralFusion(emotesToFuse);
    }
    
    private EmoteFusionRecipe FindKnownRecipe(List<EmoteBase> emotesToFuse)
    {
        // Sort emotes by name to ensure consistent matching regardless of order
        List<EmoteBase> sortedEmotes = new List<EmoteBase>(emotesToFuse);
        sortedEmotes.Sort((a, b) => string.Compare(a.emoteName, b.emoteName));
        
        foreach (var recipe in knownFusions)
        {
            if (recipe.ingredients.Count != sortedEmotes.Count)
                continue;
                
            // Sort recipe ingredients
            List<EmoteBase> sortedIngredients = new List<EmoteBase>(recipe.ingredients);
            sortedIngredients.Sort((a, b) => string.Compare(a.emoteName, b.emoteName));
            
            // Check if recipes match
            bool match = true;
            for (int i = 0; i < sortedEmotes.Count; i++)
            {
                if (sortedEmotes[i].emoteName != sortedIngredients[i].emoteName)
                {
                    match = false;
                    break;
                }
            }
            
            if (match)
                return recipe;
        }
        
        return null;
    }
    
    private FusionResult GenerateProceduralFusion(List<EmoteBase> emotesToFuse)
    {
        FusionResult result = new FusionResult();
        
        // Calculate fusion properties based on input emotes
        float powerSum = 0f;
        float speedSum = 0f;
        float rangeSum = 0f;
        float luckSum = 0f;
        bool hasExplosive = false;
        bool hasPoison = false;
        bool hasChain = false;
        bool hasHeal = false;
        
        foreach (var emote in emotesToFuse)
        {
            powerSum += emote.powerFactor;
            speedSum += emote.speedFactor;
            rangeSum += emote.rangeFactor;
            luckSum += emote.luckFactor;
            
            hasExplosive |= emote.hasExplosiveProperty;
            hasPoison |= emote.hasPoisonProperty;
            hasChain |= emote.hasChainProperty;
            hasHeal |= emote.hasHealProperty;
        }
        
        // Average the numerical values
        float count = emotesToFuse.Count;
        result.powerModifier = powerSum / count;
        result.speedModifier = speedSum / count;
        result.rangeModifier = rangeSum / count;
        result.luckModifier = luckSum / count;
        
        // Set special properties
        result.isExplosive = hasExplosive;
        result.isPoison = hasPoison;
        result.isChaining = hasChain;
        result.isHealing = hasHeal;
        
        // Generate a name for the fusion
        result.fusionName = GenerateFusionName(emotesToFuse);
        
        // Generate a description
        result.description = GenerateFusionDescription(result);
        
        return result;
    }
    
    private string GenerateFusionName(List<EmoteBase> emotesToFuse)
    {
        if (emotesToFuse.Count == 2)
        {
            return emotesToFuse[0].emoteName + "-" + emotesToFuse[1].emoteName + " Fusion";
        }
        else
        {
            return "Multi-Emote Fusion";
        }
    }
    
    private string GenerateFusionDescription(FusionResult fusion)
    {
        string desc = "A fusion with ";
        
        List<string> properties = new List<string>();
        
        if (fusion.powerModifier > 1.5f)
            properties.Add("high damage");
        else if (fusion.powerModifier < 0.7f)
            properties.Add("low damage");
            
        if (fusion.speedModifier > 1.5f)
            properties.Add("fast speed");
        else if (fusion.speedModifier < 0.7f)
            properties.Add("slow speed");
            
        if (fusion.rangeModifier > 1.5f)
            properties.Add("long range");
        else if (fusion.rangeModifier < 0.7f)
            properties.Add("short range");
            
        if (fusion.luckModifier > 1.5f)
            properties.Add("high luck");
            
        if (fusion.isExplosive)
            properties.Add("explosive properties");
            
        if (fusion.isPoison)
            properties.Add("poison effects");
            
        if (fusion.isChaining)
            properties.Add("chain reactions");
            
        if (fusion.isHealing)
            properties.Add("healing abilities");
        
        for (int i = 0; i < properties.Count; i++)
        {
            if (i > 0)
            {
                if (i == properties.Count - 1)
                    desc += " and ";
                else
                    desc += ", ";
            }
            
            desc += properties[i];
        }
        
        return desc + ".";
    }
    
    public void SaveDiscoveredFusions()
    {
        // Save to player profile or game data
        PlayerData.AddDiscoveredFusions(discoveredFusions);
    }
}

[System.Serializable]
public class EmoteFusionRecipe
{
    public List<EmoteBase> ingredients = new List<EmoteBase>();
    public FusionResult result;
}

[System.Serializable]
public class FusionResult
{
    public string fusionName;
    public string description;
    public Sprite fusionIcon;
    
    // Modifiers
    public float powerModifier = 1.0f;
    public float speedModifier = 1.0f;
    public float rangeModifier = 1.0f;
    public float luckModifier = 1.0f;
    
    // Special effects
    public bool isExplosive = false;
    public bool isPoison = false;
    public bool isChaining = false;
    public bool isHealing = false;
    
    // Apply the fusion to a weapon
    public void ApplyToWeapon(WeaponBase weapon)
    {
        weapon.damage *= powerModifier;
        weapon.fireRate /= speedModifier; // Lower fire rate = faster shooting
        
        // Set special properties through the weapon's components or scripts
        Projectile projectile = weapon.GetComponentInChildren<Projectile>();
        if (projectile != null)
        {
            if (isExplosive)
                projectile.MakeExplosive(2f * rangeModifier, weapon.damage * 0.7f);
                
            // Additional properties would be set here
        }
    }
}