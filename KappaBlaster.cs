using UnityEngine;

public class KappaBlaster : WeaponBase
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    
    public override void Fire(Vector3 position, Vector2 direction)
    {
        // Play effects
        PlayMuzzleEffect(position);
        
        // Create projectile
        GameObject projectileObj = Instantiate(projectilePrefab, position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        
        if (projectile != null)
        {
            // Set rotation to match direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectileObj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            // Initialize projectile
            projectile.Initialize(direction, damage, projectileSpeed, range);
            
            // Apply any synergy effects
            if (hasSynergy)
            {
                ApplySynergyEffects(projectile);
            }
        }
    }
    
    private void ApplySynergyEffects(Projectile projectile)
    {
        // Apply different effects based on active synergies
        // These would be set by the WeaponSynergyManager
        
        if (synergyDescription.Contains("Surprise Shot"))
        {
            // 15% chance to make this an explosive projectile
            if (Random.Range(0f, 1f) < 0.15f)
            {
                projectile.MakeExplosive(2f, damage * 1.5f);
            }
        }
        
        // Additional synergy effects would be applied here
    }
}