using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    [SerializeField] private AudioSource audio;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    [Header("Items")]
    private SO_Items currentItem;
    private int currentItemLineIndex = 0;
    private System.Action<bool> currentItemCallback;

    [Header("Item Choice Buttons")]
    public GameObject itemChoicePanel;
    public Button pickUpButton;
    public Button leaveButton;

    public GameObject wolfBossController; // assign in inspector or dynamically

    // Hidden Item Support
    private bool showingHiddenItem = false;
    private System.Action hiddenItemFinishedCallback;
    private bool hiddenItemLeadsToItemDialogue = false;
    private SO_Items hiddenItemData;
    private System.Action<bool> hiddenItemItemCallback;



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

        // SPACE = advance or complete typing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                CompleteTypingCurrentLine();
            }
            else
            {
                ShowNext();
            }
        }

        // ENTER = close ONLY if dialogue is completely finished
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (IsAllDialogueFinished())
            {
                CloseUI();
            }
        }

        if (showingHiddenItem)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                {
                    StopCoroutine(typingCoroutine);
                    dialogueText.text = dialogueText.text; // instantly complete
                    isTyping = false;
                }
                else
                {
                    FinishHiddenItemMessage();
                }
            }

            else if (Input.GetKeyDown(KeyCode.Return))
            {
                FinishHiddenItemMessage();
            }

            return; // Don't process normal dialogue logic while showing hidden message

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

            if (audio)
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
                    audio.pitch = Random.Range(0.9f, 1.2f);
                    audio.PlayOneShot(clipToPlay);
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

    private bool IsAllDialogueFinished()
    {
        if (currentDialogue != null)
            return currentDialogueIndex >= currentDialogue.dialogueLines.Length;

        if (currentScrolls != null)
            return currentDialogueIndex >= currentScrolls.dialogueLines.Length;

        if (currentTablets != null)
            return currentDialogueIndex >= currentTablets.dialogueLines.Length;

        if (currentItem != null)
            return currentItemLineIndex >= currentItem.itemLines.Length;

        return true;
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

        UnityEvent tempEvent = onDialogueEnd;
        onDialogueEnd = null;  // Clear the event temporarily
        tempEvent?.Invoke();  // Now safely invoke
        onDialogueEnd = tempEvent;

        // Check if the dialogue has finished, and if the boss fight should start
        if (triggersBattle && wolfBossController != null)
        {
            // Check if the WolfBoss script is available and trigger the battle
            WolfBoss wolfBoss = wolfBossController.GetComponent<WolfBoss>();
            if (wolfBoss != null)
            {
                // Close the dialogue and start the boss battle
                wolfBoss.StartBattle();
            }

            // Assuming the InteractableObject is not needed here anymore
            // if you still need to disable it, you can do so inside the WolfBoss script
        }
    }

    ///THIS IS THE START OF ITEM INTERACTION DIALOGUE FOR SO_ITEMS/
    public void StartItemInteraction(SO_Items item, System.Action<bool> onChoiceMade)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "";
        currentItem = item;
        nameText.text = item.itemText;
        characterPortrait.sprite = item.itemIcon;
        currentItemLineIndex = 0;
        StartCoroutine(TypeItemLine(onChoiceMade));
    }

    private IEnumerator TypeItemLine(System.Action<bool> onChoiceMade)
    {   
        isTyping = true;
        dialogueText.text = "";

        string line = currentItem.itemLines[currentItemLineIndex];
        if (currentItem.iteminteractSFX != null)
            audio.PlayOneShot(currentItem.iteminteractSFX);

        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);

            if (audio)
            {
                AudioClip clipToPlay = null;

                if (currentItem != null && currentItem.typeSFX != null)
                    clipToPlay = currentItem.typeSFX;

                if (clipToPlay != null)
                {
                    audio.pitch = Random.Range(0.9f, 1.2f);
                    audio.PlayOneShot(clipToPlay);
                }
            }
        }

        Debug.Log("Typing item line: " + line);

        isTyping = false;
        currentItemLineIndex++;

        if (currentItemLineIndex < currentItem.itemLines.Length)
        {
            Invoke(nameof(ContinueItemDialogue), 0.5f); // Auto-continue after brief pause
        }
        else
        {
            ShowItemChoiceButtons(onChoiceMade); // Present pickup/leave choices
        }
    }

    public void ShowHiddenItemMessage(SO_Items item, System.Action<bool> onChoiceMade)
    {
        showingHiddenItem = true;

        hiddenItemLeadsToItemDialogue = true;
        hiddenItemData = item;
        hiddenItemItemCallback = onChoiceMade;

        dialoguePanel.SetActive(true);
        dialogueText.text = "";
        nameText.text = "";
        characterPortrait.sprite = null;

        StartTypewriterEffect(item.hiddenItemMessage);
    }

    private void FinishHiddenItemMessage()
    {
        showingHiddenItem = false;

        // If this hidden message should lead into the item dialogue
        if (hiddenItemLeadsToItemDialogue && hiddenItemData != null)
        {
            StartItemInteraction(hiddenItemData, hiddenItemItemCallback);

            // Reset hidden tracking
            hiddenItemLeadsToItemDialogue = false;
            hiddenItemData = null;
            hiddenItemItemCallback = null;

            return;
        }

        // If it's ONLY a hidden message without item continuation
        CloseUI();
    }

    private void ContinueItemDialogue()
    {
        StartCoroutine(TypeItemLine(currentItemCallback));
    }


    private void ShowItemChoiceButtons(System.Action<bool> callback)
    {
        currentItemCallback = callback;

        itemChoicePanel.SetActive(true);

        pickUpButton.onClick.RemoveAllListeners();
        leaveButton.onClick.RemoveAllListeners();

        pickUpButton.onClick.AddListener(() => AcceptItemChoice(true));
        leaveButton.onClick.AddListener(() => AcceptItemChoice(false));
    }

    public void AcceptItemChoice(bool pickedUp)
    {
        itemChoicePanel.SetActive(false); // hide buttons
        CloseUI();
        currentItemCallback?.Invoke(pickedUp);
        currentItem = null;
    }
     
}

