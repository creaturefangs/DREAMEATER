using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [Header("HealthBar Variables")]
    public Image healthBarFill;
    public CanvasGroup healthBarCanvasGroup;
    public float maxHealth = 100f;
    public float currentHealth;

    [SerializeField] private SpriteRenderer playerSprite;
    public Animator playerAnimator;
    public GameObject deathScreenPanel;

    [Header("Visual & Audio Feedback")]
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip damageSFX;
    //[SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float uiFadeDelay = 3f; // Time before fading out

    private Color originalColor;
    private bool isFlashing = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        currentHealth = 0f;
        healthBarCanvasGroup.alpha = 0f; // Start hidden
        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth - amount, maxHealth);
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
        currentHealth += damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        playerAudio.PlayOneShot(damageSFX);

        StartCoroutine(FlashRed());

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
            originalColor = playerSprite.color;
        }

        ShowHealthBar();
    }

    private void ShowHealthBar()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        healthBarCanvasGroup.alpha = 1f;
        fadeCoroutine = StartCoroutine(HideHealthBarAfterDelay());
    }

    private IEnumerator HideHealthBarAfterDelay()
    {
        yield return new WaitForSeconds(uiFadeDelay);
        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            healthBarCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        healthBarCanvasGroup.alpha = 0f;
    }

    private IEnumerator FlashRed()
    {
        if (playerSprite != null)
        {
            isFlashing = true;
            playerSprite.material.SetColor("_Color", Color.red);
            yield return new WaitForSeconds(0.1f);
            playerSprite.material.SetColor("_Color", originalColor);
            isFlashing = false;
        }
    }

    private IEnumerator HandleDeath()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        yield return new WaitForSeconds(1f);

        Time.timeScale = 0f;
        AudioListener.pause = false;
        playerAudio.ignoreListenerPause = true;

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

