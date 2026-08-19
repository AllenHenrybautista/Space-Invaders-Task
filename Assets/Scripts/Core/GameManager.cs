using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Difficulty difficulty = Difficulty.Normal;
    [SerializeField] private EnemyGridSpawner enemyGridSpawner;
    [SerializeField] private EnemyFormationController enemyFormationController;
    [SerializeField] private float boundaryMargin = 1.5f;

    [SerializeField] private float gizmoDepth = 10f;

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

        float halfWidth = enemyGridSpawner.GridWidth / 2f;
        float leftBoundary = -(halfWidth + boundaryMargin);
        float rightBoundary = halfWidth + boundaryMargin;

        enemyFormationController.Initialize(leftBoundary, rightBoundary);
        enemyFormationController.OnAllEnemiesDefeated += HandleWaveCleared;
    }

    private void HandleWaveCleared()
    {
        Debug.Log("Wave cleared!");
        
    }

    //Testing Stuffs 
    private void OnDrawGizmos()
    {
        if (enemyGridSpawner == null)
            return;

        float previewWidth = enemyGridSpawner.GetPreviewWidth(difficulty);
        float halfWidth = previewWidth / 2f;

        float leftX = -(halfWidth + boundaryMargin);
        float rightX = halfWidth + boundaryMargin;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftX, 0f, 0f), new Vector3(leftX, 0f, gizmoDepth));
        Gizmos.DrawLine(new Vector3(rightX, 0f, 0f), new Vector3(rightX, 0f, gizmoDepth));
    }
}