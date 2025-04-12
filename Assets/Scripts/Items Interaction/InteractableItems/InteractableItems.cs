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

        DialogueManager.Instance.StartItemInteraction(itemData, OnChoiceMade);
    }

    private void OnChoiceMade(bool pickedUp)
    {
        if (pickedUp)
        {
            InventoryManager.Instance.AddItem(itemData); // or whatever your add function is
            hasBeenCollected = true;

            if (itemData.iteminteractSFX != null)
                AudioSource.PlayClipAtPoint(itemData.iteminteractSFX, transform.position);

            gameObject.SetActive(false); // or destroy, if needed
        }

        isInteracting = false; // Reset here
    }
}
