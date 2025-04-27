using UnityEngine;
using UnityEngine.Playables;

public class PlayTimelineOnTrigger : MonoBehaviour
{
    public PlayableDirector timelineDirector; // Assign in Inspector
    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            if (timelineDirector != null)
            {
                timelineDirector.Play();
                hasPlayed = true;
            }
            else
            {
                Debug.LogWarning("Timeline Director not assigned on " + gameObject.name);
            }
        }
    }
}
