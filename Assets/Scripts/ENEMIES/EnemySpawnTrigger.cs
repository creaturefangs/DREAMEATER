using UnityEngine;
using System.Collections;

public class EnemySpawnTrigger : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] string playerTag = "Player";

    [Header("Spawning Settings")]
    public GameObject[] enemyPrefabs;
    public int enemiesPerWave = 3;
    public int numberOfWaves = 1;
    public float secondsBetweenWaves = 3f;

    [Header("Spawn Positioning")]
    public Transform[] spawnPoints; // Optional
    public bool useSpawnArea = true; // Toggle random area spawning

    [Header("Spawn Flair")]
    public float enemyStaggerTime = 0.25f;
    public float randomOffsetRange = 0.5f;
    public AudioClip waveStartSound;
    public AudioSource audioSource;

    bool hasTriggered = false;
    Collider2D spawnAreaCollider;

    private void Awake()
    {
        // Get the collider attached to this object (for random area spawn)
        spawnAreaCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
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
            if (waveStartSound != null && audioSource != null)
                audioSource.PlayOneShot(waveStartSound);

            yield return StartCoroutine(SpawnEnemiesStaggered());

            if (wave < numberOfWaves - 1)
                yield return new WaitForSeconds(secondsBetweenWaves);
        }
    }

    private IEnumerator SpawnEnemiesStaggered()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnSingleEnemy();
            yield return new WaitForSeconds(enemyStaggerTime);
        }
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Vector3 spawnPos;

        if (useSpawnArea && spawnAreaCollider != null)
        {
            spawnPos = GetRandomPointInsideArea();
        }
        else if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            spawnPos = transform.position;
        }

        spawnPos.x += Random.Range(-randomOffsetRange, randomOffsetRange);
        spawnPos.y += Random.Range(-randomOffsetRange, randomOffsetRange);

        Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
    }

    private Vector3 GetRandomPointInsideArea()
    {
        Bounds b = spawnAreaCollider.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float y = Random.Range(b.min.y, b.max.y);

        return new Vector3(x, y, transform.position.z);
    }
}
