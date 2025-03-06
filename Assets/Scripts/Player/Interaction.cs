using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour

{
    public string interactionMessage = "Press Enter or Space to interact"; // Message displayed to the player
    private bool isInRange = false;  // To check if the player is in range to interact

    // Reference to the UI Text or any UI element that displays the interaction message
    public GameObject interactionUI;

    // UnityEvent that can be assigned in the Inspector for custom interaction logic
    public UnityEvent onInteract;

    // Update is called once per frame
    void Update()
    {
        if (isInRange)
        {
            // Show the interaction UI message
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
            }

            // Check for player input to interact
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                Interact();  // Call the interaction function
            }
        }
        else
        {
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }
    }

    // Trigger event when the player enters the collider range
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Make sure the player has the tag "Player"
        {
            isInRange = true;
        }
    }

    // Trigger event when the player exits the collider range
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Make sure the player has the tag "Player"
        {
            isInRange = false;
        }
    }

    // Function that handles the interaction (can be customized)
    void Interact()
    {
        Debug.Log("Interacting with: " + gameObject.name);

        // Call the UnityEvent (can be assigned in the Inspector)
        onInteract.Invoke();  // This will invoke any functions you add to the UnityEvent

        // Example: Destroy the object on interaction (can be removed or replaced with your own logic)
        // Destroy(gameObject);
    }
}


