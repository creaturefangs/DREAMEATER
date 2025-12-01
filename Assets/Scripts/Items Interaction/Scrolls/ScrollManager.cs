using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    [Header("Scroll Data")]
    public SO_Scrolls scroll; // Reference to the Scroll ScriptableObject

    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text scrollText;
    [SerializeField] private Image scrollIcon;
    [SerializeField] private GameObject scrollPanel;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private int currentDialogueIndex = 0;

    [Header("Audio")]
    private AudioSource _audio;
    [SerializeField] private AudioClip pickupClip, scrollCompleteClip;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!scrollPanel.activeSelf) return; // Only listen for input when UI is open

        if (Input.GetKeyDown(KeyCode.LeftShift)) // Move to next text chunk
        {
            ShowNextDialogue();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Exit UI (and destroy if done)
        {
            CloseUI();
        }
    }

    public void ToggleUI()
    {
        if (!scrollPanel.activeSelf)
        {
            OpenUI();
        }
        else
        {
            CloseUI();
        }
    }

    private void OpenUI()
    {
        scrollPanel.SetActive(true);
        titleText.text = scroll.titleText;
        scrollIcon.sprite = scroll.icon; // Set the scroll icon
        currentDialogueIndex = 0; // Start from the first line
        ShowNextDialogue();

        if (_audio)
        {
            _audio.PlayOneShot(pickupClip);
        }
    }

    private void CloseUI()
    {
        scrollPanel.SetActive(false);
        scrollText.text = "";

        if (currentDialogueIndex >= scroll.dialogueLines.Length) // If all text was read
        {
            if (_audio)
            {
                _audio.PlayOneShot(scrollCompleteClip); // Play sound before destroying
            }
            Destroy(gameObject); // Destroy the scroll object
        }
    }

    private void ShowNextDialogue()
    {
        if (currentDialogueIndex < scroll.dialogueLines.Length)
        {
            StartTypewriterEffect(scroll.dialogueLines[currentDialogueIndex]);
            currentDialogueIndex++;
        }
        else
        {
            CloseUI(); // Close UI if no more dialogue
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
        scrollText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            scrollText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
