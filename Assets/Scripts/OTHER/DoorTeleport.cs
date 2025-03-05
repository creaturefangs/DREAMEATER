using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
  
    public DoorTeleport linkedDoor; // The door to teleport to

    public enum SpawnPosition { Above, Below } // Drop-down selection
    public SpawnPosition spawnPosition = SpawnPosition.Above;

    private bool isTeleporting = false; // Prevents instant re-triggering

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && linkedDoor != null && !isTeleporting)
        {
            StartCoroutine(TeleportPlayer(other.transform));
        }
    }

    private IEnumerator TeleportPlayer(Transform player)
    {
        isTeleporting = true;

        // Determine spawn position
        float yOffset = (spawnPosition == SpawnPosition.Above) ? 1f : -1f;
        Vector2 targetPosition = new Vector2(linkedDoor.transform.position.x, linkedDoor.transform.position.y + yOffset);

        // Move the player to the linked door
        player.position = targetPosition;

        // Small delay to prevent instant re-triggering
        yield return new WaitForSeconds(0.5f);

        isTeleporting = false;
    }
}
