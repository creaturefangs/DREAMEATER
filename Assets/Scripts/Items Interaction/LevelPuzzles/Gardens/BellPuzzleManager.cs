using System.Collections.Generic;
using UnityEngine;

public class BellPuzzleManager : MonoBehaviour
{
    public List<int> correctSequence = new List<int> { 2, 3, 1 }; // IDs of bells in correct order
    private List<int> currentSequence = new List<int>();

    public void RingBell(int bellID)
    {
        currentSequence.Add(bellID);

        // Check if sequence is correct so far
        for (int i = 0; i < currentSequence.Count; i++)
        {
            if (currentSequence[i] != correctSequence[i])
            {
                Debug.Log("Incorrect sequence. Resetting.");
                currentSequence.Clear();
                return;
            }
        }

        // Puzzle complete!
        if (currentSequence.Count == correctSequence.Count)
        {
            Debug.Log("Correct sequence! Puzzle complete.");
            LevelController.Instance?.CompletePuzzle();
        }
    }
}
