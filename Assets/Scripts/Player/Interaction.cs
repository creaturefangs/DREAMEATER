using TMPro;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionMessage = "Press Space to interact";

    [Header("UI Elements")]
    public GameObject interactionUI; // UI prompt
    public TMP_Text interactionText;

    private InteractableObject interactableObject;

    void Update()
    {
        // Toggle UI based on interaction availability
        if (interactionUI != null)
        {
            interactionUI.SetActive(interactableObject != null);
            if (interactionText != null && interactableObject != null)
            {
                interactionText.text = interactionMessage;
            }
        }

        // Press Space to interact
        if (interactableObject != null && Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }

        // Click to interact
        if (Input.GetMouseButtonDown(0))
        {
            CheckMouseClick();
        }
    }

    public void SetInteractable(InteractableObject obj)
    {
        interactableObject = obj;
    }

    void Interact()
    {
        if (interactableObject != null)
        {
            Debug.Log("Interacting with: " + interactableObject.gameObject.name);
            interactableObject.onInteract.Invoke();
        }
    }

    void CheckMouseClick()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Keep in 2D space

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider != null)
        {
            InteractableObject clickedObject = hitCollider.GetComponent<InteractableObject>();
            if (clickedObject != null)
            {
                interactableObject = clickedObject;
                Interact();
            }
        }
    }
}



