using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;
    public float lifetime = 5f;
    public GameObject hitEffect;
    
    // Special effects
    private bool isExplosive = false;
    private float explosionRadius = 2f;
    private float explosionDamage = 2f;
    public GameObject explosionPrefab;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Initialize(Vector2 dir, float dmg, float spd = 10f, float range = 10f)
    {
        direction = dir.normalized;
        damage = dmg;
        speed = spd;
        lifetime = range / speed;
        
        Destroy(gameObject, lifetime);
    }
    
    private void Start()
    {
        rb.velocity = direction * speed;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            if (isExplosive)
            {
                Explode();
            }
            else
            {
                // Show hit effect
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
                }
                
                Destroy(gameObject);
            }
        }
        else if (collision.CompareTag("Wall"))
        {
            if (isExplosive)
            {
                Explode();
            }
            else
            {
                // Show hit effect against wall
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
                }
                
                Destroy(gameObject);
            }
        }
    }
    
    public void MakeExplosive(float radius, float explDamage)
    {
        isExplosive = true;
        explosionRadius = radius;
        explosionDamage = explDamage;
    }
    
    private void Explode()
    {
        // Show explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        // Damage all enemies in radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyController enemy = hitCollider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    // Calculate damage falloff based on distance
                    float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
                    float damageFactor = 1 - (distance / explosionRadius);
                    enemy.TakeDamage(explosionDamage * damageFactor);
                }
            }
        }
        
        Destroy(gameObject);
    }
}