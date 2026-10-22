using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public string gameSceneName = "SampleScene";

    void Start()
    {
        Time.timeScale = 1f;

        if (highScoreText != null)
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}