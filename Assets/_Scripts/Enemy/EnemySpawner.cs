using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private int totalToSpawn = 20;
    [SerializeField] private float spawnRadius = 3f;

    [Header("Key Drop")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Transform keyDropPoint;
    [SerializeField] private string keySearchTag = "Key";

    private float timer;
    private int aliveCount = 0;
    private int spawnedCount = 0;
    private bool keyDropped = false;
    private static bool anyKeyDropped = false;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        if (spawnedCount >= totalToSpawn) return;

        timer -= Time.deltaTime;

        if(timer <= 0f)
        {
            TrySpawn();
            timer = spawnInterval;
        }
    }

    private void TrySpawn()
    {
        if (aliveCount >= maxEnemies)
            return;

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return;

        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector3 spawnPos = GetRandomSpawnPosition();
        var enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        var tracker = enemy.GetComponent<EnemyDeathTracker>();
        if (tracker == null)
            tracker = enemy.AddComponent<EnemyDeathTracker>();

        tracker.Init(this);

        spawnedCount++;
        aliveCount++;
    }

    public void OnEnemyDied(Vector3 deathPosition)
    {
        aliveCount = Mathf.Max(0, aliveCount - 1);

        bool keyExistsInScene = anyKeyDropped || (!string.IsNullOrWhiteSpace(keySearchTag) && GameObject.FindWithTag(keySearchTag) != null);

        if(!keyDropped && !keyExistsInScene && spawnedCount >= totalToSpawn && aliveCount == 0 && keyPrefab != null)
        {
            Vector3 dropPos = keyDropPoint ? keyDropPoint.position : (deathPosition + Vector3.up * 0.2f);
            Instantiate(keyPrefab, dropPos, Quaternion.identity);
            keyDropped = true;
            anyKeyDropped = true;
        }
    }

    private void OnValidate()
    {
        spawnInterval = Mathf.Max(0.05f, spawnInterval);
        maxEnemies = Mathf.Max(0, maxEnemies);
        totalToSpawn = Mathf.Max(0, totalToSpawn);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        //Random point in radius
        Vector2 circle = Random.insideUnitCircle * spawnRadius;

        Vector3 pos = transform.position;
        pos += new Vector3(circle.x, 0f, circle.y);

        return pos;
    }


}