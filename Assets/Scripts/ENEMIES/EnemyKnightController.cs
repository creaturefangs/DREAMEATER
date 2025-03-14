using UnityEngine;
using System.Collections;

public class EnemyKnightController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float waitTime = 2f; // Time spent at each patrol point
    private int currentPointIndex = 0;

    [Header("Player Detection")]
    public float detectionRange = 5f;
    public LayerMask playerLayer;
    private bool playerDetected = false;

    [Header("Animations")]
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(PatrolRoutine());
    }

    private void Update()
    {
        DetectPlayer();
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!playerDetected)
            {
                // Move to the next patrol point
                Transform targetPoint = patrolPoints[currentPointIndex];
                animator.SetBool("isWalking", true);

                while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
                    yield return null;
                }

                // Arrived at patrol point, play idle animation
                animator.SetBool("isWalking", false);
                yield return new WaitForSeconds(waitTime);

                // Move to next point
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }

            yield return null;
        }
    }

    private void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (player != null)
        {
            playerDetected = true;
            StopAllCoroutines();
            animator.SetBool("isWalking", false);
            TriggerDialogue();
        }
    }

    private void TriggerDialogue()
    {
        Debug.Log("Player detected! Triggering dialogue...");
        // Call dialogue system here
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
