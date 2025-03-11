using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    public int healAmount = 20; // Amount of health restored

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
