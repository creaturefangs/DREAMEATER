using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [Header("HealthBar Variables")]
    public Image healthBarFill;
    public float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] private SpriteRenderer playerSprite;
    public Animator playerAnimator; // Reference to the Animator
    public GameObject deathScreenPanel; // UI panel to show on death

    [Header("Visual & Audio Feedback")]
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip damageSFX;
    [SerializeField] private float flashDuration = 0.2f;

    private Color originalColor;
    private bool isFlashing = false;



    private void Start()
    {
        currentHealth = 0f; // Start at 0 instead of maxHealth
        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth - amount, maxHealth); // Prevent overhealing
        UpdateHealthBar();
        PlayHealEffects();
    }

    private void PlayHealEffects()
    {
        if (playerAudio && healSound)
        {
            playerAudio.PlayOneShot(healSound);
        }

        if (playerSprite)
        {
            StartCoroutine(FlashGreenEffect());
        }
    }

    private IEnumerator FlashGreenEffect()
    {
        if (playerSprite != null)
        {
            isFlashing = true;

            // Change color using material property
            playerSprite.material.SetColor("_Color", Color.green);
            yield return new WaitForSeconds(0.1f);

            // Restore the original color
            playerSprite.material.SetColor("_Color", originalColor);
            isFlashing = false;
        }
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

        if (playerSprite != null)
        {
            originalColor = playerSprite.color; // Store original color
        }
    }

    private IEnumerator FlashRed()
    {
        if (playerSprite != null)
        {
            isFlashing = true;

            // Change color using material property
            playerSprite.material.SetColor("_Color", Color.red);
            yield return new WaitForSeconds(0.1f);

            // Restore the original color
            playerSprite.material.SetColor("_Color", originalColor);
            isFlashing = false;
        }
    }

    private IEnumerator HandleDeath()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die"); // Trigger death animation
        }

        yield return new WaitForSeconds(2f); // Wait for animation to play

        // Disable the player's sprite
        if (playerSprite != null)
        {
            playerSprite.gameObject.SetActive(false);
        }

        // Pause the game but allow music to play
        Time.timeScale = 0f; // Stops all movement and physics
        AudioListener.pause = false; // Ensures global audio isn't paused
        playerAudio.ignoreListenerPause = true; // Allows player's audio to continue

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

