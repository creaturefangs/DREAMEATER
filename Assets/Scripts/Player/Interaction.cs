using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRadius = 3f;
    public LayerMask interactableLayer;

    [Header("UI Elements")]
    public GameObject interactionUI; // Assign in Inspector
    public TMP_Text interactionText; // Assign in Inspector

    private InteractableObject currentInteractable;

    void Update()
    {
        DetectInteractable();

        if (currentInteractable != null)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                currentInteractable.Interact();
            }
        }
    }

    void DetectInteractable()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRadius, interactableLayer);

        if (hit != null)
        {
            InteractableObject interactable = hit.GetComponent<InteractableObject>();

            if (interactable != currentInteractable)
            {
                currentInteractable = interactable;
                ShowInteractionUI();
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable = null;
                HideInteractionUI();
            }
        }
    }

    void ShowInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
            interactionText.text = "[Space] or [Click] to Interact";
        }
    }

    void HideInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}




