using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    
    [Header("UI References")]
    public GameObject mainMenuUI;
    public GameObject gameplayUI;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject victoryUI;
    public GameObject fusionMenuUI;
    public Text scoreText;
    public Text floorText;
    public Image healthBar;
    public GameObject bossHealthUI;
    public Image bossHealthBar;
    public Text bossNameText;
    public GameObject damageTextPrefab;
    
    [Header("Room Generation")]
    public DungeonGenerator dungeonGenerator;
    
    // Game state
    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isVictory = false;
    private int currentScore = 0;
    private int currentFloor = 1;
    private PlayerController playerController;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize UI
        if (gameplayUI) gameplayUI.SetActive(false);
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (victoryUI) victoryUI.SetActive(false);
        if (fusionMenuUI) fusionMenuUI.SetActive(false);
        if (bossHealthUI) bossHealthUI.SetActive(false);
    }
    
    public void StartGame()
    {
        // Hide main menu
        if (mainMenuUI) mainMenuUI.SetActive(false);
        if (gameplayUI) gameplayUI.SetActive(true);
        
        // Reset state
        isPaused = false;
        isGameOver = false;
        isVictory = false;
        currentScore = 0;
        currentFloor = 1;
        Time.timeScale = 1f;
        
        // Update UI
        UpdateScoreUI();
        UpdateFloorUI();
        
        // Generate dungeon
        dungeonGenerator.GenerateDungeon();
        
        // Spawn player at starting room
        Vector2 startPos = dungeonGenerator.GetStartingRoomPosition();
        if (playerSpawnPoint == null)
        {
            GameObject spawnPoint = new GameObject("PlayerSpawnPoint");
            spawnPoint.transform.position = startPos;
            playerSpawnPoint = spawnPoint.transform;
        }
        else
        {
            playerSpawnPoint.position = startPos;
        }
        
        SpawnPlayer();
    }
    
    private void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            GameObject playerObj = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            playerController = playerObj.GetComponent<PlayerController>();
            
            // Initialize player
            if (playerController != null)
            {
                UpdateHealthUI();
            }
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (fusionMenuUI) fusionMenuUI.SetActive(false);
    }
    
    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        
        if (gameOverUI) gameOverUI.SetActive(true);
    }
    
    public void OnGameCompleted()
    {
        isVictory = true;
        Time.timeScale = 0f;
        
        if (victoryUI) victoryUI.SetActive(true);
    }
    
    public void RestartGame()
    {
        // Reload the scene to restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitToMainMenu()
    {
        if (gameplayUI) gameplayUI.SetActive(false);
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (victoryUI) victoryUI.SetActive(false);
        if (fusionMenuUI) fusionMenuUI.SetActive(false);
        
        if (mainMenuUI) mainMenuUI.SetActive(true);
        
        // Destroy any existing dungeon
        if (dungeonGenerator != null)
        {
            dungeonGenerator.GetComponent<DungeonGenerator>().ClearExistingDungeon();
        }
        
        // Destroy player
        if (playerController != null)
        {
            Destroy(playerController.gameObject);
        }
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    public void OpenFusionMenu()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (fusionMenuUI) fusionMenuUI.SetActive(true);
    }
    
    public void OnFloorGenerated(int floor)
    {
        currentFloor = floor;
        UpdateFloorUI();
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }
    
    public void UpdateHealthUI()
    {
        if (playerController != null && healthBar != null)
        {
            float healthPercent = (float)playerController.currentHealth / playerController.maxHealth;
            healthBar.fillAmount = healthPercent;
        }
    }
    
    public void ShowBossUI(BossController boss)
    {
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(true);
            
            if (bossNameText != null)
            {
                bossNameText.text = boss.bossName;
            }
            
            UpdateBossHealthUI(boss);
        }
    }
    
    public void UpdateBossHealthUI(BossController boss)
    {
        if (bossHealthBar != null && boss != null)
        {
            bossHealthBar.fillAmount = boss.CurrentHealthPercent;
        }
    }
    
    public void HideBossUI()
    {
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(false);
        }
    }
    
    public void ShowDamageNumber(Vector3 position, float damage)
    {
        if (damageTextPrefab != null)
        {
            GameObject damageObj = Instantiate(damageTextPrefab, position, Quaternion.identity);
            Text damageText = damageObj.GetComponent<Text>();
            
            if (damageText != null)
            {
                damageText.text = damage.ToString("0.0");
                
                // Destroy after animation
                Destroy(damageObj, 1f);
            }
        }
    }
    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }
    
    private void UpdateFloorUI()
    {
        if (floorText != null)
        {
            floorText.text = "Floor: " + currentFloor.ToString();
        }
    }
    
    private void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}