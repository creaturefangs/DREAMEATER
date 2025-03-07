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

    public SpriteRenderer playerSprite; // Reference to the player's sprite
    public Animator playerAnimator; // Reference to the Animator
    public GameObject deathScreenPanel; // UI panel to show on death

    private void Start()
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

        // Flash red effect
        StartCoroutine(FlashRed());

        // Check for death condition
        if (currentHealth >= maxHealth)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    private IEnumerator FlashRed()
    {
        if (playerSprite != null)
        {
            playerSprite.color = Color.red; // Change to red
            yield return new WaitForSeconds(0.1f); // Flash duration
            playerSprite.color = Color.white; // Revert to original
        }
    }

    private IEnumerator HandleDeath()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die"); // Trigger death animation
        }

        yield return new WaitForSeconds(3f); // Wait for animation to play

        // Fade in death screen UI
        if (deathScreenPanel != null)
        {
            CanvasGroup canvasGroup = deathScreenPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeInUI(canvasGroup));
            }
            else
            {
                deathScreenPanel.SetActive(true);
            }
        }
    }

    private IEnumerator FadeInUI(CanvasGroup canvasGroup)
    {
        float duration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}

