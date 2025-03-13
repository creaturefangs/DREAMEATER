using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public UnityEvent onInteract;

    public float interactionRadius = 3f; // Interaction range
    private Transform player; // Reference to player

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
        if (isInRange)
        {
            Interaction playerInteraction = player.GetComponent<Interaction>();
            if (playerInteraction != null)
            {
                playerInteraction.SetInteractable(this);
            }
        }
    }

    // Draw interaction radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
