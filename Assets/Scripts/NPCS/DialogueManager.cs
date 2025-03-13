using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue Data")]
    private SO_Dialogue currentDialogue; // Store the current dialogue data

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

    public void StartDialogue(SO_Dialogue dialogue)
    {
        if (dialoguePanel.activeSelf) return; // Prevent restarting dialogue while one is active

        currentDialogue = dialogue;
        dialoguePanel.SetActive(true);
        nameText.text = currentDialogue.characterName;
        characterPortrait.sprite = currentDialogue.characterPortrait;
        currentDialogueIndex = 0;
        ShowNextDialogue();

        onDialogueStart?.Invoke();
    }

    public void CloseUI()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        if (currentDialogueIndex >= currentDialogue.dialogueLines.Length)
        {
            onDialogueEnd?.Invoke();
        }
    }

    private void ShowNextDialogue()
    {
        if (currentDialogue == null) return;

        if (currentDialogueIndex < currentDialogue.dialogueLines.Length)
        {
            StartTypewriterEffect(currentDialogue.dialogueLines[currentDialogueIndex]);
            currentDialogueIndex++;
        }
        else
        {
            CloseUI();
        }
    }

    public void StartScrollDialogue(SO_Scrolls scroll)
    {
        if (scroll == null) return;

        dialoguePanel.SetActive(true);
        nameText.text = scroll.title;
        characterPortrait.sprite = scroll.image;
        currentDialogueIndex = 0;

        onDialogueStart?.Invoke();

        ShowNextScroll(scroll);
    }

    private void ShowNextScroll(SO_Scrolls scroll)
    {
        if (currentDialogueIndex < scroll.contents.Length)
        {
            StartTypewriterEffect(scroll.contents[currentDialogueIndex]);
            currentDialogueIndex++;
        }
        else
        {
            CloseUI();
        }
    }

    public void StartTabletDialogue(SO_Tablets tablet)
    {
        if (tablet == null) return;

        dialoguePanel.SetActive(true);
        nameText.text = tablet.title;
        characterPortrait.sprite = tablet.image;
        currentDialogueIndex = 0;

        onDialogueStart?.Invoke();

        ShowNextTablet(tablet);
    }

    private void ShowNextTablet(SO_Tablets tablet)
    {
        if (currentDialogueIndex < tablet.contents.Length)
        {
            StartTypewriterEffect(tablet.contents[currentDialogueIndex]);
            currentDialogueIndex++;
        }
        else
        {
            CloseUI();
        }
    }











    private void StartTypewriterEffect(string message)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false; // Mark as not typing
        }

        dialogueText.text = ""; // Clear text before starting
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        dialogueText.text = ""; // Clear previous text
        isTyping = true;

        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            Debug.Log($"Typing: {dialogueText.text}"); // Debug to check unexpected changes

            if (_audio && currentDialogue.dialogueSFX != null)
            {
                _audio.pitch = Random.Range(0.9f, 1.2f);
                _audio.PlayOneShot(currentDialogue.dialogueSFX);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }
}

