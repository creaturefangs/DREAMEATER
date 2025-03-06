using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TabletsManager : MonoBehaviour
{
    [Header("Tablet Data")]
    public SO_Tablets tablet;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text tabletText;
    [SerializeField] private GameObject tabletPanel;
    [SerializeField] private GameObject interactionUI; // UI prompt to show when player is near

    
    public float typingSpeed = 0.05f; // Speed of typewriter effect

    private bool _isPickedUp = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Transform player;

    [Header("Audio")]
    private AudioSource _audio;
    [SerializeField] private AudioClip pickupClip, tabletdropClip;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

    }

    public void ToggleUI()
    {
        if (!_isPickedUp)
        {
            PickupNote();
        }
        else
        {
            DropNote();
        }
        _isPickedUp = !_isPickedUp;
    }

    private void PickupNote()
    {
        tabletPanel.SetActive(true);
        StartTypewriterEffect(tablet.noteText);

        if (_audio && !_audio.isPlaying)
        {
            _audio.PlayOneShot(pickupClip);
        }
    }

    private void DropNote()
    {
        tabletPanel.SetActive(false);
        tabletText.text = "";

        if (_audio && !_audio.isPlaying)
        {
            _audio.PlayOneShot(tabletdropClip);
        }
    }


    // Typewriter effect for text display
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
        tabletText.text = ""; // Clear previous text

        foreach (char letter in message.ToCharArray())
        {
            tabletText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

}