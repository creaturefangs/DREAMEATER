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

    private void StartTypewriterEffect(string message)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;

            // Play typing sound with random pitch
            if (_audio && currentDialogue.dialogueSFX != null)
            {
                _audio.pitch = Random.Range(0.9f, 1.2f);
                _audio.PlayOneShot(currentDialogue.dialogueSFX);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}

