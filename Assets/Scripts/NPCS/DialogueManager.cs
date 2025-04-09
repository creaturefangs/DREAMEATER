using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue Data")]
    private SO_Dialogue currentDialogue; // Store the current dialogue data
    private SO_Scrolls currentScrolls;
    private SO_Tablets currentTablets;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private int currentDialogueIndex = 0;

    [Header("Audio")]
    [SerializeField] private AudioSource _audio;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    public GameObject wolfBossController; // assign in inspector or dynamically
   

    private void Awake()
    {
        // Ensure there is only one instance of DialogueManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isTyping)
            {
                // Instantly complete current line instead of advancing
                StopCoroutine(typingCoroutine);
                CompleteTypingCurrentLine();
            }
            else
            {
                ShowNext(); // Smart handler that knows which type we're in
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            CloseUI();
        }

    }

    public void StartInteraction(object data)
    {
        if (data == null) return;

        if (data is SO_Dialogue dialogue)
        {
            StartDialogue(dialogue);
        }
        else if (data is SO_Scrolls scroll)
        {
            StartScrollDialogue(scroll);
        }
        else if (data is SO_Tablets tablet)
        {
            StartTabletDialogue(tablet);
        }
        else
        {
            Debug.LogWarning("Interaction object is not a valid dialogue, scroll, or tablet!");
        }
    }

    //  Start NPC Dialogue
    public void StartDialogue(SO_Dialogue dialogue)
    {
        if (dialoguePanel.activeSelf || dialogue == null) return;

        currentDialogue = dialogue;
        dialoguePanel.SetActive(true);
        nameText.text = currentDialogue.characterName;
        dialogueText.font = currentDialogue.dialogueFont;
        characterPortrait.sprite = currentDialogue.characterPortrait;
        currentDialogueIndex = 0;

        // Update wolf sprite at start of dialogue
        if (wolfBossController != null && currentDialogue.dialogueSprites != null && currentDialogue.dialogueSprites.Length > 0)
        {
            SpriteRenderer wolfSpriteRenderer = wolfBossController.GetComponent<SpriteRenderer>();
            if (wolfSpriteRenderer != null)
            {
                wolfSpriteRenderer.sprite = currentDialogue.dialogueSprites[0];
            }
        }

        onDialogueStart?.Invoke();
        ShowNextDialogue();
    }

    private void ShowNextDialogue()
    {
        if (currentDialogue == null || currentDialogueIndex >= currentDialogue.dialogueLines.Length)
        {
            CloseUI();
            return;
        }

        dialogueText.text = "";

        // Update UI character portrait
        if (currentDialogue.dialogueSprites != null && currentDialogueIndex < currentDialogue.dialogueSprites.Length)
        {
            characterPortrait.sprite = currentDialogue.characterPortrait; // keep UI consistent
        }

        // Update in-world wolf sprite
        if (wolfBossController != null && currentDialogue.dialogueSprites != null &&
            currentDialogueIndex < currentDialogue.dialogueSprites.Length)
        {
            SpriteRenderer wolfSpriteRenderer = wolfBossController.GetComponent<SpriteRenderer>();
            if (wolfSpriteRenderer != null)
            {
                wolfSpriteRenderer.sprite = currentDialogue.dialogueSprites[currentDialogueIndex];
            }
        }

        StartTypewriterEffect(currentDialogue.dialogueLines[currentDialogueIndex]);
        currentDialogueIndex++;
    }

    //  Start Scroll Dialogue
    public void StartScrollDialogue(SO_Scrolls scroll)
    {
        if (scroll == null) return;

        currentScrolls = scroll;
        dialoguePanel.SetActive(true);
        nameText.text = scroll.titleText;
        characterPortrait.sprite = scroll.icon;
        currentDialogueIndex = 0;

        onDialogueStart?.Invoke();
        ShowNextScroll();
    }

    private void ShowNextScroll()
    {
        if (currentScrolls == null || currentDialogueIndex >= currentScrolls.dialogueLines.Length)
        {
            CloseUI();
            return;
        }

        StartTypewriterEffect(currentScrolls.dialogueLines[currentDialogueIndex]);
        currentDialogueIndex++;
    }

    // Start Tablet Dialogue
    public void StartTabletDialogue(SO_Tablets tablet)
    {
        if (tablet == null) return;

        currentTablets = tablet;
        dialoguePanel.SetActive(true);
        nameText.text = tablet.titleText;
        characterPortrait.sprite = tablet.icon;
        currentDialogueIndex = 0;

        onDialogueStart?.Invoke();
        ShowNextTablet();
    }

    private void ShowNextTablet()
    {
        if (currentTablets == null || currentDialogueIndex >= currentTablets.dialogueLines.Length)
        {
            CloseUI();
            return;
        }

        StartTypewriterEffect(currentTablets.dialogueLines[currentDialogueIndex]);
        currentDialogueIndex++;
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf;
    }

    // Typing Effect
    private void StartTypewriterEffect(string message)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        dialogueText.text = ""; // Clear text before starting
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            //Debug.Log($"Typing: {dialogueText.text}"); // Debugging

            if (_audio)
            {
                AudioClip clipToPlay = null;

                if (currentDialogue != null && currentDialogue.dialogueSFX != null)
                    clipToPlay = currentDialogue.dialogueSFX;
                else if (currentScrolls != null && currentScrolls.dialogueSFX != null)
                    clipToPlay = currentScrolls.dialogueSFX;
                else if (currentTablets != null && currentTablets.dialogueSFX != null)
                    clipToPlay = currentTablets.dialogueSFX;

                if (clipToPlay != null)
                {
                    _audio.pitch = Random.Range(0.9f, 1.2f);
                    _audio.PlayOneShot(clipToPlay);
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void CompleteTypingCurrentLine()
    {
        isTyping = false;

        if (currentDialogue != null)
        {
            dialogueText.text = currentDialogue.dialogueLines[currentDialogueIndex];
        }
        else if (currentScrolls != null)
        {
            dialogueText.text = currentScrolls.dialogueLines[currentDialogueIndex];
        }
        else if (currentTablets != null)
        {
            dialogueText.text = currentTablets.dialogueLines[currentDialogueIndex];
        }
    }

    private void ShowNext()
    {
        if (currentDialogue != null)
            ShowNextDialogue();
        else if (currentScrolls != null)
            ShowNextScroll();
        else if (currentTablets != null)
            ShowNextTablet();
    }

    // Close UI
    public void CloseUI()
    {
        bool triggersBattle = currentDialogue != null && currentDialogue.triggersBossBattle;

        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        currentDialogue = null;
        currentScrolls = null;
        currentTablets = null;
        currentDialogueIndex = 0;

        // Set the event to null to prevent re-triggering during CloseUI
        UnityEvent tempEvent = onDialogueEnd;
        onDialogueEnd = null;  // Clear the event temporarily

        tempEvent?.Invoke();  // Now safely invoke

        // Restore the event
        onDialogueEnd = tempEvent;

        if (triggersBattle && wolfBossController != null)
        {
            wolfBossController.GetComponent<WolfBoss>().StartBattle();
        }
    }
}

