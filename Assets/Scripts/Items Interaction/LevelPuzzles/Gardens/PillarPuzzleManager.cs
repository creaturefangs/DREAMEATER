using System.Collections.Generic;
using UnityEngine;

public class PillarPuzzleManager : MonoBehaviour
{
    public LockableDoorTeleport doorToUnlock;

    [Tooltip("Enter the correct order using the Pillar IDs (e.g. 0, 1, 2, 3)")]
    public List<int> correctOrder = new List<int> { 0, 1, 2, 3 };

    private List<int> currentInput = new List<int>();

    [Header("Audio Clips")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip puzzleCompleteSound;

    private AudioSource audioSource;
    public CameraPan cameraPan; // Assign in Inspector
    public Transform doorFocusTarget; // Empty transform at door location

    private void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    public void TryActivatePillar(int pillarID, Pillar pillar)
    {
        // Add to current input
        currentInput.Add(pillarID);

        // Check if current input is valid so far
        for (int i = 0; i < currentInput.Count; i++)
        {
            if (currentInput[i] != correctOrder[i])
            {
                // Incorrect input
                Debug.Log("Wrong pillar! Resetting...");
                PlaySound(wrongSound);
                ResetPuzzle();
                return;
            }
        }

        // Correct input so far
        PlaySound(correctSound);
        pillar.ShowGlow(); // Trigger glow on correct activation

        // If all pillars correct
        if (currentInput.Count == correctOrder.Count)
        {
            Debug.Log("Pillar puzzle complete!");
            PlaySound(puzzleCompleteSound);
            if (doorToUnlock != null)
                doorToUnlock.isLocked = false;
        }
    }

    private void ResetPuzzle()
    {
        currentInput.Clear();

        // Reset all pillars in the scene
        Pillar[] allPillars = FindObjectsOfType<Pillar>();
        foreach (var pillar in allPillars)
        {
            pillar.ResetPillar();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void CompletePuzzle()
    {
        Debug.Log("Puzzle complete!");
        PlaySound(puzzleCompleteSound);

        if (cameraPan != null && doorFocusTarget != null)
        {
            cameraPan.PanToTarget(doorFocusTarget, () =>
            {
                // Shake door, unlock it
                DoorShake shake = doorToUnlock.GetComponent<DoorShake>();
                if (shake != null) shake.ShakeDoor();

                if (doorToUnlock != null)
                    doorToUnlock.isLocked = false;
            });
        }
        else
        {
            // Fallback if no camera pan set
            if (doorToUnlock != null)
                doorToUnlock.isLocked = false;
        }
    }
}


