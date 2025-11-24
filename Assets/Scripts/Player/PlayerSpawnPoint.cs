using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
  
        [Header("Player Spawn Settings")]
        public GameObject playerPrefab;      // Player prefab to spawn
        public Transform spawnPoint;         // Location where player appears

        private GameObject currentPlayer;

        private void Start()
        {
            SpawnPlayer();
        }

        public void SpawnPlayer()
        {
            // Clear any existing player object (optional safety measure)
            if (currentPlayer != null)
                Destroy(currentPlayer);

            // Spawn player at the desired location
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        }
   
}
