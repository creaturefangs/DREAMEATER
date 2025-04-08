using System.Collections.Generic;
using UnityEngine;

public class BellPuzzleManager : MonoBehaviour
{
    public List<int> correctSequence = new List<int> { 2, 3, 1 };
    private List<int> currentSequence = new List<int>();

    // Returns true if the bell press was part of the correct sequence
    public bool RingBell(int bellID)
    {
        currentSequence.Add(bellID);

        for (int i = 0; i < currentSequence.Count; i++)
        {
            if (currentSequence[i] != correctSequence[i])
            {
                Debug.Log("Incorrect sequence. Resetting.");
                currentSequence.Clear();
                return false; // wrong bell
            }
        }

        // Puzzle completed
        if (currentSequence.Count == correctSequence.Count)
        {
            Debug.Log("Correct sequence! Puzzle complete.");
            LevelController.Instance?.CompletePuzzle();
        }

        return true; // correct bell so far
    }
}
