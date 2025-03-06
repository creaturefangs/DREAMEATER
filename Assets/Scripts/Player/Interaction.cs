using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRadius = 3f; // How close the player must be
    public string interactionMessage = "Press Space to interact";
    private bool isInRange = false; // Track if the player is nearby
    private Transform player; // Reference to the player

    [Header("UI Elements")]
    public GameObject interactionUI; // UI to show when near
    

    [Header("Interaction Event")]
    public UnityEvent onInteract; // Assignable Unity event

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Find player by tag
    }

    void Update()
    {
        if (player == null) return;

        // Check if player is within range
        isInRange = Vector2.Distance(transform.position, player.position) <= interactionRadius;

        // Show UI prompt if in range
        if (interactionUI != null)
        {
            interactionUI.SetActive(isInRange);
        }

        // Interact when pressing Spacebar in range
        if (isInRange && Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }

        // Check for mouse click interaction
        if (Input.GetMouseButtonDown(0))
        {
            CheckMouseClick();
        }
    }

    void Interact()
    {
        Debug.Log("Interacting with: " + gameObject.name);
        onInteract.Invoke(); // Call UnityEvent

    }

    void CheckMouseClick()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Keep in 2D space

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider != null && hitCollider.gameObject == gameObject)
        {
            Interact();
        }
    }


    // Draw interaction radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}



