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

        if (Input.GetKeyDown(KeyCode.LeftShift)) // Move to next dialogue chunk
        {
            ShowNextDialogue();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Exit UI
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
        if (dialoguePanel.activeSelf || dialogue == null) return; // Prevent restarting dialogue while active

        currentDialogue = dialogue;
        dialoguePanel.SetActive(true);
        nameText.text = currentDialogue.characterName;
        characterPortrait.sprite = currentDialogue.characterPortrait;
        currentDialogueIndex = 0;

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

            if (_audio && currentDialogue != null && currentDialogue.dialogueSFX != null)
            {
                _audio.pitch = Random.Range(0.9f, 1.2f);
                _audio.PlayOneShot(currentDialogue.dialogueSFX);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // Close UI
    public void CloseUI()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        currentDialogue = null;
        currentScrolls = null;
        currentTablets = null;
        currentDialogueIndex = 0;

        onDialogueEnd?.Invoke();
    }
}

