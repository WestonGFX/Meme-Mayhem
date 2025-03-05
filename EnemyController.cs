using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Basic Settings")]
    public string enemyName;
    public float maxHealth = 10f;
    public float moveSpeed = 3f;
    public int damageOnContact = 1;
    public int scoreValue = 100;
    
    [Header("Attack Settings")]
    public bool canAttackFromDistance = false;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public GameObject projectilePrefab;
    public float projectileDamage = 1f;
    
    [Header("Drop Settings")]
    public GameObject[] possibleDrops;
    public float dropChance = 0.3f;
    
    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    // State variables
    private float currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private bool canAttack = true;
    private EnemyState currentState = EnemyState.Idle;
    
    // AI behavior variables
    private float distanceToPlayer;
    private Vector2 directionToPlayer;
    
    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Fleeing,
        Stunned
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    private void Update()
    {
        if (player == null)
            return;
            
        // Calculate distance to player
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        directionToPlayer = (player.position - transform.position).normalized;
        
        // Update sprite direction
        if (directionToPlayer.x < 0)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
        
        // Update state based on distance and other factors
        UpdateState();
        
        // Execute behavior based on state
        ExecuteStateBehavior();
    }
    
    private void UpdateState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // If player is within detection range, start chasing
                if (distanceToPlayer < 10f)
                {
                    currentState = EnemyState.Chasing;
                }
                break;
                
            case EnemyState.Chasing:
                // If player is in attack range, switch to attacking
                if (canAttackFromDistance && distanceToPlayer < attackRange && canAttack)
                {
                    currentState = EnemyState.Attacking;
                }
                // If player is too far, go back to idle
                else if (distanceToPlayer > 15f)
                {
                    currentState = EnemyState.Idle;
                }
                break;
                
            case EnemyState.Attacking:
                // After attack, go back to chasing
                if (!canAttack || distanceToPlayer > attackRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;
                
            case EnemyState.Fleeing:
                // If health is restored or player is far enough, return to chasing
                if (currentHealth > maxHealth * 0.3f || distanceToPlayer > 12f)
                {
                    currentState = EnemyState.Chasing;
                }
                break;
                
            case EnemyState.Stunned:
                // Handled by stun coroutine
                break;
        }
        
        // Special case: Low health causes fleeing for some enemies
        if (currentHealth < maxHealth * 0.3f && Random.value < 0.7f && currentState != EnemyState.Stunned)
        {
            currentState = EnemyState.Fleeing;
        }
    }
    
    private void ExecuteStateBehavior()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // Just stand or patrol slightly
                animator.SetBool("IsMoving", false);
                rb.velocity = Vector2.zero;
                break;
                
            case EnemyState.Chasing:
                // Move towards the player
                animator.SetBool("IsMoving", true);
                rb.velocity = directionToPlayer * moveSpeed;
                break;
                
            case EnemyState.Attacking:
                // Stop moving and attack
                animator.SetBool("IsMoving", false);
                rb.velocity = Vector2.zero;
                
                if (canAttack)
                {
                    Attack();
                }
                break;
                
            case EnemyState.Fleeing:
                // Move away from player
                animator.SetBool("IsMoving", true);
                rb.velocity = -directionToPlayer * moveSpeed * 1.2f;
                break;
                
            case EnemyState.Stunned:
                // Don't move while stunned
                animator.SetBool("IsMoving", false);
                rb.velocity = Vector2.zero;
                break;
        }
    }
    
    private void Attack()
    {
        // Start attack animation
        animator.SetTrigger("Attack");
        
        if (canAttackFromDistance && projectilePrefab != null)
        {
            // Ranged attack
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            
            if (projectile != null)
            {
                projectile.Initialize(directionToPlayer, projectileDamage);
            }
        }
        
        // Start cooldown
        canAttack = false;
        StartCoroutine(AttackCooldown());
    }
    
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // Play hurt animation
        animator.SetTrigger("Hurt");
        
        // Show damage number
        GameManager.Instance.ShowDamageNumber(transform.position, damage);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Stun(float duration)
    {
        // Stop current behavior and become stunned
        StopAllCoroutines();
        currentState = EnemyState.Stunned;
        
        // Start stun animation/effect
        animator.SetTrigger("Stun");
        
        // Start stun timer
        StartCoroutine(StunTimer(duration));
    }
    
    private IEnumerator StunTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentState = EnemyState.Chasing;
        canAttack = true;
    }
    
    private void Die()
    {
        // Play death animation
        animator.SetTrigger("Die");
        
        // Disable collisions and movement
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        
        // Add score
        GameManager.Instance.AddScore(scoreValue);
        
        // Chance to drop item
        if (Random.value < dropChance && possibleDrops.Length > 0)
        {
            int dropIndex = Random.Range(0, possibleDrops.Length);
            Instantiate(possibleDrops[dropIndex], transform.position, Quaternion.identity);
        }
        
        // Destroy after animation plays
        Destroy(gameObject, 1f);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to player on contact
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageOnContact);
            }
        }
    }
}