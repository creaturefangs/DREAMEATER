using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    public bool puzzleCompleted = false;
    public AudioSource doorAudio;
    public AudioClip doorUnlocked;

    [SerializeField] private DoorInteractable puzzleDoor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    public void CompletePuzzle()
    {
        puzzleCompleted = true;
        if (puzzleDoor != null)
        {
            puzzleDoor.isLocked = false;
            Debug.Log("Puzzle completed! Door is now unlocked.");

            doorAudio.PlayOneShot(doorUnlocked);

        }
    }
}


