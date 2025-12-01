using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorUI : MonoBehaviour
{
    public static DoorUI Instance;

    [Header("UI References")]
    public GameObject panel;            // The whole popup panel
    public TextMeshProUGUI messageText; // Displays text
    public Button yesButton;            // Shows only for unlock prompts
    public Button noButton;

    private DoorInteractable pendingDoor; // The door waiting for confirmation

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Hide();
    }

    // Show “This door is locked” message
    public void ShowLockedMessage()
    {
        pendingDoor = null;

        messageText.text = "It's locked.";

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(true);

        panel.SetActive(true);
    }

    // Show “Use key to unlock this door?” prompt
    public void ShowUnlockPrompt(DoorInteractable door)
    {
        pendingDoor = door;

        messageText.text = "Use the required item to unlock?";

        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        panel.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(ConfirmUnlock);

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(Hide);
    }

    private void ConfirmUnlock()
    {
        if (pendingDoor != null)
            pendingDoor.ConfirmUnlock();

        Hide();
    }

    public void Hide()
    {
        panel.SetActive(false);
        pendingDoor = null;

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
    }
}
