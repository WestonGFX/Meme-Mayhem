using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject gameManagerPrefab;
    public GameObject uiManagerPrefab;
    public GameObject dungeonGeneratorPrefab;
    public GameObject emoteFusionSystemPrefab;
    public GameObject weaponSynergyManagerPrefab;
    
    private void Awake()
    {
        // Load player data
        PlayerData.LoadData();
        
        // Make sure we have all the necessary systems
        EnsureSystemExists<GameManager>(gameManagerPrefab);
        EnsureSystemExists<DungeonGenerator>(dungeonGeneratorPrefab);
        EnsureSystemExists<EmoteFusionSystem>(emoteFusionSystemPrefab);
        EnsureSystemExists<WeaponSynergyManager>(weaponSynergyManagerPrefab);
        
        // Initialize UI
        if (uiManagerPrefab != null && FindObjectOfType<UIManager>() == null)
        {
            Instantiate(uiManagerPrefab);
        }
    }
    
    private T EnsureSystemExists<T>(GameObject prefab) where T : MonoBehaviour
    {
        T existingSystem = FindObjectOfType<T>();
        
        if (existingSystem == null && prefab != null)
        {
            GameObject systemObj = Instantiate(prefab);
            return systemObj.GetComponent<T>();
        }
        
        return existingSystem;
    }
}