using UnityEngine;
using System.Collections;

public class HealthPickupDespawn : MonoBehaviour
{
    [Header("Health Settings")]
    public int healAmount = 20; // Amount of health restored
    public float floatDuration = 2f; // how long it floats before stopping
    public float despawnTime = 10f;      // Total lifetime before disappearing

    [Header("Despawn Effects")]
    public AudioClip despawnSound;
    public float despawnFlashTime = 1f; // How long before despawn the flash plays
    public AudioSource itemAudioSource; // Reference to external AudioSource (like on "Items" GameObject)

    [Header("Flash Settings")]
    public SpriteRenderer spriteRenderer;      // Assign in Inspector
    public Color flashColor = Color.black;
    public float flashDuration = 0.2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Start the coroutine to stop it after floatDuration
        StartCoroutine(FloatAndStop());
        StartCoroutine(DespawnAfterTime());
    }

    private IEnumerator FloatAndStop()
    {
        yield return new WaitForSeconds(floatDuration);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(despawnTime - despawnFlashTime);

        // Play sound from external AudioSource
        if (itemAudioSource != null && despawnSound != null)
        {
            itemAudioSource.PlayOneShot(despawnSound);
        }


        // Flash effect via SpriteRenderer
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;

            yield return new WaitForSeconds(flashDuration);

            spriteRenderer.color = originalColor;
        }

        yield return new WaitForSeconds(despawnFlashTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthBarManager healthManager = other.GetComponent<HealthBarManager>();

        if (healthManager != null)
        {
            healthManager.Heal(healAmount);
            Destroy(gameObject); // Remove the pickup after use
        }
    }
}
