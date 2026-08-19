using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerHealth playerHealth;

    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;



    void Start()
    {
        setupUI();
    }

    private void OnEnable()
    {
        if(scoreManager != null)
            scoreManager.OnScoreChanged += UpdateScoreUI;
    }

    private void OnDisable()
    {
        if(scoreManager != null)
            scoreManager.OnScoreChanged -= UpdateScoreUI;
    }

    private void setupUI()
    {
        if (scoreManager != null)
            UpdateScoreUI(scoreManager.CurrentScore);
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void UpdateHealthUI(int currentHealth)
    {
        healthText.text = $"Health: {currentHealth}";
    }

    private void UpdateScoreUI(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }
}
