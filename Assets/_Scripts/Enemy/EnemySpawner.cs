using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] public GameObject enemyPrefab;
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
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            EnemyDeathTracker tracker = enemy.AddComponent<EnemyDeathTracker>();
            tracker.spawner = this;

            currentEnemies++;
            timer = spawnInterval;
        }
    }

    public void EnemyDied()
    {
        currentEnemies--;
        if (currentEnemies < 0)
        {
            currentEnemies = 0;
        }
    }


}