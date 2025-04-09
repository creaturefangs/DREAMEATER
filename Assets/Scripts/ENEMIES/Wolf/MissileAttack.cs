using UnityEngine;

public class MissileAttack : MonoBehaviour
{
    public float missileSpeed = 10f; // Speed of the missile
    public float missileLifeTime = 5f; // Lifetime before the missile disappears
    public GameObject explosionPrefab; // Optional: Explosion effect on hit or time out
    HealthBarManager healthBarManager;

    private Vector3 targetPosition;
    private bool hasTarget = false;

    void Start()
    {
        // Find the player's position when the missile is fired
        if (PlayerMovement.instance != null) // Assuming you have a reference to the player instance
        {
            targetPosition = PlayerMovement.instance.transform.position;
            hasTarget = true;
        }

        // Start the missile movement
        if (!hasTarget)
        {
            Destroy(gameObject); // If no player, destroy missile immediately
        }

        // Destroy the missile after a set amount of time
        Destroy(gameObject, missileLifeTime);
    }

    void Update()
    {
        if (hasTarget)
        {
            // Move towards the target (player's position)
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * missileSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Optional: Explosion effect or damage logic on collision
        if (other.CompareTag("Player"))
        {
            // You can add damage logic here
            healthBarManager.TakeDamage(5);

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // Destroy missile on collision with player
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            // Optional: Missiles can hit other enemies
            Destroy(gameObject);
        }
    }
}
