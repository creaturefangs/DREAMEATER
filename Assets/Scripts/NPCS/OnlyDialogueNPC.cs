using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyDialogueNPC : DialogueNPCBase
{

    public SO_Dialogue dialogue;
    private bool _canInteract;

    public override void Interact()
    {
        if (_canInteract)
        {
            if (DialogueManager.Instance != null)
            {
                print("Interacting");
                DialogueManager.Instance.StartDialogue(dialogue);
                _canInteract = false;
                Invoke("ChangeInteract", 4f);
            }
            else
            {
                Debug.LogWarning("DialogueController instance not found!");
            }
        }
    }
}
