using System.Collections.Generic;
using UnityEngine;

public class EnemySwarmShooter : MonoBehaviour
{
    [SerializeField] private EnemyFormationController formation;
    [SerializeField] private float fireInterval = 1f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < fireInterval)
            return;

        timer = 0f;
        FireFromRandomEnemy();
    }

    private void FireFromRandomEnemy()
    {
        IReadOnlyList<Enemy> aliveEnemies = formation.AliveEnemies;

        if (aliveEnemies.Count == 0)
            return;

        int index = Random.Range(0, aliveEnemies.Count);
        Enemy enemy = aliveEnemies[index];

        if (enemy.TryGetComponent(out EnemyShooter shooter))
            shooter.Shoot();
    }
}