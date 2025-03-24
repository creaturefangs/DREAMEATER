using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public UnityEvent onInteract;

    public float interactionRadius = 3f; // Interaction range
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Find player by tag
    }

    void Update()
    {
        if (player == null) return;

        // Check if player is within interaction range
        float distance = Vector2.Distance(transform.position, player.position);
        bool isInRange = distance <= interactionRadius;

        // Notify player script if it's nearby
        Interaction playerInteraction = player.GetComponent<Interaction>();
        if (playerInteraction != null)
        {
            if (isInRange)
            {
                playerInteraction.SetInteractable(this);
            }
            else if (playerInteraction != null && playerInteraction.interactableObject == this)
            {
                playerInteraction.SetInteractable(null); // Reset when out of range
            }
        }
    }

    public bool IsPlayerInRange(Transform playerTransform)
    {
        return Vector2.Distance(transform.position, playerTransform.position) <= interactionRadius;
    }

    // Draw interaction radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
