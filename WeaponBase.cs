using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    public string weaponName;
    public Sprite weaponIcon;
    public float damage = 1f;
    public float fireRate = 0.2f;
    public float range = 10f;
    
    [Header("Visual Effects")]
    public GameObject muzzleFlashPrefab;
    public AudioClip fireSound;
    
    // Synergy properties
    [HideInInspector] public bool hasSynergy = false;
    [HideInInspector] public string synergyDescription = "";
    
    // Internal variables
    protected PlayerController player;
    
    public virtual void Initialize(PlayerController owner)
    {
        player = owner;
    }
    
    public abstract void Fire(Vector3 position, Vector2 direction);
    
    protected void PlayMuzzleEffect(Vector3 position)
    {
        if (muzzleFlashPrefab != null)
        {
            Instantiate(muzzleFlashPrefab, position, Quaternion.identity);
        }
        
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, position);
        }
    }
}