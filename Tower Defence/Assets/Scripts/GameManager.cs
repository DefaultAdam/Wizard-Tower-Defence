using UnityEngine;

// game loop manager for game flow
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // static reference to game manager

    public GameObject pausePanel; // pause screen overlay
    public bool isPaused = false; // current pause state

    // set instance on awake
    void Awake()
    {
        instance = this;
    }

    // handle pause key input
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
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
}
