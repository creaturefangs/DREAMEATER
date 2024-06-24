using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
