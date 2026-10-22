using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverPanel;

    [Header("Survival Score")]
    public int pointsPerSecond = 1;

    public bool IsGameOver => isGameOver;

    private bool isGameOver = false;
    private float survivalTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // +pointsPerSecond for every full second survived
        survivalTimer += Time.deltaTime;
        if (survivalTimer >= 1f)
        {
            survivalTimer -= 1f;
            AddScore(pointsPerSecond);
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return; // guard: only run once
        isGameOver = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameOver();

        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + score;

        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
}