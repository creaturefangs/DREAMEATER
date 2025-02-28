using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public Image healthBarFill;
    public float maxHealth = 100f;
    private float currentHealth;

    public AudioSource playerAudio;
    public AudioClip damageSFX;

    void Start()
    {
        currentHealth = 0f; // Start at 0 instead of maxHealth
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth += damage; // Increase health instead of subtracting
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent overflow
        UpdateHealthBar();
        playerAudio.PlayOneShot(damageSFX);
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }
}

