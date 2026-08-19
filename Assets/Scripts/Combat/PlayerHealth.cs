using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxLife = 4;

    public Team Team => Team.Player;

    // This is now a real event (same style as ScoreManager)
    public event Action<int> OnHealthChanged;

    // Optional but useful: let other scripts read the current value
    public int CurrentLife => currentLife;
    public int MaxLife => maxLife;

    private int currentLife;

    private void Awake()
    {
        currentLife = maxLife;
    }

    private void OnEnable()
    {
        // Tell listeners the starting health
        OnHealthChanged?.Invoke(currentLife);
    }

    public void TakeDamage(int amount)
    {
        currentLife -= amount;
        currentLife = Mathf.Max(currentLife, 0);

        Debug.Log($"Player hit! Life remaining: {currentLife}");

        // Notify the UI (and anything else listening)
        OnHealthChanged?.Invoke(currentLife);

        if (currentLife <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Game Over :(");
        Time.timeScale = 0f;
        // will add a game over screen later
    }
}