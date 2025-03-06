using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    public Collider2D attackCollider; // Collider used for attacking
    public int attackDamage = 10; // Damage dealt to enemies
    public float attackDuration = 0.3f; // Time the attack collider is active

    private bool isAttacking = false; // Prevent multiple attacks at once

    [Header("Audio")]
    public AudioSource audioSource; // Audio source component
    public AudioClip attackSound; // Sound effect for attack

    void Update()
    {
        if ( Input.GetMouseButtonDown(0)) // Attack input
        {
            if (!isAttacking)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack"); // Play attack animation
        attackCollider.enabled = true; // Enable the attack collider

        // Play attack sound
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Disable attack collider after a short delay
        Invoke(nameof(ResetAttack), attackDuration);
    }

    void ResetAttack()
    {
        attackCollider.enabled = false; // Disable collider
        isAttacking = false;
    }

    // Detect enemies in the attack range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }
}
