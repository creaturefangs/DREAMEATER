using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public bool puzzleCompleted = false;

    [SerializeField] private LockableDoorTeleport puzzleDoor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional
        }
    }

    public void CompletePuzzle()
    {
        puzzleCompleted = true;
        if (puzzleDoor != null)
        {
            puzzleDoor.isLocked = false;
            Debug.Log("Puzzle completed! Door is now unlocked.");
        }
    }
}
