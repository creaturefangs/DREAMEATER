using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorTeleport : MonoBehaviour
{
    public DoorTeleport linkedDoor; // The door to teleport to
    public enum SpawnPosition { Above, Below } // Dropdown selection
    public SpawnPosition spawnPosition = SpawnPosition.Above;

    public float fadeDuration = 0.5f; // Time for fade effect
    public AudioClip teleportSound; // Sound effect
    private bool isTeleporting = false; // Prevents instant re-triggering
    private Image fadeImage;
    private AudioSource audioSource;

    [SerializeField] public bool isLocked; 

    private void Start()
    {
        // Find or create fade UI
        GameObject fadeCanvas = GameObject.Find("FadeCanvas");
        if (fadeCanvas == null)
        {
            fadeCanvas = new GameObject("FadeCanvas");
            Canvas canvas = fadeCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeImage = new GameObject("FadeImage").AddComponent<Image>();
            fadeImage.transform.SetParent(fadeCanvas.transform);
            RectTransform rt = fadeImage.rectTransform;
            rt.sizeDelta = new Vector2(1920, 1440); // Doubles the size for full coverage
            rt.anchoredPosition = Vector2.zero; // Centers the fade
            fadeImage.color = new Color(0, 0, 0, 0); // Transparent at start
        }
        else
        {
            fadeImage = fadeCanvas.GetComponentInChildren<Image>();
        }

        // Set up audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        if (teleportSound)
        {
            audioSource.clip = teleportSound;
        }

        // Debugging: Check if the linked door is properly set
        if (linkedDoor == null)
        {
            Debug.LogError(gameObject.name + " has no linked door assigned!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(gameObject.name + " triggered by " + other.name); // Debugging collision

        if (other.CompareTag("Player") && linkedDoor != null && !isTeleporting)
        {
            Debug.Log("Teleporting player to: " + linkedDoor.gameObject.name);
            StartCoroutine(TeleportPlayer(other.transform));
        }
    }

    private IEnumerator TeleportPlayer(Transform player)
    {
        isTeleporting = true;

        // Fade to black
        Debug.Log("Fading to black...");
        yield return StartCoroutine(FadeScreen(1f));

        // Play teleport sound
        if (teleportSound && audioSource)
        {
            Debug.Log("Playing teleport sound");
            audioSource.Play();
        }

        // Determine spawn position
        float yOffset = (spawnPosition == SpawnPosition.Above) ? 4f : -4f;
        Vector2 targetPosition = new Vector2(linkedDoor.transform.position.x, linkedDoor.transform.position.y + yOffset);

        // Move the player
        Debug.Log("Moving player to: " + targetPosition);
        player.position = targetPosition;

        // Wait before fading back in
        yield return new WaitForSeconds(0.2f);

        // Fade back to clear
        Debug.Log("Fading back to clear...");
        yield return StartCoroutine(FadeScreen(0f));

        // Small delay to prevent instant re-triggering
        yield return new WaitForSeconds(0.3f);

        isTeleporting = false;
        Debug.Log("Teleportation complete!");
    }

    private IEnumerator FadeScreen(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }
}
