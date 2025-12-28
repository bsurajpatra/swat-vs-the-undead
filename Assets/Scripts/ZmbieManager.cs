using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieManager : MonoBehaviour
{
    [Header("Zombie Management")]
    [HideInInspector] public int totalZombies;   // set dynamically from spawner
    private int zombiesKilled = 0;

    

    public static ZombieManager instance; // Singleton reference

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetTotalZombies(int count)
    {
        totalZombies = count;
        Debug.Log($"🧟 Total zombies set to: {totalZombies}");
    }

    public void ZombieKilled()
    {
        zombiesKilled++;
        Debug.Log($"🧟 Zombie killed! Count: {zombiesKilled}/{totalZombies}");

        if (zombiesKilled >= totalZombies && totalZombies > 0)
        {
            Debug.Log("🏆 All zombies eliminated! Loading Victory Scene...");
            SceneManager.LoadScene(3);
        }
    }
}
