using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 5f; // Time between checks/spawns (in seconds)
    public int maxEnemies = 5; // Max number of enemies active at once

    private int currentEnemyCount = 0;
    private Transform[] spawnPoints; // Locations where enemies can appear

    private void Start()
    {
        // Get all child transforms. Index 0 is the spawner object itself.
        spawnPoints = GetComponentsInChildren<Transform>();

        // Start the continuous spawning loop
        StartCoroutine(SpawnEnemiesRoutine());
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (true) // Loop indefinitely while the game is running
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned to the spawner!");
            return;
        }

        // --- NEW SAFETY CHECK ---
        // We require at least two transforms (the parent + one child spawn point)
        if (spawnPoints.Length <= 1)
        {
            Debug.LogWarning("Enemy Spawner has no valid child spawn points! Please add some empty GameObjects as children.");
            return;
        }
        // -------------------------

        // Pick a random spawn point. Range is from 1 (inclusive) up to Length (exclusive).
        // This safely skips index 0 (the spawner parent).
        int randomIndex = Random.Range(1, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Instantiate the enemy at the chosen position/rotation
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        currentEnemyCount++;

        // Add the tracker component to the new enemy
        EnemyDeathTracker tracker = newEnemy.AddComponent<EnemyDeathTracker>();
        tracker.spawner = this;

        Debug.Log("Spawned a new enemy. Current total: " + currentEnemyCount);
    }
    
    // Called by the EnemyDeathTracker when an enemy dies
    public void EnemyDied()
    {
        currentEnemyCount--;
    }
}