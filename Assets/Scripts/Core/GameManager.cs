using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Difficulty difficulty = Difficulty.Normal;
    [SerializeField] private EnemyGridSpawner enemyGridSpawner;
    [SerializeField] private EnemyFormationController enemyFormationController;
    [SerializeField] private EnemyDangerZone dangerZone;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private UIManager uimanager;
    [SerializeField] private float leftBoundaryX = -5f;
    [SerializeField] private float rightBoundaryX = 5f;
    [SerializeField] private float gizmoDepth = 10f;

    public event Action OnWaveCleared;
    public event Action OnGameOver;

    private void Start()
    {
        StartWave();
    }

    // to centralize the wave difficulty and spawning logic, 
    // i've added it here 
    // this also allows the feature to support modularity or expansion.
    private void StartWave()
    {
        enemyGridSpawner.SpawnGrid(difficulty);
        float gridHalfWidth = enemyGridSpawner.GridWidth / 2f;
        float playAreaHalfWidth = (rightBoundaryX - leftBoundaryX) / 2f;
        if (gridHalfWidth > playAreaHalfWidth)
        {
            Debug.LogWarning(
                $"Enemy grid (half-width {gridHalfWidth}) is wider than the play area " +
                $"(half-width {playAreaHalfWidth}). Consider reducing columns or spacing.",
                this
            );
        }
        enemyFormationController.Initialize(leftBoundaryX, rightBoundaryX);
        enemyFormationController.OnAllEnemiesDefeated += HandleWaveCleared;

        if (dangerZone != null)
            dangerZone.OnEnemyReachedZone += HandleGameOver;

        if (playerHealth != null)
            playerHealth.OnPlayerDied += HandleGameOver;
    }

    private void HandleWaveCleared()
    {
        Debug.Log("Wave cleared!");
        OnWaveCleared?.Invoke();
    }

    private void HandleGameOver()
    {
        Debug.Log("Game over.");
        Time.timeScale = 0f;
        enemyFormationController.Stop();
        OnGameOver?.Invoke();
    }

    //Testing Stuffs 
    private void OnDrawGizmos()
    {
        if (enemyGridSpawner == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftBoundaryX, 0f, 0f), new Vector3(leftBoundaryX, 0f, gizmoDepth));
        Gizmos.DrawLine(new Vector3(rightBoundaryX, 0f, 0f), new Vector3(rightBoundaryX, 0f, gizmoDepth));
        float gridHalfWidth = enemyGridSpawner.GetPreviewWidth(difficulty) / 2f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-gridHalfWidth, 0f, 0f), new Vector3(-gridHalfWidth, 0f, gizmoDepth));
        Gizmos.DrawLine(new Vector3(gridHalfWidth, 0f, 0f), new Vector3(gridHalfWidth, 0f, gizmoDepth));
    }
}