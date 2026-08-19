using UnityEngine;

public class EnemyShooter : MonoBehaviour, ITeamMember
{
    [SerializeField] private BulletSpawner bulletSpawner;

    private readonly Vector3 shootDirection = Vector3.back;

    public Team Team => Team.Enemy;

    private void Awake()
    {
        bulletSpawner = GetComponent<BulletSpawner>();

        if (bulletSpawner == null)
            bulletSpawner = GetComponentInChildren<BulletSpawner>();
    }

    public void Shoot()
    {
        if (bulletSpawner == null)
            return;

        bulletSpawner.Shoot(shootDirection);
    }
}