using UnityEngine;
using UnityEngine.SceneManagement;

// game loop manager for game flow
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // static reference to game manager

    public GameObject pausePanel; // pause screen overlay
    public string menuSceneName = "MainMenu"; // name of main menu scene
    public bool isPaused = false; // current pause state

    // set instance and reset time scale on awake
    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        isPaused = false;
    }

    // initialise pause panel reference and ensure unpaused on start
    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel == null && HUD.instance != null)
        {
            pausePanel = HUD.instance.pausePanel;
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    // handle pause and restart key input
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    // toggle pause state and game speed
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // pause game and freeze time
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    // resume game and unfreeze time
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    // restart active scene and reset time scale
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // return to main menu scene and reset time scale
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
