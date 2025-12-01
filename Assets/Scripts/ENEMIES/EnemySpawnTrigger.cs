using UnityEngine;
using System.Collections;

public class EnemySpawnTrigger : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] string playerTag = "Player";   // Player tag to detect

    [Header("Spawning Settings")]
    public GameObject[] enemyPrefabs;               // The enemy types to spawn
    public int enemiesPerWave = 3;                  // How many per wave
    public int numberOfWaves = 1;                   // Set to 1 if no waves needed
    public float secondsBetweenWaves = 3f;          // Delay between waves

    [Header("Spawn Positioning")]
    public Transform[] spawnPoints;                 // Optional spawn points
                                                    // If empty, spawns at trigger position

    bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true;
            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        for (int wave = 0; wave < numberOfWaves; wave++)
        {
            SpawnEnemies();

            if (wave < numberOfWaves - 1)
                yield return new WaitForSeconds(secondsBetweenWaves);
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            // Pick a random enemy from the list
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Pick a spawn point (or use trigger position)
            Vector3 spawnPos;

            if (spawnPoints != null && spawnPoints.Length > 0)
                spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            else
                spawnPos = transform.position;

            Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        }
    }
}
