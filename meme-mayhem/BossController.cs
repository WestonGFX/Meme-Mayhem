    // Properties
    public float CurrentHealth { get; private set; }
    public float CurrentHealthPercent => CurrentHealth / maxHealth;
    public int CurrentPhase { get; private set; } = 1;
    
    // Private variables
    private Transform player;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isDead = false;
    private float lastAttackTime;
    
    [System.Serializable]
    public class AttackPattern
    {
        public string attackName;
        public float cooldown = 2f;
        public int minPhaseRequired = 1;
        public GameObject attackPrefab;
        public bool requiresTargeting = true;
        public float attackDuration = 1f;
        public float damage = 1f;
        
        [HideInInspector]
        public float lastUsedTime = -999f;
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHealth = maxHealth;
        
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        // Initialize phase thresholds if not set
        if (phaseHealthThresholds == null || phaseHealthThresholds.Length == 0)
        {
            phaseHealthThresholds = new float[numberOfPhases - 1];
            for (int i = 0; i < numberOfPhases - 1; i++)
            {
                phaseHealthThresholds[i] = 1.0f - ((float)(i + 1) / numberOfPhases);
            }
        }
    }
    
    private void Start()
    {
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Let the game know we're here
        GameManager.Instance.ShowBossUI(this);
        
        // Start boss music or effects
        StartCoroutine(BossIntroSequence());
    }
    
    private IEnumerator BossIntroSequence()
    {
        // Play intro animation
        animator.SetTrigger("Intro");
        
        // Wait for animation to finish
        yield return new WaitForSeconds(2f);
        
        // Start attack patterns
        lastAttackTime = Time.time;
    }
    
    private void Update()
    {
        if (isDead || player == null)
            return;
            
        // Face the player
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        if (directionToPlayer.x < 0)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
        
        // Attack if not currently attacking and cooldown has passed
        if (!isAttacking && Time.time - lastAttackTime > timeBetweenAttacks)
        {
            StartCoroutine(PerformAttack());
        }
    }
    
    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        
        // Choose an attack pattern based on current phase
        AttackPattern selectedPattern = ChooseAttackPattern();
        
        if (selectedPattern != null)
        {
            // Play attack animation
            animator.SetTrigger("Attack");
            animator.SetInteger("AttackType", attackPatterns.IndexOf(selectedPattern));
            
            // Wait for animation to reach attack point
            yield return new WaitForSeconds(0.5f);
            
            // Execute attack
            if (selectedPattern.attackPrefab != null)
            {
                if (selectedPattern.requiresTargeting && player != null)
                {
                    // Direction to player
                    Vector2 direction = (player.position - transform.position).normalized;
                    
                    // Instantiate attack
                    GameObject attackObj = Instantiate(selectedPattern.attackPrefab, transform.position, Quaternion.identity);
                    BossAttack attack = attackObj.GetComponent<BossAttack>();
                    
                    if (attack != null)
                    {
                        attack.InitializeAttack(direction, selectedPattern.damage, selectedPattern.attackDuration);
                    }
                }
                else
                {
                    // Non-directional attack
                    GameObject attackObj = Instantiate(selectedPattern.attackPrefab, transform.position, Quaternion.identity);
                    BossAttack attack = attackObj.GetComponent<BossAttack>();
                    
                    if (attack != null)
                    {
                        attack.InitializeAttack(Vector2.zero, selectedPattern.damage, selectedPattern.attackDuration);
                    }
                }
            }
            
            // Mark this attack as used
            selectedPattern.lastUsedTime = Time.time;
            
            // Wait for attack duration
            yield return new WaitForSeconds(selectedPattern.attackDuration);
        }
        else
        {
            // No valid attack, just wait
            yield return new WaitForSeconds(1f);
        }
        
        // Reset attack state
        isAttacking = false;
        lastAttackTime = Time.time;
    }
    
    private AttackPattern ChooseAttackPattern()
    {
        // Filter patterns by current phase
        List<AttackPattern> validPatterns = new List<AttackPattern>();
        
        foreach (var pattern in attackPatterns)
        {
            if (pattern.minPhaseRequired <= CurrentPhase && 
                Time.time - pattern.lastUsedTime >= pattern.cooldown)
            {
                validPatterns.Add(pattern);
            }
        }
        
        if (validPatterns.Count > 0)
        {
            // Choose a random pattern from valid ones
            int randomIndex = Random.Range(0, validPatterns.Count);
            return validPatterns[randomIndex];
        }
        
        return null;
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead)
            return;
            
        CurrentHealth -= damage;
        
        // Show damage number
        GameManager.Instance.ShowDamageNumber(transform.position, damage);
        
        // Update boss health UI
        GameManager.Instance.UpdateBossHealthUI(this);
        
        // Check for phase change
        CheckPhaseChange();
        
        // Play hurt animation
        animator.SetTrigger("Hurt");
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    
    private void CheckPhaseChange()
    {
        for (int i = 0; i < phaseHealthThresholds.Length; i++)
        {
            int phaseToCheck = i + 2; // Phase 2, 3, etc.
            
            if (CurrentPhase < phaseToCheck && 
                CurrentHealthPercent <= phaseHealthThresholds[i])
            {
                ChangePhase(phaseToCheck);
                break;
            }
        }
    }
    
    private void ChangePhase(int newPhase)
    {
        CurrentPhase = newPhase;
        
        // Play phase change animation
        animator.SetTrigger("PhaseChange");
        animator.SetInteger("Phase", newPhase);
        
        // Show phase change effect
        if (phaseChangeEffects.Length >= newPhase - 1 && phaseChangeEffects[newPhase - 2] != null)
        {
            Instantiate(phaseChangeEffects[newPhase - 2], transform.position, Quaternion.identity);
        }
        
        // Maybe add some invulnerability during phase change
        StartCoroutine(PhaseChangeInvulnerability());
    }
    
    private IEnumerator PhaseChangeInvulnerability()
    {
        // Make invulnerable briefly
        GetComponent<Collider2D>().enabled = false;
        
        yield return new WaitForSeconds(1.5f);
        
        GetComponent<Collider2D>().enabled = true;
    }
    
    private void Die()
    {
        isDead = true;
        
        // Play death animation
        animator.SetTrigger("Die");
        
        // Disable collisions
        GetComponent<Collider2D>().enabled = false;
        
        // Stop movement
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        
        // Play effects
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        
        // Add score
        GameManager.Instance.AddScore(scoreValue);
        
        // Hide boss UI
        GameManager.Instance.HideBossUI();
        
        // Drop items or unlock next floor
        StartCoroutine(OnBossDefeated());
    }
    
    private IEnumerator OnBossDefeated()
    {
        // Wait for death animation
        yield return new WaitForSeconds(2f);
        
        // Create exit door
        CreateExitDoor();
        
        // Destroy the boss object
        Destroy(gameObject);
    }
    
    private void CreateExitDoor()
    {
        // Create a door to the next floor
        GameObject exitDoor = new GameObject("ExitDoor");
        exitDoor.transform.position = transform.position;
        
        // Add visuals and collider
        SpriteRenderer doorSprite = exitDoor.AddComponent<SpriteRenderer>();
        // Set door sprite
        
        BoxCollider2D doorCollider = exitDoor.AddComponent<BoxCollider2D>();
        doorCollider.isTrigger = true;
        
        // Add component to handle floor transition
        FloorExit exitScript = exitDoor.AddComponent<FloorExit>();
    }
}