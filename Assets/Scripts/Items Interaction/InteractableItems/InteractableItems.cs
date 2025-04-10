using UnityEngine;
using UnityEngine.Events;

public class InteractableItems : MonoBehaviour
{
    public SO_Items itemData;
    public UnityEvent onItemPickedUp;
    public UnityEvent onItemLeft;

    private bool isInteracting = false;
    private bool hasBeenCollected = false;

    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"{gameObject.name} has no itemData assigned!");
        }
    }

    public void Interact()
    {
        if (hasBeenCollected || isInteracting || itemData == null)
            return;

        isInteracting = true;

        // Play SFX
        if (itemData.iteminteractSFX != null)
        {
            AudioSource.PlayClipAtPoint(itemData.iteminteractSFX, transform.position);
        }

        // Use DialogueManager to show item description
        DialogueManager.Instance.StartItemInteraction(itemData, OnChoosePickUp, OnChooseLeave);
    }

    private void OnChoosePickUp()
    {
        if (hasBeenCollected) return;

        hasBeenCollected = true;
        isInteracting = false;

        InventoryManager.Instance.AddItem(itemData);
        onItemPickedUp?.Invoke();

        // Hide or destroy the object
        gameObject.SetActive(false);
    }

    private void OnChooseLeave()
    {
        isInteracting = false;
        onItemLeft?.Invoke();
    }
}
