using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public int price = 15;
    public GameObject priceTextPrefab;
    
    private Text priceText;
    private bool isSold = false;
    
    private void Start()
    {
        // Create floating price text
        if (priceTextPrefab != null)
        {
            GameObject textObj = Instantiate(priceTextPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            priceText = textObj.GetComponent<Text>();
            
            if (priceText != null)
            {
                priceText.text = price.ToString();
            }
            
            // Parent the text to this object
            textObj.transform.SetParent(transform);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSold)
            return;
            
        if (collision.CompareTag("Player"))
        {
            // Check if player has enough money
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null && player.coins >= price)
            {
                // Deduct money
                player.coins -= price;
                
                // Mark as sold
                isSold = true;
                
                // Update price text
                if (priceText != null)
                {
                    priceText.text = "SOLD";
                }
                
                // Give the item to the player
                ItemBase item = GetComponent<ItemBase>();
                if (item != null)
                {
                    item.OnPickup(player);
                }
                
                // Destroy the item in the shop
                Destroy(gameObject, 0.5f);
            }
        }
    }
}