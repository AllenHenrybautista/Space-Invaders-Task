using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxLife = 4;
    public Team Team => Team.Player;

    private int currentLife;

    private void Awake()
    {
        currentLife = maxLife;
    }

    public void TakeDamage(int amount)
    {
        currentLife -= amount;
        currentLife = Mathf.Max(currentLife, 0);

        Debug.Log($"Player hit! Life remaining: {currentLife}");

        if (currentLife <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Game Over :(");
        Time.timeScale = 0f;
        //will add a game over screen later, for now just pause the game
    }
}