using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable, ITeamMember
{
    [SerializeField] private int maxLife = 4;

    private int currentLife;
    private bool isDead;

    public Team Team => Team.Player;
    public event Action<int> OnHealthChanged;
    public event Action OnPlayerDied;
    public int CurrentLife => currentLife;
    public int MaxLife => maxLife;

    

    private void Awake()
    {
        currentLife = maxLife;
    }

    private void OnEnable()
    {
        OnHealthChanged?.Invoke(currentLife);
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        currentLife -= amount;
        currentLife = Mathf.Max(currentLife, 0);

        OnHealthChanged?.Invoke(currentLife);

        if (currentLife <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        OnPlayerDied?.Invoke();
    }
}