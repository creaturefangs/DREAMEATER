using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LanternSaveSpot : MonoBehaviour
{
    public GameObject uiPanel; // Assign in Inspector

    public enum RespawnPosition
    {
        Above,
        Below,
        Left,
        Right
    }

    public RespawnPosition selectedRespawnPosition; // Dropdown in Inspector
    public float offsetDistance = 1.0f; // Adjust spawn offset


    private void Start()
    {
        uiPanel.SetActive(false);
    }

    public void ShowUI()
    {
     
        uiPanel.SetActive(true);
       
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
    }

    public void SaveRespawnPoint()
    {
        Vector2 respawnPosition = GetRespawnPosition();
        PlayerPrefs.SetFloat("RespawnX", respawnPosition.x);
        PlayerPrefs.SetFloat("RespawnY", respawnPosition.y);
        PlayerPrefs.Save();
    }

    private Vector2 GetRespawnPosition()
    {
        switch (selectedRespawnPosition)
        {
            case RespawnPosition.Above:
                return new Vector2(transform.position.x, transform.position.y + offsetDistance);
            case RespawnPosition.Below:
                return new Vector2(transform.position.x, transform.position.y - offsetDistance);
            case RespawnPosition.Left:
                return new Vector2(transform.position.x - offsetDistance, transform.position.y);
            case RespawnPosition.Right:
                return new Vector2(transform.position.x + offsetDistance, transform.position.y);
            default:
                return transform.position;
        }
    }
}
