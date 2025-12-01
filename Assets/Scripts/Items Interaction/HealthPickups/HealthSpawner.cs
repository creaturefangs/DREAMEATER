using UnityEngine;
using System.Collections;

public class HealthSpawner : MonoBehaviour
{
    public GameObject healthPickupPrefab;
    public int pickupsToSpawn = 3;
    public float launchForce = 5f;
    public float cooldownTime = 5f;

    public AudioClip spawnSound;
    [SerializeField] private AudioSource audioSource;
    private bool isOnCooldown = false;

    private void Awake()
    {
        
    }

    public void OnHit()
    {
        if (isOnCooldown) return;

        StartCoroutine(ShakeAndSpawn());
    }

    private IEnumerator ShakeAndSpawn()
    {
        isOnCooldown = true;

        // Shake the object
        Vector3 originalPos = transform.position;
        float shakeDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);
            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // Spawn pickups in arc
        for (int i = 0; i < pickupsToSpawn; i++)
        {
            GameObject pickup = Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = pickup.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 force = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 2f)).normalized * launchForce;
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }

        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}

