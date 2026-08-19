using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFormationController : MonoBehaviour
{
    [SerializeField] private float baseMoveSpeed = 1.5f;
    [SerializeField] private float stepDownDistance = 0.5f;
    [SerializeField] private float speedUpPerEnemyLost = 0.15f;

    private readonly List<Enemy> aliveEnemies = new();
    private int startingEnemyCount;
    private int direction = 1;
    private float leftBoundaryX;
    private float rightBoundaryX;
    private bool isActive;

    public event Action OnAllEnemiesDefeated;

    public IReadOnlyList<Enemy> AliveEnemies => aliveEnemies;

    public void Initialize(float leftBoundary, float rightBoundary)
    {
        leftBoundaryX = leftBoundary;
        rightBoundaryX = rightBoundary;
        Enemy[] enemiesInFormation = GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in enemiesInFormation)
        {
            aliveEnemies.Add(enemy);
            enemy.OnDeath += HandleEnemyDeath;
        }
        startingEnemyCount = aliveEnemies.Count;
        isActive = true;
    }

    // Called by GameManager when the game ends (either win or loss), so the
    // formation freezes in place instead of continuing to move during the
    // game-over/wave-cleared transition.
    public void Stop()
    {
        isActive = false;
    }

    private void Update()
    {
        SwarmBehavior();
    }

    private void SwarmBehavior()
    {
        if (!isActive || aliveEnemies.Count == 0)
            return;
        float currentSpeed = CalculateCurrentSpeed();
        transform.position += Vector3.right * direction * currentSpeed * Time.deltaTime;
        float halfWidth = CalculateHalfWidth();
        float edgeX = transform.position.x + halfWidth * direction;
        bool hitRightWall = direction > 0 && edgeX >= rightBoundaryX;
        bool hitLeftWall = direction < 0 && edgeX <= leftBoundaryX;
        if (hitRightWall || hitLeftWall)
            StepDown();
    }

    private void StepDown()
    {
        transform.position += Vector3.back * stepDownDistance;
        direction *= -1;
    }

    private float CalculateCurrentSpeed()
    {
        int enemiesLost = startingEnemyCount - aliveEnemies.Count;
        return baseMoveSpeed + (enemiesLost * speedUpPerEnemyLost);
    }

    private float CalculateHalfWidth()
    {
        float minLocalX = float.MaxValue;
        float maxLocalX = float.MinValue;
        foreach (Enemy enemy in aliveEnemies)
        {
            float localX = enemy.transform.position.x - transform.position.x;
            minLocalX = Mathf.Min(minLocalX, localX);
            maxLocalX = Mathf.Max(maxLocalX, localX);
        }
        return Mathf.Max(Mathf.Abs(minLocalX), Mathf.Abs(maxLocalX));
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath;
        aliveEnemies.Remove(enemy);
        if (aliveEnemies.Count == 0)
            OnAllEnemiesDefeated?.Invoke();
    }
}