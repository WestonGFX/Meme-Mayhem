using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 3f;
    public AnimationCurve sizeCurve;
    public AnimationCurve speedCurve;
    public GameObject hitEffectPrefab;
    
    private Vector2 direction;
    private float damage;
    private float duration;
    private float startTime;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void InitializeAttack(Vector2 dir, float dmg, float dur)
    {
        direction = dir.normalized;
        damage = dmg;
        duration = dur;
        startTime = Time.time;
        
        // Set rotation to match direction if applicable
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    private void Update()
    {
        // Calculate normalized progress
        float progress = (Time.time - startTime) / duration;
        
        // Update size based on curve if provided
        if (sizeCurve != null)
        {
            float sizeMultiplier = sizeCurve.Evaluate(progress);
            transform.localScale = Vector3.one * sizeMultiplier;
        }
        
        // Update speed based on curve if provided
        if (speedCurve != null)
        {
            float speedMultiplier = speedCurve.Evaluate(progress);
            rb.velocity = direction * speed * speedMultiplier;
        }
        else
        {
            rb.velocity = direction * speed;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Deal damage to player
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage((int)damage);
            }
            
            // Spawn hit effect
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, collision.transform.position, Quaternion.identity);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            // Spawn hit effect on wall
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
    }
}