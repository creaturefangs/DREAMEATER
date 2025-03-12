using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Event")]
    public UnityEvent onInteraction;

    public void Interact()
    {
        onInteraction.Invoke();
    }
}
