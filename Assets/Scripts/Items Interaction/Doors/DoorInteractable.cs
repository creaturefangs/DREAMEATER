using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorInteractable : DoorInteractableBase
{
    public DoorManager manager;

    public override void Interact()
    {
        if (manager != null)
        {
            manager.TryOpenDoor();
        }
        else
        {
            Debug.LogWarning("DoorInteractable has no DoorManager assigned!");
        }
    }
}