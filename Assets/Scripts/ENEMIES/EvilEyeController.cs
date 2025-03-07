using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilEyeController : MonoBehaviour
{

    public float moveSpeed = 2f; // Speed of the enemy
    public float damageAmount = 10f; // Damage dealt to the player on collision

    private Transform player; // Reference to the player
    private HealthBarManager playerHealth; // Reference to the player's health script

    private bool canDamage = true; // Prevents rapid damage

    private void Start()
    {
        // Find the player object by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<HealthBarManager>();
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // Move towards the player
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the enemy collides with the player
        if (other.CompareTag("Player") && playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            StartCoroutine(DamageCooldown()); // Start cooldown
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false; // Disable damage
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        canDamage = true; // Re-enable damage
    }
}
