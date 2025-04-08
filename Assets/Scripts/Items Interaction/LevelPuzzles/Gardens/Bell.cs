using UnityEngine;
using System.Collections;

public class Bell : MonoBehaviour
{
    public int bellID;
    public AudioClip bellSound;
    public GameObject ringEffectPrefab;

    private AudioSource audioSource;
    private BellPuzzleManager puzzleManager;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;

    [Header("Pitch Settings")]
    public float basePitch = 1f;
    public float pitchStep = 0.2f;

    [Header("Feedback Settings")]
    public Color flashColor = Color.yellow;
    public float flashDuration = 0.2f;
    public float bounceScale = 1.2f;
    public float bounceDuration = 0.15f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        audioSource.pitch = basePitch + (bellID * pitchStep);
        puzzleManager = FindObjectOfType<BellPuzzleManager>();
    }

    public void Ring()
    {
        audioSource.PlayOneShot(bellSound);
        bool isCorrect = puzzleManager.RingBell(bellID);

        if (isCorrect)
        {
            StartCoroutine(FeedbackEffect());

            if (ringEffectPrefab)
            {
                Instantiate(ringEffectPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnMouseDown()
    {
        Ring();
    }

    private System.Collections.IEnumerator FeedbackEffect()
    {
        transform.localScale = originalScale * bounceScale;

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }

        yield return new WaitForSeconds(bounceDuration);
        transform.localScale = originalScale;
    }
}


