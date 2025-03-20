using UnityEngine;
using UnityEngine.SceneManagement;

public class BttnController : MonoBehaviour
{
    public void OnClickMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnClickStartGame()
    {
        SceneManager.LoadScene("GameRoom");

    }

    public void GotoTestLevel()
    {
        SceneManager.LoadScene("StartCutscene");
    }

    public void GotoStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }


    public void OnClickExitGame()
    {
        Application.Quit();
    }

    public void ReloadGameFromSave()
    {

        Time.timeScale = 1f;


    }

}
