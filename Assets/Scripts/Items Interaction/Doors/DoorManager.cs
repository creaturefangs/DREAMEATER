using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;

    private void Awake() => Instance = this;

    [Header("UI Elements")]
    public GameObject interactionUI;
    public TMP_Text interactionText;
    public Button yesButton;
    public Button noButton;

    private OnlyDoorInteract currentDoor;
    private bool awaitingEnter = false;

    private void Start()
    {
        if (yesButton != null)
            yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null)
            noButton.onClick.AddListener(OnNoClicked);

        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    public void TryDoor(OnlyDoorInteract door)
    {
        currentDoor = door;
        interactionUI.SetActive(true);

        if (door.isLocked && !string.IsNullOrEmpty(door.requiredItemID))
        {
            interactionText.text = $"Door {door.doorID} requires {door.requiredItemID}. Use it?";
            awaitingEnter = false;
        }
        else
        {
            interactionText.text = $"Door {door.doorID} is unlocked. Enter?";
            awaitingEnter = true;
        }
    }

    private void OnYesClicked()
    {
        if (currentDoor == null) return;

        if (!awaitingEnter && currentDoor.isLocked && !string.IsNullOrEmpty(currentDoor.requiredItemID))
        {
            // Check key
            if (InventoryManager.Instance.HasItem(currentDoor.requiredItemID))
            {
                InventoryManager.Instance.RemoveItemByID(currentDoor.requiredItemID);
                currentDoor.isLocked = false;
                interactionText.text = $"Door {currentDoor.doorID} unlocked. Enter?";
                awaitingEnter = true;
            }
            else
            {
                interactionText.text = $"You do not have {currentDoor.requiredItemID}!";
            }
        }
        else if (awaitingEnter)
        {
            // Teleport player
            if (currentDoor.linkedDoor != null)
            {
                Transform player = GameObject.FindGameObjectWithTag("Player").transform;
                player.position = currentDoor.linkedDoor.transform.position;
            }
            CloseUI();
        }
    }

    private void OnNoClicked()
    {
        if (currentDoor == null) return;

        Debug.Log($"Player chose NO on door {currentDoor.doorID}");
        CloseUI();
    }

    private void CloseUI()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);

        currentDoor = null;
    }
}
