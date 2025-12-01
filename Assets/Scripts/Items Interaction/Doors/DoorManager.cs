using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorManager : MonoBehaviour
{
    // --------------------------
    // DOOR TYPE
    // --------------------------
    public enum DoorType
    {
        PressToOpen,
        AutoOpenOnTrigger
    }

    [Header("Door Mode")]
    public DoorType doorType = DoorType.PressToOpen;

    // --------------------------
    // LOCK SETTINGS
    // --------------------------
    [Header("Lock Settings")]
    public bool isLocked = true;
    public string requiredItemID;

    // --------------------------
    // DOOR VISUALS
    // --------------------------
    [Header("UI")]
    public static DoorManager Instance;

    [SerializeField] private GameObject promptRoot;   // The UI object that turns on/off
    [SerializeField] private TMP_Text promptText;     // The actual "Press E" text
    public Image fadeImage;                           // Optional fade image


    [Header("Door Objects")]
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject openedDoor;

    // --------------------------
    // AUDIO
    // --------------------------
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip denySFX;
    [SerializeField] private AudioClip teleportSound;

    // --------------------------
    // TELEPORT SETTINGS
    // --------------------------
    [Header("Teleport Settings")]
    public DoorManager linkedDoor;

    public enum SpawnPosition { Above, Below }
    public SpawnPosition spawnPosition = SpawnPosition.Above;

    public float fadeDuration = 0.5f;
    private bool isTeleporting = false;
    


    // ================================
    // INITIALIZATION
    // ================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        if (promptRoot != null)
            promptRoot.SetActive(false);
    }

    private void Start()
    {
        SetupDoorVisuals();
        SetupFadeCanvas();
        SetupAudioSource();
    }

    // ========== UI HANDLING ==========
    public void ShowPrompt(string text)
    {
        if (promptRoot == null || promptText == null) return;

        promptText.text = text;
        promptRoot.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptRoot == null) return;

        promptRoot.SetActive(false);
    }

    // Shows the unlock prompt for the door
    public void ShowUnlockPrompt()
    {
        if (promptRoot != null && promptText != null)
        {
            promptText.text = "Use key to unlock?";
            promptRoot.SetActive(true);
        }
    }

    // Shows a simple locked message
    public void ShowLockedMessage()
    {
        if (promptRoot != null && promptText != null)
        {
            promptText.text = "The door is locked!";
            promptRoot.SetActive(true);

            // Optional: hide automatically after a short delay
            Invoke(nameof(HidePrompt), 2f);
        }
    }

    // ========== DOOR INTERACTION HANDLING ==========
    public void InteractWithDoor(DoorInteractableBase door)
    {
        if (door == null) return;

        door.Interact();

        // Fade logic can be added here later if needed.
    }

    // ================================
    // PRESS-TO-OPEN INTERACTION
    // Called from your interaction system
    // ================================


    public void TryOpenDoor()
    {
        // Player manually interacted — so PressToOpen only
        if (doorType != DoorType.PressToOpen)
            return;

        HandleDoorInteraction();
    }



    // ================================
    // DOOR LOGIC SHARED
    // ================================
    private void HandleDoorInteraction()
    {
        if (!isLocked)
        {
            OpenDoor();
            TeleportPlayerIfLinked();
            return;
        }

        // Locked
        if (InventoryManager.Instance.HasItem(requiredItemID))
        {
            ShowUnlockPrompt();
        }
        else
        {
            ShowLockedMessage();

            if (audioSource && denySFX)
                audioSource.PlayOneShot(denySFX);
        }
    }



    // Called when player presses "Yes" in UI
    public void ConfirmUnlock()
    {
        InventoryManager.Instance.RemoveItemByID(requiredItemID);
        isLocked = false;

        OpenDoor();
        TeleportPlayerIfLinked();
    }



    // ================================
    // DOOR OPENING
    // ================================
    public void OpenDoor()
    {
        if (closedDoor) closedDoor.SetActive(false);
        if (openedDoor) openedDoor.SetActive(true);

        if (audioSource && openSFX)
            audioSource.PlayOneShot(openSFX);
    }

    private void SetupDoorVisuals()
    {
        if (isLocked)
        {
            if (closedDoor) closedDoor.SetActive(true);
            if (openedDoor) openedDoor.SetActive(false);
        }
        else
        {
            if (closedDoor) closedDoor.SetActive(false);
            if (openedDoor) openedDoor.SetActive(true);
        }
    }



    // ================================
    // AUTO OPEN (WALK THROUGH)
    // ================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (doorType != DoorType.AutoOpenOnTrigger)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (isTeleporting)
            return;

        // Locked door blocks teleport
        if (isLocked)
        {
            DoorUI.Instance.ShowLockedMessage();
            if (audioSource && denySFX)
                audioSource.PlayOneShot(denySFX);
            return;
        }

        // Auto open + teleport
        OpenDoor();
        StartCoroutine(TeleportPlayer(other.transform));
    }



    // ================================
    // TELEPORT HELPERS
    // ================================

    private void TeleportPlayerIfLinked()
    {
        if (linkedDoor == null || isTeleporting)
            return;

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            StartCoroutine(TeleportPlayer(player));
    }

    private IEnumerator TeleportPlayer(Transform player)
    {
        isTeleporting = true;

        // Fade out
        yield return StartCoroutine(FadeScreen(1f));

        // Teleport sound
        if (audioSource && teleportSound)
            audioSource.PlayOneShot(teleportSound);

        // ---- New: use linkedDoor.spawnPosition ----
        float yOffset =
            (linkedDoor.spawnPosition == SpawnPosition.Above) ? 4f :
            (linkedDoor.spawnPosition == SpawnPosition.Below) ? -4f : 0f;

        Vector2 targetPos = new Vector2(
            linkedDoor.transform.position.x,
            linkedDoor.transform.position.y + yOffset
        );

        player.position = targetPos;

        yield return new WaitForSeconds(0.2f);

        // Fade in
        yield return StartCoroutine(FadeScreen(0f));

        yield return new WaitForSeconds(0.3f);

        isTeleporting = false;
    }



    // ================================
    // FADE SCREEN
    // ================================
    private IEnumerator FadeScreen(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }



    // ================================
    // INITIALIZATION HELPERS
    // ================================
    private void SetupFadeCanvas()
    {
        GameObject fadeCanvas = GameObject.Find("FadeCanvas");
        if (fadeCanvas == null)
        {
            fadeCanvas = new GameObject("FadeCanvas");
            Canvas c = fadeCanvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;

            fadeImage = new GameObject("FadeImage").AddComponent<Image>();
            fadeImage.transform.SetParent(fadeCanvas.transform);

            RectTransform rt = fadeImage.rectTransform;
            rt.sizeDelta = new Vector2(1920, 1440);
            rt.anchoredPosition = Vector2.zero;

            fadeImage.color = new Color(0, 0, 0, 0);
        }
        else
        {
            fadeImage = fadeCanvas.GetComponentInChildren<Image>();
        }
    }



    private void SetupAudioSource()
    {
        // Use the shared "DoorAudio" if audioSource not assigned
        if (audioSource == null)
        {
            GameObject audioObj = GameObject.Find("DoorAudio");
            if (audioObj != null)
            {
                audioSource = audioObj.GetComponent<AudioSource>();

                if (audioSource == null)
                    Debug.LogWarning("DoorAudio object found, but has no AudioSource component!");
            }
            else
            {
                Debug.LogWarning("DoorAudio GameObject not found in the scene!");
            }
        }
    }
}
