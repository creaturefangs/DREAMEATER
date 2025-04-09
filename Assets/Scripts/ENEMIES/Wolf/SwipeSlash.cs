using UnityEngine;
using System.Collections;

public class SwipeSlash : MonoBehaviour
{
    public float duration = 0.4f;
    public float damageWindow = 0.2f;
    private Collider2D swipeCollider;
    private HealthBarManager healthBarManager;

    private void Awake()
    {
        swipeCollider = GetComponent<Collider2D>();
        swipeCollider.enabled = false;
    }

    private void OnEnable()
    {
        StartCoroutine(PerformSlash());
    }

    private IEnumerator PerformSlash()
    {
        // Enable damage
        swipeCollider.enabled = true;
        yield return new WaitForSeconds(damageWindow);

        // Disable collider
        swipeCollider.enabled = false;

        // Let trails fade out before destroy
        yield return new WaitForSeconds(duration - damageWindow);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Or whatever the player tag is
        {
            // Damage logic here
            healthBarManager.TakeDamage(10);
            Debug.Log("Player hit by claw swipe!");
        }
    }
}
