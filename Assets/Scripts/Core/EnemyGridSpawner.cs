using UnityEngine;

public class EnemyGridSpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private float horizontalSpacing = 1.5f;
    [SerializeField] private float verticalSpacing = 1.2f;

    private int columns;
    private int rows;

    public float GridWidth => (columns - 1) * horizontalSpacing;

    public void SpawnGrid(Difficulty difficulty)
    {
        (columns, rows) = GetGridSize(difficulty);

        float startX = -GridWidth / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 localPosition = new Vector3(
                    startX + column * horizontalSpacing,
                    0f,
                    row * verticalSpacing
                );

                Enemy enemy = Instantiate(enemyPrefab, transform);
                enemy.transform.localPosition = localPosition;
            }
        }
    }

    private (int columns, int rows) GetGridSize(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => (5, 3),
            Difficulty.Normal => (6, 3),
            Difficulty.Hard => (8, 4),
            _ => (8, 4)
        };
    }

    //Gizmo Stuffs for test
    public float GetPreviewWidth(Difficulty difficulty)
    {
        var (previewColumns, _) = GetGridSize(difficulty);
        return (previewColumns - 1) * horizontalSpacing;
    }
}