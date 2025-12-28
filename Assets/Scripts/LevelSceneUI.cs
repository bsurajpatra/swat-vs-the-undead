using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timingText;
    public TextMeshProUGUI targetText;
    
    [Header("Auto-find Settings")]
    public bool autoFindTexts = true;
    
    private LevelManager levelManager;
    
    void Start()
    {
        levelManager = LevelManager.instance;
        
        if (levelManager == null)
        {
            Debug.LogError("❌ LevelManager not found! Make sure LevelManager exists in the scene.");
            return;
        }
        
        // Auto-find text components if not assigned
        if (autoFindTexts)
        {
            AutoFindTextComponents();
        }
        
        UpdateUI();
    }
    
    void AutoFindTextComponents()
    {
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        
        foreach (TextMeshProUGUI text in allTexts)
        {
            string name = text.name.ToLower();
            
            if (levelText == null && (name.Contains("level") || name.Contains("lvl")))
            {
                levelText = text;
                Debug.Log($"✅ Found level text: {text.name}");
            }
            
            if (timingText == null && (name.Contains("timing") || name.Contains("time") || name.Contains("timer")))
            {
                timingText = text;
                Debug.Log($"✅ Found timing text: {text.name}");
            }
            
            if (targetText == null && (name.Contains("target") || name.Contains("zombie")))
            {
                targetText = text;
                Debug.Log($"✅ Found target text: {text.name}");
            }
        }
    }
    
    void UpdateUI()
    {
        if (levelManager == null) return;
        
        LevelData currentLevel = levelManager.GetCurrentLevelData();
        
        // Update Level Text
        if (levelText != null)
        {
            levelText.text = $"Level: {currentLevel.levelNumber}";
            levelText.gameObject.SetActive(true);
        }
        
        // Update Timing Text
        if (timingText != null)
        {
            int minutes = Mathf.FloorToInt(currentLevel.timeLimit / 60f);
            int seconds = Mathf.FloorToInt(currentLevel.timeLimit % 60f);
            timingText.text = $"Time Limit: {minutes:00}:{seconds:00}";
            timingText.gameObject.SetActive(true);
        }
        
        // Update Target Text
        if (targetText != null)
        {
            targetText.text = $"Target: {currentLevel.zombieCount} Zombies";
            targetText.gameObject.SetActive(true);
        }
        
        // Display best record if available
        if (GameRecordManager.instance != null)
        {
            GameRecord bestRecord = GameRecordManager.instance.GetBestRecordForLevel(currentLevel.levelNumber);
            if (bestRecord != null)
            {
                int bestMinutes = Mathf.FloorToInt(bestRecord.timeTaken / 60f);
                int bestSeconds = Mathf.FloorToInt(bestRecord.timeTaken % 60f);
                Debug.Log($"🏆 Best record for Level {currentLevel.levelNumber}: {bestMinutes:00}:{bestSeconds:00}");
            }
        }
    }
    

    // Public method to refresh UI (can be called from other scripts)
    public void RefreshUI()
    {
        UpdateUI();
    }

    // --- Level selection buttons ---
    public void OnLevelButton(int level)
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.SetCurrentLevel(level);
        }

        // Load the main gameplay scene (SampleScene)
        SceneManager.LoadScene("SampleScene");
    }

    // Helpers for assigning directly in the Button OnClick without parameters
    public void OnLevel1Button() => OnLevelButton(1);
    public void OnLevel2Button() => OnLevelButton(2);
    public void OnLevel3Button() => OnLevelButton(3);
}
