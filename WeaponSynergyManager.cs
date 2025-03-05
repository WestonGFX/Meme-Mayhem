using UnityEngine;
using System.Collections.Generic;

public class WeaponSynergyManager : MonoBehaviour
{
    public static WeaponSynergyManager Instance { get; private set; }
    
    [System.Serializable]
    public class Synergy
    {
        public string weaponName;
        public string itemName;
        public string synergyName;
        public string description;
        
        // Actions to take when synergy is applied
        public float damageMultiplier = 1.0f;
        public float fireRateMultiplier = 1.0f;
        public float rangeMultiplier = 1.0f;
        
        public bool addExplosive = false;
        public bool addPoison = false;
        public bool addChaining = false;
        public bool addHoming = false;
    }
    
    public List<Synergy> possibleSynergies = new List<Synergy>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize synergies
            InitializeSynergies();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSynergies()
    {
        // Example synergies (these would be loaded from data in a full implementation)
        
        // Kappa Blaster + PogChamp Energy synergy
        Synergy surpriseShot = new Synergy
        {
            weaponName = "Kappa Blaster",
            itemName = "PogChamp Energy",
            synergyName = "Surprise Shot",
            description = "15% chance to fire an explosive round",
            damageMultiplier = 1.2f,
            addExplosive = true
        };
        possibleSynergies.Add(surpriseShot);
        
        // LUL Cannon + 4Head Comedy synergy
        Synergy laughRiot = new Synergy
        {
            weaponName = "LUL Cannon",
            itemName = "4Head Comedy",
            synergyName = "Laugh Riot",
            description = "Enemies hit by LUL projectiles may begin laughing, unable to attack for 2 seconds",
            fireRateMultiplier = 1.15f
        };
        possibleSynergies.Add(laughRiot);
        
        // Add more synergies here
    }
    
    public void ApplySynergy(WeaponBase weapon, ItemBase item)
    {
        if (weapon == null || item == null)
            return;
            
        // Check for matching synergies
        foreach (var synergy in possibleSynergies)
        {
            if (synergy.weaponName == weapon.weaponName && 
                synergy.itemName == item.itemName)
            {
                // Found a synergy, apply it
                ApplySynergyEffects(weapon, synergy);
                
                // Set synergy flag and description
                weapon.hasSynergy = true;
                weapon.synergyDescription = synergy.synergyName + ": " + synergy.description;
                
                // Show synergy discovery UI
                ShowSynergyDiscovered(synergy);
                
                break;
            }
        }
    }
    
    private void ApplySynergyEffects(WeaponBase weapon, Synergy synergy)
    {
        // Apply stat multipliers
        weapon.damage *= synergy.damageMultiplier;
        weapon.fireRate /= synergy.fireRateMultiplier; // Lower value = faster shooting
        weapon.range *= synergy.rangeMultiplier;
        
        // Apply special effects
        if (synergy.addExplosive)
        {
            // Add explosive component or set flag for explosive shots
            // This would be implemented differently depending on weapon type
        }
        
        if (synergy.addPoison)
        {
            // Add poison effect
        }
        
        if (synergy.addChaining)
        {
            // Add chain effect
        }
        
        if (synergy.addHoming)
        {
            // Add homing effect
        }
    }
    
    private void ShowSynergyDiscovered(Synergy synergy)
    {
        // Show UI notification for discovered synergy
        Debug.Log("Synergy discovered: " + synergy.synergyName);
        
        // In a full implementation, this would create a UI element
    }
}