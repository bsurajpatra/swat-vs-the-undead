using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int zombieCount;
    public float timeLimit; // in seconds
    
    public LevelData(int level, int zombies, float time)
    {
        levelNumber = level;
        zombieCount = zombies;
        timeLimit = time;
    }
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    
    [Header("Level Configuration")]
    public LevelData[] levels = new LevelData[]
    {
        new LevelData(1, 20, 180f),  // Level 1: 20 zombies, 3 minutes
        new LevelData(2, 30, 240f), // Level 2: 30 zombies, 4 minutes
        new LevelData(3, 40, 300f)  // Level 3: 40 zombies, 5 minutes
    };
    
    private int currentLevel = 1;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Load current level from PlayerPrefs
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        // Ensure current level is within valid range
        if (currentLevel < 1 || currentLevel > levels.Length)
        {
            currentLevel = 1;
        }
    }
    
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public LevelData GetCurrentLevelData()
    {
        if (currentLevel >= 1 && currentLevel <= levels.Length)
        {
            return levels[currentLevel - 1];
        }
        return levels[0]; // Default to level 1
    }
    
    public void SetCurrentLevel(int level)
    {
        if (level >= 1 && level <= levels.Length)
        {
            currentLevel = level;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            PlayerPrefs.Save();
        }
    }
    
    public void NextLevel()
    {
        if (currentLevel < levels.Length)
        {
            currentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            PlayerPrefs.Save();
        }
    }
    
    public void ResetToLevel1()
    {
        currentLevel = 1;
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
    }
    
    public int GetMaxLevel()
    {
        return levels.Length;
    }
    
    public bool IsLastLevel()
    {
        return currentLevel >= levels.Length;
    }
}

