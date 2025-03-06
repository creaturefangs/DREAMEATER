using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackPrefab; // Prefab for the attack effect (e.g., sword slash)
    public Transform attackSpawnPoint; // Center point for spawning attack
    public float attackRange = 1.5f; // Distance where the attack object appears
    public int attackDamage = 10; // Damage dealt to enemies
    public float attackDuration = 0.3f; // How long the attack lasts

    [Header("Audio")]
    public AudioSource audioSource; // Audio source for attack sound
    public AudioClip attackSound; // Attack sound effect

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Attack();
        }
    }

    void Attack()
    {
        Vector3 attackDirection = GetMouseDirection(); // Get direction based on cursor
        Vector3 spawnPosition = attackSpawnPoint.position + attackDirection * attackRange; // Offset attack in that direction

        // Instantiate attack prefab
        GameObject attackInstance = Instantiate(attackPrefab, spawnPosition, Quaternion.identity);

        // Rotate attack to face cursor
        attackInstance.transform.right = attackDirection;

        // Play attack sound with random pitch
        if (audioSource != null && attackSound != null)
        {
            audioSource.pitch = Random.Range(1.0f, 5.0f); // Random pitch between 1.0 and 5.0
            audioSource.PlayOneShot(attackSound);
        }

        // Destroy attack object after duration
        Destroy(attackInstance, attackDuration);
    }

    Vector3 GetMouseDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure it's in 2D space
        Vector3 direction = (mousePosition - attackSpawnPoint.position).normalized;
        return direction;
    }
}
