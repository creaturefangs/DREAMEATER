using TMPro;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionMessage = "Press E to interact";

    [Header("UI Elements")]
    public GameObject interactionUI;
    public TMP_Text interactionText;
  
    public InteractableObject interactableObject;

    private void Update()
    {
        // Show or hide the UI
        if (interactionUI != null)
        {
            bool showUI = interactableObject != null
                          && interactableObject.IsPlayerInRange(transform)
                          && !DialogueManager.Instance.IsDialogueActive();

            interactionUI.SetActive(showUI);

            if (interactionText != null && showUI)
            {
                interactionText.text = interactionMessage;
            }
        }

        // Press E to interact
        if (interactableObject != null
            && Input.GetKeyDown(KeyCode.E)
            && !DialogueManager.Instance.IsDialogueActive())
        {
            Interact();
        }

        // Mouse click interaction
        if (Input.GetMouseButtonDown(0))
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

        // Let the interactable handle its own logic
        if (interactableObject != null)
        {

            interactableObject.Interact();
        }

        // Start dialogue if applicable
        if (DialogueManager.Instance != null
            && interactableObject.interactionData != null
            && !DialogueManager.Instance.IsDialogueActive())
        {
            DialogueManager.Instance.StartInteraction(interactableObject.interactionData);
        }
    }

    private void CheckMouseClick()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mousePosition);
        if (hit != null)
        {
            InteractableObject clicked = hit.GetComponent<InteractableObject>();

            if (clicked != null && clicked.IsPlayerInRange(transform))
            {
                if (clicked != null && clicked.IsPlayerInRange(transform))
                    interactableObject = clicked;
                Interact();
            }
        }
    }
}



