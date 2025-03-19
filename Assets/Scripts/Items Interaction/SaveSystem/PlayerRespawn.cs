using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector2 respawnPoint;

    private void Start()
    {
        LoadRespawn();
    }

    private void LoadRespawn()
    {
        if (PlayerPrefs.HasKey("RespawnX") && PlayerPrefs.HasKey("RespawnY"))
        {
            respawnPoint = new Vector2(PlayerPrefs.GetFloat("RespawnX"), PlayerPrefs.GetFloat("RespawnY"));
            transform.position = respawnPoint;
        }
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
    }
}
