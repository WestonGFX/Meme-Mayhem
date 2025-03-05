using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashCooldown = 1f;
    public float dashDuration = 0.2f;

    [Header("Combat Settings")]
    public Transform firePoint;
    public GameObject defaultProjectilePrefab;
    public float fireRate = 0.2f;
    public float damage = 1f;
    
    [Header("Health Settings")]
    public int maxHealth = 6;
    public int currentHealth;
    
    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject dashEffect;
    
    // Private variables
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 aimDirection;
    private float lastFireTime;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTimer;
    private Camera mainCamera;
    
    // Weapon and item variables
    public WeaponBase currentWeapon;
    public List<ItemBase> passiveItems = new List<ItemBase>();
    public ItemBase activeItem;
    
    // Emote fusion system
    public List<EmoteBase> collectedEmotes = new List<EmoteBase>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        currentHealth = maxHealth;
    }
    
    private void Update()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
            return;
        }
        
        // Get movement input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
        
        // Aim direction (mouse for keyboard/mouse, right stick for controller)
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y).normalized;
        }
        else if (Mathf.Abs(Input.GetAxis("RHorizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("RVertical")) > 0.1f)
        {
            aimDirection = new Vector2(Input.GetAxis("RHorizontal"), Input.GetAxis("RVertical")).normalized;
        }
        
        // Flip sprite based on aim direction
        if (aimDirection.x < 0)
            spriteRenderer.flipX = true;
        else if (aimDirection.x > 0)
            spriteRenderer.flipX = false;
        
        // Shooting input
        if (Input.GetMouseButton(0) || Input.GetButton("Fire1"))
        {
            TryShoot();
        }
        
        // Dash input
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire2")) && canDash)
        {
            StartDash();
        }
        
        // Active item input
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Fire3"))
        {
            UseActiveItem();
        }
        
        // Emote fusion input
        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire4"))
        {
            OpenEmoteFusionMenu();
        }
    }
    
    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.velocity = aimDirection * dashSpeed;
            return;
        }
        
        rb.velocity = moveDirection * moveSpeed;
    }
    
    private void TryShoot()
    {
        if (Time.time - lastFireTime < fireRate)
            return;
            
        if (currentWeapon != null)
        {
            currentWeapon.Fire(firePoint.position, aimDirection);
        }
        else
        {
            // Default weapon if none equipped
            GameObject projectile = Instantiate(defaultProjectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(aimDirection, damage);
            }
        }
        
        lastFireTime = Time.time;
        
        // Play shoot animation
        animator.SetTrigger("Shoot");
    }
    
    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimer = dashDuration;
        
        // Instantiate dash effect
        if (dashEffect != null)
        {
            Instantiate(dashEffect, transform.position, Quaternion.identity);
        }
        
        // Play dash animation
        animator.SetTrigger("Dash");
        
        // Start cooldown coroutine
        StartCoroutine(DashCooldown());
    }
    
    private System.Collections.IEnumerator DashCooldown()
    {
        yield return new System.Collections.WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    private void UseActiveItem()
    {
        if (activeItem != null)
        {
            activeItem.Use(this);
        }
    }
    
    private void OpenEmoteFusionMenu()
    {
        // Pause game and open fusion menu
        GameManager.Instance.OpenFusionMenu();
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Play hurt animation
        animator.SetTrigger("Hurt");
        
        if (currentHealth <= 0)
        {
            Die();
        }
        
        // Notify UI to update
        GameManager.Instance.UpdateHealthUI();
    }
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        
        // Notify UI to update
        GameManager.Instance.UpdateHealthUI();
    }
    
    private void Die()
    {
        // Play death animation
        animator.SetTrigger("Die");
        
        // Disable controls
        enabled = false;
        rb.velocity = Vector2.zero;
        
        // Game over
        GameManager.Instance.GameOver();
    }
    
    public void AddEmote(EmoteBase emote)
    {
        collectedEmotes.Add(emote);
    }
    
    public void ApplyWeaponSynergies()
    {
        // Apply synergies between weapon and passive items
        if (currentWeapon != null)
        {
            foreach (var item in passiveItems)
            {
                WeaponSynergyManager.Instance.ApplySynergy(currentWeapon, item);
            }
        }
    }
}