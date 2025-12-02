using UnityEngine;

public class OnlyDoorInteract : DoorInteractableBase
{
    public string doorID = "Door1";
    public bool isLocked = true;

    private bool canInteract = true;
    public float interactionCooldown = 0.5f; // time before door can be interacted again
    public string requiredItemID; // assign the key needed
    public OnlyDoorInteract linkedDoor; // destination door

    public override void Interact()
    {
        if (!canInteract) return;

        canInteract = false;

        // Call UnityEvent for visual/audio effects
        Invoke("EnableInteraction", 4f);

        // Pass interaction to DoorManager
        if (DoorManager.Instance != null)
        {
            DoorManager.Instance.TryDoor(this);
        }
        else
        {
            Debug.LogWarning("DoorManager instance not found!");
        }

        // Re-enable interaction after cooldown
        Invoke(nameof(EnableInteraction), interactionCooldown);
    }

    private void EnableInteraction()
    {
        canInteract = true;
    }
}