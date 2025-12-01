using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 10; // Set attack damage

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Debug.Log("Trigger entered with: " + other.name);

        if (other.CompareTag("Breakable"))
        {
            Debug.Log("Breakable object detected.");
            HealthSpawner spawner = other.GetComponent<HealthSpawner>();
            if (spawner != null)
            {
                Debug.Log("HealthSpawner found, calling OnHit.");
                spawner.OnHit();
            }
        }
    }
}

