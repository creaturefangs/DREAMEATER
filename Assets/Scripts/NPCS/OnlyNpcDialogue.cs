using UnityEngine;

public class OnlyNpcDialogue : NPCDialogueBase
{
    public SO_Dialogue dialogue;
    private bool canInteract = true;

    public override void Interact()
    {
        if (!canInteract) return;

        if (DialogueManager.Instance != null)
        {
            print("Interacting");
            DialogueManager.Instance.StartDialogue(dialogue);
            canInteract = false;
            Invoke("EnableInteraction", 4f);
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
