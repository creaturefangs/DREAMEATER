using TMPro;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionMessage = "Press Space to interact";

    [Header("UI Elements")]
    public GameObject interactionUI; // UI prompt
    public TMP_Text interactionText;

    public ScriptableObject interactionData; // Can be SO_Dialogue, SO_Scrolls, or SO_Tablets

    public InteractableObject interactableObject;

    private void Update()
    {
        // Toggle UI based on interaction availability and range
        if (interactionUI != null)
        {
            bool showUI = interactableObject != null && interactableObject.IsPlayerInRange(transform) && !DialogueManager.Instance.IsDialogueActive();
            interactionUI.SetActive(showUI);

            if (interactionText != null && showUI)
            {
                interactionText.text = interactionMessage;
            }
        }

        // Press Space to interact
        if (interactableObject != null && Input.GetKeyDown(KeyCode.Space) && !DialogueManager.Instance.IsDialogueActive())
        {
            Interact();
        }

        // Click to interact (only if within range and not mid-dialogue)
        if (Input.GetMouseButtonDown(0) && !DialogueManager.Instance.IsDialogueActive())
        {
            CheckMouseClick();
        }
    }

    public void SetInteractable(InteractableObject obj)
    {
        interactableObject = obj;
    }

    private void Interact()
    {

        Debug.Log("Interaction.Interact() called");

        if (interactableObject != null)
        {
            interactableObject.onInteract.Invoke(); // Only fire if this doesn’t double-trigger interaction
        }

        if (DialogueManager.Instance != null && interactionData != null && !DialogueManager.Instance.IsDialogueActive())
        {
            DialogueManager.Instance.StartInteraction(interactionData);
        }
    }


    private void CheckMouseClick()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Keep in 2D space

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider != null)
        {
            InteractableObject clickedObject = hitCollider.GetComponent<InteractableObject>();
            if (clickedObject != null && clickedObject.IsPlayerInRange(transform))
            {
                interactableObject = clickedObject;
                Interact();
            }
        }
    }
}



