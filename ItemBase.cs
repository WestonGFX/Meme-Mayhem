using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName;
    public string description;
    public Sprite itemIcon;
    public ItemType itemType;
    
    public enum ItemType
    {
        Passive,
        Active
    }
    
    public virtual void OnPickup(PlayerController player)
    {
        // Default behavior when item is picked up
        if (itemType == ItemType.Passive)
        {
            player.passiveItems.Add(this);
            ApplyPassiveEffect(player);
        }
        else
        {
            player.activeItem = this;
        }
    }
    
    public virtual void ApplyPassiveEffect(PlayerController player)
    {
        // Override in subclasses for specific passive effects
    }
    
    public virtual void Use(PlayerController player)
    {
        // Override in subclasses for specific active item uses
    }
}