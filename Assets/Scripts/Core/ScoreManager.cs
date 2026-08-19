using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action<int> OnScoreChanged;

    private int currentScore;

    public int CurrentScore => currentScore;

    private void OnEnable()
    {
        Enemy.AnyEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        Enemy.AnyEnemyDied -= HandleEnemyDied;
    }

    private void HandleEnemyDied(Enemy enemy)
    {
        AddScore(enemy.PointValue);
    }

    private void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }
}