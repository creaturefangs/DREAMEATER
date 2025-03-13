using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class OnlyReadableObjects :InteractObjectBase
{
    public SO_Scrolls scrollData; // Reference to SO_Scroll ScriptableObject
    public SO_Tablets tabletData; // Reference to SO_Tablet ScriptableObject
    private bool canInteract = true;

    public override void Interact()
    {
        if (DialogueManager.Instance != null)
        {
            if (scrollData != null)
            {
                print("Interacting");
                DialogueManager.Instance.StartScrollDialogue(scrollData);
                canInteract = false;
                Invoke("EnableInteraction", 4f);
            }
            else if (tabletData != null)
            {
                print("Interacting");
                DialogueManager.Instance.StartTabletDialogue(tabletData);
                canInteract = false;
                Invoke("EnableInteraction", 4f);
            }
        }
        else
        {
            Debug.LogWarning("DialogueManager instance not found!");
        }
    }

    private void EnableInteraction()
    {
        canInteract = true;
    }
}
