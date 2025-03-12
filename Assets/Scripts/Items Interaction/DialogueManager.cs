using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Data")]
    public SO_Dialogue dialogue; // Reference to the Dialogue ScriptableObject

    [Header("UI Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Typing Settings")]
    public float typingSpeed = 1f;

    [Header("Font Settings")]
    private SO_Dialogue fontData; 
    private TMP_Text textComponent;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private int currentDialogueIndex = 0;

    [Header("Audio")]
    [SerializeField] private AudioSource NPCaudio;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private void Awake()
    {
        textComponent = dialogueText; // Assign textComponent properly

        if (fontData && textComponent)
        {
            ApplyFont();
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

    //  This function can be called from Unity Events!
    public void StartDialogueFromEvent()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (dialogue == null || dialogue.dialogueLines == null)
        {
            Debug.LogError("Dialogue data is missing!");
            return;
        }

        Debug.Log($"Starting dialogue: {dialogue.characterName}, Lines: {dialogue.dialogueLines.Length}");

        dialoguePanel.SetActive(true);
        nameText.text = dialogue.characterName;
        characterPortrait.sprite = dialogue.characterPortrait;
        currentDialogueIndex = 0;
        ShowNextDialogue();

        onDialogueStart?.Invoke();
    }

    public void OnDialogueEnd()
    {
        onDialogueEnd?.Invoke(); // Ensure the event is triggered safely
    }


    public void ApplyFont()
    {
        if (fontData != null)
        {
            dialogueText.font = fontData.font;
            dialogueText.fontSize = fontData.fontSize;
        }

    }

    public void ChangeFont(SO_Dialogue newFont)
    {
        fontData = newFont;
        ApplyFont();
    }

    public void CloseUI()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        if (dialogue != null && currentDialogueIndex >= dialogue.dialogueLines.Length)
        {
            onDialogueEnd?.Invoke();
        }
    }

    private void ShowNextDialogue()
    {
        //  Prevent errors if dialogue is missing
        if (dialogue == null || dialogue.dialogueLines == null || dialogue.dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines found!");
            CloseUI();
            return;
        }

        Debug.Log($"Typing line {currentDialogueIndex + 1} of {dialogue.dialogueLines.Length}");

        //  Only proceed if there are lines left
        if (currentDialogueIndex < dialogue.dialogueLines.Length)
        {
            StopTypewriterEffect();
            StartTypewriterEffect(dialogue.dialogueLines[currentDialogueIndex]);
            currentDialogueIndex++; //  Increment AFTER displaying
        }
        else
        {
            Debug.Log("End of dialogue reached, closing UI.");
            CloseUI();
        }
    }

    private void StartTypewriterEffect(string message)
    {
        StopTypewriterEffect(); //  Ensure no overlapping typewriter effects
        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private void StopTypewriterEffect()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        isTyping = false;
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        dialogueText.text = ""; // Clear text before starting

        foreach (char letter in message.ToCharArray())
        {
            if (!isTyping) // Skip animation if interrupted
            {
                dialogueText.text = message;
                break;
            }

            dialogueText.text += letter;

            // Play typing sound with random pitch
            if (NPCaudio && dialogue.dialogueSFX != null)
            {
                NPCaudio.pitch = Random.Range(0.9f, 1.2f);
                NPCaudio.PlayOneShot(dialogue.dialogueSFX);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}

