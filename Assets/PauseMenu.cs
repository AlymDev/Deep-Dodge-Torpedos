using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public string menuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        // No pausing on the game over screen
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (isPaused) Resume();
        else Pause();
    }
    public void Pause()
{
    if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

    isPaused = true;
    pausePanel.SetActive(true);
    Time.timeScale = 0f;
}

    public void Resume()
{
    Debug.Log("Resume clicked");
    isPaused = false;
    pausePanel.SetActive(false);
    Time.timeScale = 1f;
}

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
