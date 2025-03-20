using UnityEngine;

public class ViolenceController : MonoBehaviour
{
    [Header("Violence Enemy Settings")]
    public GameObject violenceEnemy;  // Reference to the enemy in the scene

    private void Start()
    {
        if (violenceEnemy != null)
        {
            violenceEnemy.SetActive(false); // Ensure it's inactive at start
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && violenceEnemy != null && !violenceEnemy.activeInHierarchy)
        {
            Debug.Log("Player entered Violence Territory! Activating Violence.");
            violenceEnemy.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && violenceEnemy != null && violenceEnemy.activeInHierarchy)
        {
            Debug.Log("Player left Violence Territory! Deactivating Violence.");
            violenceEnemy.SetActive(false);
        }
    }
}
