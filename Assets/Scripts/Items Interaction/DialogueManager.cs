using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private SO_Dialogue currentDialogue;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioSource npcAudio;

    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private GameObject _currentDialogueNPC;
    private bool _inDialogue = false;
    private GameObject _currentNPC;
    private Coroutine _currentCoroutine = null;

    private PlayerMovement playerController;

    // Holds current dialogue data from OnlyDialogueNPC
    private string[] dialogueLines;
    private AudioClip dialogueSFX;
    private TMP_FontAsset dialogueFont;
    private int fontSize;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
        playerController = FindObjectOfType<PlayerMovement>();

        if (playerController == null)
        {
            Debug.LogError("PlayerMovement script not found in the scene!");
        }
    }

    private void Update()
    {
        if (_currentDialogueNPC != null)
        {
            // Get only X and Y positions for 2D distance check
            float distance = Vector2.Distance(
                new Vector2(playerController.transform.position.x, playerController.transform.position.y),
                new Vector2(_currentDialogueNPC.transform.position.x, _currentDialogueNPC.transform.position.y));

            if (distance > 5f) // Change 5f to your desired interaction range
            {
                print("Outside Distance");
                _currentDialogueNPC = null;
                StopDialogue(_currentNPC);
            }
        }

        if (!dialoguePanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.LeftShift)) // Move to next dialogue
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
        if (dialogue == null || dialogue.dialogueLines.Length == 0)
        {
            Debug.LogError("No dialogue assigned or dialogue is empty!");
            return;
        }

        currentDialogue = dialogue;
        currentDialogueIndex = 0;
        ShowNextDialogue();
    }

    private void StopDialogue(GameObject npc)
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        ResetDialogueText();
        CloseUI();

        _inDialogue = false;

        if (npcAudio.clip != null)
            npcAudio.clip = null;

        if (npc != null)
            npc.GetComponent<Collider>().enabled = true;
    }

    private void ApplyFont()
    {
        if (dialogueFont != null)
        {
            dialogueText.font = dialogueFont;
            dialogueText.fontSize = fontSize;
        }
    }

    private void ResetDialogueText()
    {
        dialogueText.text = "";
    }

    private void ShowNextDialogue()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueLines[currentDialogueIndex]; // Instantly display text
            isTyping = false;
            return;
        }

        if (currentDialogueIndex < dialogueLines.Length)
        {
            typingCoroutine = StartCoroutine(TypeText(dialogueLines[currentDialogueIndex]));
            currentDialogueIndex++;
        }
        else
        {
            CloseUI();
        }
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;

            if (npcAudio && dialogueSFX != null)
            {
                npcAudio.pitch = Random.Range(0.9f, 1.2f);
                npcAudio.PlayOneShot(dialogueSFX);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void CloseUI()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
}

