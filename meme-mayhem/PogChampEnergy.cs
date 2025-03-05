using UnityEngine;

public class PogChampEnergy : ItemBase
{
    [Header("PogChamp Energy Settings")]
    public float damageBonusPercent = 20f;
    public float fireRatePenaltyPercent = 5f;
    
    private float originalDamage;
    private float originalFireRate;
    
    public override void ApplyPassiveEffect(PlayerController player)
    {
        base.ApplyPassiveEffect(player);
        
        // Store original values
        if (player.currentWeapon != null)
        {
            originalDamage = player.currentWeapon.damage;
            originalFireRate = player.currentWeapon.fireRate;
            
            // Apply bonuses
            player.currentWeapon.damage *= (1 + damageBonusPercent / 100f);
            player.currentWeapon.fireRate *= (1 + fireRatePenaltyPercent / 100f);
        }
        
        // Subscribe to weapon change event
        player.OnWeaponChanged += UpdateWeaponStats;
    }
    
    private void UpdateWeaponStats(WeaponBase newWeapon)
    {
        if (newWeapon != null)
        {
            originalDamage = newWeapon.damage;
            originalFireRate = newWeapon.fireRate;
            
            newWeapon.damage *= (1 + damageBonusPercent / 100f);
            newWeapon.fireRate *= (1 + fireRatePenaltyPercent / 100f);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up event subscription if the item is removed
        if (GetComponentInParent<PlayerController>() is PlayerController player)
        {
            player.OnWeaponChanged -= UpdateWeaponStats;
            
            // Revert weapon stats if item is removed
            if (player.currentWeapon != null)
            {
                player.currentWeapon.damage = originalDamage;
                player.currentWeapon.fireRate = originalFireRate;
            }
        }
    }
}