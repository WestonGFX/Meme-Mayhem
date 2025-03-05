    private int totalWaves;
    private int currentWave = 0;
    private int enemiesRemainingInWave = 0;
    private List<List<GameObject>> waveEnemyPrefabs = new List<List<GameObject>>();
    private Vector2Int roomDimensions;
    private GameObject rewardPrefab;
    private bool challengeCompleted = false;
    
    public void SetupWaves(int waves, List<GameObject> enemyPrefabs, Vector2Int dimensions)
    {
        totalWaves = waves;
        roomDimensions = dimensions;
        
        // Create different enemy compositions for each wave
        for (int i = 0; i < totalWaves; i++)
        {
            List<GameObject> waveEnemies = new List<GameObject>();
            
            // Add increasing number of enemies per wave
            int enemyCount = 3 + i * 2;
            
            for (int j = 0; j < enemyCount; j++)
            {
                // Add random enemy from available prefabs
                int enemyIndex = Random.Range(0, enemyPrefabs.Count);
                waveEnemies.Add(enemyPrefabs[enemyIndex]);
            }
            
            waveEnemyPrefabs.Add(waveEnemies);
        }
    }
    
    public void SetReward(GameObject reward)
    {
        rewardPrefab = reward;
    }
    
    private void Start()
    {
        // Start the first wave
        StartWave(0);
    }
    
    private void Update()
    {
        // Check if current wave is complete
        if (currentWave < totalWaves && enemiesRemainingInWave <= 0)
        {
            // Start next wave
            currentWave++;
            
            if (currentWave < totalWaves)
            {
                StartWave(currentWave);
            }
            else
            {
                // All waves complete
                OnChallengeComplete();
            }
        }
    }
    
    private void StartWave(int waveIndex)
    {
        if (waveIndex >= waveEnemyPrefabs.Count)
            return;
            
        List<GameObject> enemiesForWave = waveEnemyPrefabs[waveIndex];
        enemiesRemainingInWave = enemiesForWave.Count;
        
        // Display wave UI
        ShowWaveUI(waveIndex + 1);
        
        // Spawn enemies
        for (int i = 0; i < enemiesForWave.Count; i++)
        {
            SpawnEnemy(enemiesForWave[i]);
        }
    }
    
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        // Calculate a random position within the room
        float x = Random.Range(-roomDimensions.x * 0.4f, roomDimensions.x * 0.4f);
        float y = Random.Range(-roomDimensions.y * 0.4f, roomDimensions.y * 0.4f);
        Vector2 spawnPos = new Vector2(x, y);
        
        // Spawn the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        // Subscribe to enemy death
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            // Use reflection or another way to register for death events
            // For prototype, we'll check remaining enemies in Update()
        }
    }
    
    private void OnEnemyDefeated()
    {
        enemiesRemainingInWave--;
    }
    
    private void OnChallengeComplete()
    {
        if (challengeCompleted)
            return;
            
        challengeCompleted = true;
        
        // Show challenge complete UI
        ShowChallengeCompleteUI();
        
        // Spawn reward in center of room
        if (rewardPrefab != null)
        {
            Instantiate(rewardPrefab, Vector3.zero, Quaternion.identity);
        }
    }
    
    private void ShowWaveUI(int waveNumber)
    {
        // Create and show wave UI
        GameObject waveText = new GameObject("WaveText");
        waveText.transform.position = new Vector3(0, 3, 0);
        
        // Add UI text component (would be implemented in full game)
        UnityEngine.UI.Text text = waveText.AddComponent<UnityEngine.UI.Text>();
        if (text != null)
        {
            text.text = "Wave " + waveNumber + " of " + totalWaves;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 24;
            // Additional text setup here
        }
        
        // Destroy after showing
        Destroy(waveText, 2f);
    }
    
    private void ShowChallengeCompleteUI()
    {
        // Create and show completion UI
        GameObject completeText = new GameObject("CompleteText");
        completeText.transform.position = new Vector3(0, 3, 0);
        
        // Add UI text component (would be implemented in full game)
        UnityEngine.UI.Text text = completeText.AddComponent<UnityEngine.UI.Text>();
        if (text != null)
        {
            text.text = "Challenge Complete!";
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 28;
            // Additional text setup here
        }
        
        // Destroy after showing
        Destroy(completeText, 3f);
    }
}