using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillCounter : MonoBehaviour
{
    // Make this globally accessible (so zombies can update it easily)
    public static KillCounter instance;

    private int remainingZombies = 0;  // tracks remaining zombies (counts down)
    private float timeRemaining = 0f;   // timer countdown
    private bool timerActive = false;
    private bool gameEnded = false;     // prevent multiple scene loads

    [Header("UI Reference")]
    public TextMeshProUGUI killCounterText;  // reference to the TMP text UI
    public TextMeshProUGUI timerText;        // reference to the timer text UI (displayed below counter)

    [Header("Timer Settings")]
    public float timerDuration = 180f; // fallback if no selection
    
    private float gameStartTime;
    private int totalZombiesAtStart;

    void Awake()
    {
        // store this instance globally
        instance = this;
    }

    void Start()
    {
        // Try to find timer text automatically if not assigned
        if (timerText == null)
        {
            // Search for a TextMeshProUGUI with "Timer" in the name
            TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in allTexts)
            {
                if (text.name.Contains("Timer") || text.name.Contains("timer"))
                {
                    timerText = text;
                    Debug.Log($"✅ Found timer text: {text.name}");
                    break;
                }
            }

            // If still not found, try to find it as a sibling of killCounterText
            if (timerText == null && killCounterText != null)
            {
                Transform parent = killCounterText.transform.parent;
                if (parent != null)
                {
                    TextMeshProUGUI[] siblings = parent.GetComponentsInChildren<TextMeshProUGUI>();
                    foreach (TextMeshProUGUI text in siblings)
                    {
                        if (text != killCounterText && (text.name.Contains("Timer") || text.name.Contains("timer")))
                        {
                            timerText = text;
                            Debug.Log($"✅ Found timer text as sibling: {text.name}");
                            break;
                        }
                    }
                }
            }

            if (timerText == null)
            {
                Debug.LogWarning("⚠️ Timer text not found! Please create a TextMeshProUGUI element named 'TimerText' or assign it manually in the Inspector.");
            }
        }
        
        // Ensure timer text is visible
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.enabled = true;
        }
    }

    void Update()
    {
        // Update timer countdown
        if (timerActive && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            // Game over if timer reaches 0
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerActive = false;
                OnTimerExpired();
            }
        }
    }

    // Initialize the counter with total zombie count
    public void SetTotalZombies(int totalCount)
    {
        remainingZombies = totalCount;
        totalZombiesAtStart = totalCount;
        gameStartTime = Time.time;
        
        // Get timer from LevelSelection if available
        if (LevelSelection.SelectedTimeLimit > 0)
        {
            timeRemaining = LevelSelection.SelectedTimeLimit;
            Debug.Log($"⏱️ Using selected level timer: {timeRemaining}s");
        }
        else
        {
            timeRemaining = timerDuration;
        }
        
        timerActive = true;
        gameEnded = false;
        
        UpdateUI();
        UpdateTimerUI();
    }

    // called when a zombie dies (decrements the counter)
    public void AddKill()
    {
        if (remainingZombies > 0 && !gameEnded)
        {
            remainingZombies--;
            UpdateUI();
            
            // Check if all zombies are killed within time limit
            if (remainingZombies == 0 && timeRemaining > 0)
            {
                OnAllZombiesKilled();
            }
        }
    }

    // refresh the on-screen text
    void UpdateUI()
    {
        if (killCounterText != null)
        {
            // Only update counter text if timer text exists separately
            // Otherwise, UpdateTimerUI will handle the combined display
            if (timerText != null)
            {
                killCounterText.text = "Remaining: " + remainingZombies;
            }
            // If timerText is null, UpdateTimerUI will update both in one text
        }
    }

    // Update timer display
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
            
            // Ensure it's visible
            if (!timerText.gameObject.activeInHierarchy)
            {
                timerText.gameObject.SetActive(true);
            }
        }
        else
        {
            // Fallback: Display timer in the counter text if timer text is not available
            if (killCounterText != null && timerActive)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                killCounterText.text = $"Remaining: {remainingZombies}\nTime: {minutes:00}:{seconds:00}";
            }
        }
    }

    // Handle timer expiration (game over - load scene 2)
    void OnTimerExpired()
    {
        if (gameEnded) return; // Prevent multiple calls
        
        gameEnded = true;
        timerActive = false;
        
        // Save record (failed)
        SaveGameRecord(false);
        
        Debug.Log("⏰ Time's up! Game Over! Loading scene 2...");
        SceneManager.LoadScene(2); // Game over scene
    }

    // Handle victory condition (all zombies killed within time - load scene 3)
    void OnAllZombiesKilled()
    {
        if (gameEnded) return; // Prevent multiple calls
        
        gameEnded = true;
        timerActive = false;
        
        // Save record (success)
        SaveGameRecord(true);
        
        Debug.Log("🏆 All zombies killed within time limit! Victory! Loading scene 3...");
        SceneManager.LoadScene(3); // Victory scene
    }
    
    // Save game record
    void SaveGameRecord(bool completed)
    {
        if (GameRecordManager.instance == null) return;
        
        float timeTaken = Time.time - gameStartTime;
        int zombiesKilled = totalZombiesAtStart - remainingZombies;
        
        int currentLevel = LevelSelection.SelectedLevel > 0
            ? LevelSelection.SelectedLevel
            : 1;
        
        GameRecordManager.instance.SaveRecord(currentLevel, completed, timeTaken, zombiesKilled);
    }
}
