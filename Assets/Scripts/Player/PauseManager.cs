using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    public AudioSource backgroundMusic; // Assign this in the Inspector


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause game time
        isPaused = true;

        if (backgroundMusic != null)
        {
            backgroundMusic.Pause(); // Pause background music
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;

        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause(); // Resume background music
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Ensure time resumes before quitting



        Application.Quit(); // Quit the application in a build

    }
}
