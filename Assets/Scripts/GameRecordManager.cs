using UnityEngine;
using System;

[System.Serializable]
public class GameRecord
{
    public int level;
    public bool completed;
    public float timeTaken;
    public int zombiesKilled;
    public DateTime dateTime;
    
    public GameRecord(int level, bool completed, float timeTaken, int zombiesKilled)
    {
        this.level = level;
        this.completed = completed;
        this.timeTaken = timeTaken;
        this.zombiesKilled = zombiesKilled;
        this.dateTime = DateTime.Now;
    }
}

public class GameRecordManager : MonoBehaviour
{
    public static GameRecordManager instance;
    
    private const string RECORD_PREFIX = "GameRecord_";
    private const string RECORD_COUNT_KEY = "GameRecordCount";
    
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
    
    public void SaveRecord(int level, bool completed, float timeTaken, int zombiesKilled)
    {
        GameRecord record = new GameRecord(level, completed, timeTaken, zombiesKilled);
        
        // Get current record count
        int recordCount = PlayerPrefs.GetInt(RECORD_COUNT_KEY, 0);
        recordCount++;
        
        // Save record data
        string recordKey = RECORD_PREFIX + recordCount;
        PlayerPrefs.SetInt(recordKey + "_Level", record.level);
        PlayerPrefs.SetInt(recordKey + "_Completed", record.completed ? 1 : 0);
        PlayerPrefs.SetFloat(recordKey + "_TimeTaken", record.timeTaken);
        PlayerPrefs.SetInt(recordKey + "_ZombiesKilled", record.zombiesKilled);
        PlayerPrefs.SetString(recordKey + "_DateTime", record.dateTime.ToString());
        
        // Update record count
        PlayerPrefs.SetInt(RECORD_COUNT_KEY, recordCount);
        PlayerPrefs.Save();
        
        Debug.Log($"📊 Record saved: Level {level}, Completed: {completed}, Time: {timeTaken:F2}s, Kills: {zombiesKilled}");
    }
    
    public GameRecord[] GetAllRecords()
    {
        int recordCount = PlayerPrefs.GetInt(RECORD_COUNT_KEY, 0);
        GameRecord[] records = new GameRecord[recordCount];
        
        for (int i = 0; i < recordCount; i++)
        {
            string recordKey = RECORD_PREFIX + (i + 1);
            
            int level = PlayerPrefs.GetInt(recordKey + "_Level", 0);
            bool completed = PlayerPrefs.GetInt(recordKey + "_Completed", 0) == 1;
            float timeTaken = PlayerPrefs.GetFloat(recordKey + "_TimeTaken", 0f);
            int zombiesKilled = PlayerPrefs.GetInt(recordKey + "_ZombiesKilled", 0);
            
            records[i] = new GameRecord(level, completed, timeTaken, zombiesKilled);
        }
        
        return records;
    }
    
    public GameRecord GetBestRecordForLevel(int level)
    {
        GameRecord[] allRecords = GetAllRecords();
        GameRecord bestRecord = null;
        
        foreach (GameRecord record in allRecords)
        {
            if (record.level == level && record.completed)
            {
                if (bestRecord == null || record.timeTaken < bestRecord.timeTaken)
                {
                    bestRecord = record;
                }
            }
        }
        
        return bestRecord;
    }
    
    public int GetTotalRecords()
    {
        return PlayerPrefs.GetInt(RECORD_COUNT_KEY, 0);
    }
    
    public void ClearAllRecords()
    {
        int recordCount = PlayerPrefs.GetInt(RECORD_COUNT_KEY, 0);
        
        for (int i = 0; i < recordCount; i++)
        {
            string recordKey = RECORD_PREFIX + (i + 1);
            PlayerPrefs.DeleteKey(recordKey + "_Level");
            PlayerPrefs.DeleteKey(recordKey + "_Completed");
            PlayerPrefs.DeleteKey(recordKey + "_TimeTaken");
            PlayerPrefs.DeleteKey(recordKey + "_ZombiesKilled");
            PlayerPrefs.DeleteKey(recordKey + "_DateTime");
        }
        
        PlayerPrefs.DeleteKey(RECORD_COUNT_KEY);
        PlayerPrefs.Save();
        
        Debug.Log("🗑️ All records cleared!");
    }
}

