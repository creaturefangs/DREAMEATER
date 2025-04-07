using UnityEngine;

public class Bell : MonoBehaviour
{
    public int bellID;
    public AudioClip bellSound;
    private AudioSource audioSource;
    private BellPuzzleManager puzzleManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        puzzleManager = FindObjectOfType<BellPuzzleManager>();
    }

    public void Ring()
    {
        audioSource.PlayOneShot(bellSound);
        puzzleManager.RingBell(bellID);
    }

    private void OnMouseDown() // or use your interaction method
    {
        Ring();
    }
}


