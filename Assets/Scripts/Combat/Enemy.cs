using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, ITeamMember
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int pointValue = 10;

    private int currentHealth;

    public Team Team => Team.Enemy;
    public int PointValue => pointValue;

    public event Action<Enemy> OnDeath;
    public static event Action<Enemy> AnyEnemyDied;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        OnDeath?.Invoke(this);
        AnyEnemyDied?.Invoke(this);
        gameObject.SetActive(false);
    }
}