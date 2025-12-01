using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public UnityEvent onInteract;

    public float interactionRadius = 3f;
    private Transform player;

    public ScriptableObject interactionData;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool isInRange = distance <= interactionRadius;

        Interaction playerInteraction = player.GetComponent<Interaction>();

        if (playerInteraction != null)
        {
            if (isInRange)
            {
                playerInteraction.SetInteractable(this);
            }
            else if (playerInteraction.interactableObject == this)
            {
                playerInteraction.SetInteractable(null);
            }
        }
    }

    public bool IsPlayerInRange(Transform playerTransform)
    {
        return Vector2.Distance(transform.position, playerTransform.position) <= interactionRadius;
    }

    public void Interact()
    {

        onInteract?.Invoke();

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
