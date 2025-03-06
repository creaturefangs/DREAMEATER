using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 30;

    [Header("Audio")]
    public AudioSource audioSource; // Audio source component
    public AudioClip hitSound; // Sound when hit
    public AudioClip deathSound; // Sound when enemy dies

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage!");

        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");

        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Destroy enemy after a short delay to allow the death sound to play
        Destroy(gameObject, 0.5f);
    }
}
