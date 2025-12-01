using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TabletsManager : MonoBehaviour
{
    [Header("Tablet Data")]
    public SO_Tablets tablet; // ScriptableObject reference

    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tabletText;
    [SerializeField] private GameObject tabletPanel;
    [SerializeField] private Image tabletIcon;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f; // Speed of typewriter effect

    private bool isTyping = false;
    private bool awaitingInput = false;
    private Coroutine typingCoroutine;
    private int currentChunkIndex = 0;
    private List<string> dialogueChunks = new List<string>();

    [Header("Audio")]
    private AudioSource _audio;
    [SerializeField] private AudioClip pickupClip, tabletdropClip;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void ToggleUI()
    {
        if (!tabletPanel.activeSelf)
        {
            ShowTablet();
        }
        else
        {
            HideTablet();
        }
    }

    private void ShowTablet()
    {
        tabletPanel.SetActive(true);
        titleText.text = tablet.titleText;
        tabletIcon.sprite = tablet.icon; // Assign the icon from ScriptableObject
        PrepareTextChunks();
        currentChunkIndex = 0;
        ShowNextChunk();

        if (_audio && pickupClip)
        {
            _audio.PlayOneShot(pickupClip);
        }
    }

    private void HideTablet()
    {
        tabletPanel.SetActive(false);
        tabletText.text = "";
        awaitingInput = false;

        if (_audio && tabletdropClip)
        {
            _audio.PlayOneShot(tabletdropClip);
        }
    }

    private void PrepareTextChunks()
    {
        dialogueChunks.Clear();
        string currentChunk = "";
        int lineCount = 0;

        foreach (string line in tablet.dialogueLines)
        {
            currentChunk += line + "\n";
            lineCount++;

            if (lineCount >= 3)
            {
                dialogueChunks.Add(currentChunk.Trim());
                currentChunk = "";
                lineCount = 0;
            }
        }

        if (!string.IsNullOrEmpty(currentChunk))
        {
            dialogueChunks.Add(currentChunk.Trim());
        }
    }

    private void ShowNextChunk()
    {
        if (currentChunkIndex < dialogueChunks.Count)
        {
            StartTypewriterEffect(dialogueChunks[currentChunkIndex]);
            currentChunkIndex++;
            awaitingInput = false;
        }
        else
        {
            HideTablet();
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
        tabletText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            tabletText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        awaitingInput = true;
    }

    private void Update()
    {
        if (tabletPanel.activeSelf)
        {
            if (awaitingInput && Input.GetKeyDown(KeyCode.LeftShift))
            {
                ShowNextChunk();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HideTablet();
            }
        }
    }
}