using UnityEngine;

public class PlayOnTrigger : MonoBehaviour
{
    public AudioSource audioSource; // Assign in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && audioSource != null)
        {
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
