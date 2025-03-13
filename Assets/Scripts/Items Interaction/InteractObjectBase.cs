using UnityEngine;
using UnityEngine.Events;

public class InteractObjectBase : MonoBehaviour
{
    public UnityEvent onInteract;
    protected bool canInteract = true;

    public virtual void Interact()
    {
        if (canInteract)
        {
            onInteract?.Invoke();
            canInteract = false;
            Invoke(nameof(ResetInteraction), 4f);
        }
    }

    private void ResetInteraction()
    {
        canInteract = true;
    }
}

