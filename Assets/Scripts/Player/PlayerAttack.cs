using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackPrefab;
    public Transform attackSpawnPoint;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackDuration = 0.3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    void Update()
    {
        if (PauseManager.GameIsPaused) return; // Prevent attack if the game is paused

        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Attack();
        }
    }

    void Attack()
    {
        Vector3 attackDirection = GetMouseDirection();
        Vector3 spawnPosition = attackSpawnPoint.position + attackDirection * attackRange;

        GameObject attackInstance = Instantiate(attackPrefab, spawnPosition, Quaternion.identity);
        attackInstance.transform.right = attackDirection;

        if (audioSource != null && attackSound != null)
        {
            audioSource.pitch = Random.Range(1.0f, 5.0f);
            audioSource.PlayOneShot(attackSound);
        }

        Destroy(attackInstance, attackDuration);
    }

    Vector3 GetMouseDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        return (mousePosition - attackSpawnPoint.position).normalized;
    }
}
