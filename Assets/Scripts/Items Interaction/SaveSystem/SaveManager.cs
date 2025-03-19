using UnityEngine;
using System.Collections; 

public class SaveManager : MonoBehaviour
{
    public static void SaveRespawnPoint(Vector2 position)
    {
        PlayerPrefs.SetFloat("RespawnX", position.x);
        PlayerPrefs.SetFloat("RespawnY", position.y);
        PlayerPrefs.Save();
    }

    public static Vector2 LoadRespawnPoint()
    {
        if (PlayerPrefs.HasKey("RespawnX") && PlayerPrefs.HasKey("RespawnY"))
        {
            return new Vector2(PlayerPrefs.GetFloat("RespawnX"), PlayerPrefs.GetFloat("RespawnY"));
        }
        return Vector2.zero; // Default spawn position
    }
}
