using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] public GameObject[] enemyPrefabs;
    public float spawnInterval = 3f; 
    public int maxEnemies = 5;

    public float timer;
    private int currentEnemies = 0;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        int currentEnemies = GameObject.FindGameObjectsWithTag("Zombie").Length;

        if(timer <= 0 && currentEnemies < maxEnemies)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject chosenPrefab = enemyPrefabs[index];

        GameObject enemy = Instantiate(chosenPrefab, transform.position, Quaternion.identity);

        EnemyDeathTracker tracker = enemy.AddComponent<EnemyDeathTracker>();
        tracker.spawner = this;

        currentEnemies++;
    }

    public void EnemyDied()
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);
    }


}