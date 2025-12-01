using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    public AudioSource backgroundMusic;

    public static bool GameIsPaused { get; private set; } = false; // Global reference for other scripts

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
        Time.timeScale = 0f;
        isPaused = true;
        GameIsPaused = true; // Set global pause state

        if (backgroundMusic != null)
        {
            backgroundMusic.Pause();
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        GameIsPaused = false; // Reset global pause state

        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause();
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Ensure time resumes before quitting
        Application.Quit();
    }
}
