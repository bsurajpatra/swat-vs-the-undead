using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Spawning")]
    public GameObject zombiePrefab;
    public int zombieCount = 20; // Default / fallback
    public Vector3 spawnAreaSize = new Vector3(40, 0, 40);

    void Start()
    {
        if (zombiePrefab == null)
        {
            Debug.LogError("❌ Zombie prefab not assigned!");
            return;
        }

        // Get zombie count from LevelSelection if available
        if (LevelSelection.SelectedZombieCount > 0)
        {
            zombieCount = LevelSelection.SelectedZombieCount;
            Debug.Log($"📊 Using selected level config: {zombieCount} zombies");
        }

        SpawnZombies();

        // ✅ Inform ZombieManager about total zombies spawned
        if (ZombieManager.instance != null)
        {
            ZombieManager.instance.SetTotalZombies(zombieCount);
            Debug.Log($"🧟 Total zombies set to: {zombieCount}");
        }
        else
        {
            Debug.LogWarning("⚠️ ZombieManager not found in scene. Add one to track kills!");
        }

        // ✅ Initialize KillCounter with total zombie count
        if (KillCounter.instance != null)
        {
            KillCounter.instance.SetTotalZombies(zombieCount);
            Debug.Log($"📊 KillCounter initialized with {zombieCount} zombies");
        }
        else
        {
            Debug.LogWarning("⚠️ KillCounter not found in scene. Add one to display zombie count!");
        }
    }

    void SpawnZombies()
    {
        for (int i = 0; i < zombieCount; i++)
        {
            // Generate a random position within the spawn area
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            ) + transform.position;

            // Slightly raise the Y position (in case ground is not exactly at 0)
            randomPos.y = 0.1f;

            // Random rotation (so zombies face different directions)
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            Instantiate(zombiePrefab, randomPos, randomRot);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
