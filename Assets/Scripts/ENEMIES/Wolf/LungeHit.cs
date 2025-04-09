using UnityEngine;

public class LungeHit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Damage logic here
            Debug.Log("Player hit by lunge!");

            other.GetComponent<HealthBarManager>().TakeDamage(15);

            // Shake camera
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.2f, 0.2f); // duration, intensity
            }

            // Optional: destroy hitbox after first hit
            Destroy(gameObject);
        }
    }
}
