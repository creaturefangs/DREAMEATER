using System.Collections;
using UnityEngine;

public class EvilEyeController : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Life Steal Settings")]
    public float stealRange = 2f;           // Distance where life steal begins
    public float lifeStealAmount = 5f;      // Amount of health drained
    public float stealCooldown = 1.5f;      // Delay between each drain

    [Header("Health Steal Visual")]
    public GameObject healthOrbPrefab;      // The orb that flies from player -> enemy
    public float orbSpeed = 4f;

    private Transform player;
    private HealthBarManager playerHealth;

    private bool canSteal = true;

    private void Start()
    {
        // Find player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<HealthBarManager>();
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // Move toward player
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );

            // Check for life-steal distance
            float dist = Vector2.Distance(transform.position, player.position);

            if (dist <= stealRange && canSteal)
            {
                StartCoroutine(PerformLifeSteal());
            }
        }
    }

    private IEnumerator PerformLifeSteal()
    {
        canSteal = false;

        // Deal damage to player
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(lifeStealAmount);
        }

        // Spawn the orb visual
        if (healthOrbPrefab != null)
        {
            GameObject orb = Instantiate(healthOrbPrefab, player.position, Quaternion.identity);
            StartCoroutine(MoveOrbToEnemy(orb));
        }

        yield return new WaitForSeconds(stealCooldown);
        canSteal = true;
    }

    private IEnumerator MoveOrbToEnemy(GameObject orb)
    {
        while (orb != null && Vector2.Distance(orb.transform.position, transform.position) > 0.1f)
        {
            orb.transform.position = Vector2.MoveTowards(
                orb.transform.position,
                transform.position,
                orbSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Destroy on arrival
        if (orb != null)
            Destroy(orb);
    }
}
