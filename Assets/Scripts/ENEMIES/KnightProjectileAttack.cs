using UnityEngine;
using System.Collections;

public class KnightProjectileAttack : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int projectileCount = 3;
    public float timeBetweenShots = 1f;
    public float projectileSpeed = 3f;
    public float projectileLifetime = 5f;

    [Header("Player Reference")]
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void StartAttack()
    {
        StartCoroutine(FireProjectiles());
    }

    private IEnumerator FireProjectiles()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(MoveProjectile(projectile));
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private IEnumerator MoveProjectile(GameObject projectile)
    {
        SpriteRenderer sprite = projectile.GetComponent<SpriteRenderer>();
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        // Wait before moving
        yield return new WaitForSeconds(1f);

        // Move towards player
        Vector2 direction = (player.position - projectile.transform.position).normalized;
        rb.linearVelocity = direction * projectileSpeed;

        // Flash effect before self-destruct
        yield return new WaitForSeconds(projectileLifetime - 0.5f);
        for (int i = 0; i < 3; i++)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(projectile);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit! Deal damage here.");
            Destroy(gameObject);
        }
    }
}
