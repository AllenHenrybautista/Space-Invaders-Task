using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private TextMeshProUGUI totalScore;
    [SerializeField] private GameObject waveClearedPanel;
    [SerializeField] private GameObject navPanel;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;

    void Start()
    {
        setupUI();
    }

    private void OnEnable()
    {
        if (scoreManager != null)
            scoreManager.OnScoreChanged += UpdateScoreUI;
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHealthUI;
        if (gameManager != null)
        {
            gameManager.OnWaveCleared += ShowWaveClearedMessage;
            gameManager.OnGameOver += ShowGameOverMessage;
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null)
            scoreManager.OnScoreChanged -= UpdateScoreUI;
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        if (gameManager != null)
        {
            gameManager.OnWaveCleared -= ShowWaveClearedMessage;
            gameManager.OnGameOver -= ShowGameOverMessage;
        }
    }

    private void setupUI()
    {
        if (scoreManager != null)
            UpdateScoreUI(scoreManager.CurrentScore);
        if (playerHealth != null)
            UpdateHealthUI(playerHealth.CurrentLife);
    }

    private void UpdateHealthUI(int currentHealth)
    {
        healthText.text = $"Lives: {currentHealth}";
    }

    private void UpdateScoreUI(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }

    private void ShowWaveClearedMessage()
    {
        if (waveClearedPanel != null)
        {
            waveClearedPanel.SetActive(true);

            if (navPanel != null)
                navPanel.SetActive(false);

            if (totalScore != null && scoreManager != null)
                totalScore.text = $"Final Score: {scoreManager.CurrentScore}";
        }
    }

    private void ShowGameOverMessage()
    {
        if (gameOverPanel == null)
            return;

        gameOverPanel.SetActive(true);

        if (navPanel != null)
            navPanel.SetActive(false);

        if (gameOverScoreText != null && scoreManager != null)
            gameOverScoreText.text = $"Final Score: {scoreManager.CurrentScore}";
    }

    public void OnExitPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadMainMenu();
    }
}