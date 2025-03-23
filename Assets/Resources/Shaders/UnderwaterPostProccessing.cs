using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class ForceUnderwaterEffect : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;

    void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.weight = 1;
            Debug.Log("Underwater effect forced ON.");
        }
        else
        {
            Debug.LogError("Post Process Volume not assigned!");
        }
    }
}