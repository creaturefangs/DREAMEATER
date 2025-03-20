using UnityEngine;
using System.Collections;

public class ViolenceEnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    private int currentPointIndex = 0;
    private bool isMoving = true;
    private bool isChasing = false;

    [Header("Spawn Settings")]
    public GameObject footstepPrefab;
    public Transform leftSpawn;
    public Transform rightSpawn;
    private bool spawnLeft = true;
    public float spawnDelay = 0.5f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip patrolSFX;
    public AudioClip footstepSFX;
    public AudioClip detectionSFX;

    [Header("Player Detection")]
    public Transform player;
    public float detectionRange = 5f;
    public CameraShake cameraShake;
    public float chaseSpeed = 3.5f; // Speed when chasing the player

    private void Start()
    {
        StartCoroutine(SpawnFootsteps());
        if (audioSource && patrolSFX)
            audioSource.PlayOneShot(patrolSFX);
    }

    private void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }
    }

    private void Patrol()
    {
        if (!isMoving || patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    private IEnumerator SpawnFootsteps()
    {
        while (true) // Footsteps continue whether patrolling or chasing
        {
            yield return new WaitForSeconds(spawnDelay);

            Transform spawnPoint = spawnLeft ? leftSpawn : rightSpawn;
            GameObject footstep = Instantiate(footstepPrefab, spawnPoint.position, Quaternion.identity);
            Destroy(footstep, 2f); // Destroy footstep prefab after 2 seconds

            if (audioSource && footstepSFX)
                audioSource.PlayOneShot(footstepSFX);

            spawnLeft = !spawnLeft;
        }
    }

    private void DetectPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            if (audioSource && detectionSFX)
                audioSource.PlayOneShot(detectionSFX);
            if (cameraShake)
                StartCoroutine(cameraShake.Shake(0.3f, 0.5f));

            isMoving = false;
            isChasing = true; // Start chasing
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        // Draw detection radius in scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
