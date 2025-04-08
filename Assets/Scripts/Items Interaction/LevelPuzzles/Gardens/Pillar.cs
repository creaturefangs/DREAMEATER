using UnityEngine;

public class Pillar : MonoBehaviour
{
    public int pillarID;
    public PillarPuzzleManager puzzleManager;
    public GameObject glowObject; // Assign a glow child or particle in Inspector

    private bool isActivated = false;

    private void Start()
    {
        if (glowObject != null)
        {
            glowObject.SetActive(false);
        }
    }

    private void OnMouseDown() // Replace with your actual interaction if needed
    {
        if (!isActivated)
        {
            puzzleManager?.TryActivatePillar(pillarID, this);
            isActivated = true;
        }
    }

    public void ShowGlow()
    {
        if (glowObject != null)
        {
            glowObject.SetActive(true);
        }
    }

    public void ResetPillar()
    {
        isActivated = false;
        if (glowObject != null)
        {
            glowObject.SetActive(false);
        }

        // Optional: Reset collider or visuals
        GetComponent<Collider2D>().enabled = true;
    }
}
