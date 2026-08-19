using UnityEngine;

public class EnemyShooter : MonoBehaviour, ITeamMember
{
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private float fireInterval = 2f;

    private readonly Vector3 shootDirection = Vector3.back;

    //To prevent friendly fire i added this
    public Team Team => Team.Enemy;

    private void OnEnable()
    {
        InvokeRepeating(nameof(Shoot), fireInterval, fireInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Shoot));
    }

    private void Shoot()
    {
        if (bulletSpawner == null)
            return;

        bulletSpawner.Shoot(shootDirection);
    }
}