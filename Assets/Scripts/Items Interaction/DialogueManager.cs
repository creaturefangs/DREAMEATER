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
    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private int currentDialogueIndex = 0;

    [Header("Audio")]
    private AudioSource _audio;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
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

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        nameText.text = dialogue.characterName;
        characterPortrait.sprite = dialogue.characterPortrait;
        currentDialogueIndex = 0;
        ShowNextDialogue();

        onDialogueStart?.Invoke();
    }

    public void CloseUI()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        if (currentDialogueIndex >= dialogue.dialogueLines.Length)
        {
            onDialogueEnd?.Invoke();
        }
    }

    private void ShowNextDialogue()
    {
        if (currentDialogueIndex < dialogue.dialogueLines.Length)
        {
            StartTypewriterEffect(dialogue.dialogueLines[currentDialogueIndex]);
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
            if (_audio && dialogue.dialogueSFX != null)
            {
                _audio.pitch = Random.Range(0.9f, 1.2f);
                _audio.PlayOneShot(dialogue.dialogueSFX);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}

