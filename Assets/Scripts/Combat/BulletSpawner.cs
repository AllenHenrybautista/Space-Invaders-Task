using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    //To Support Shared pool of bullets for both player and enemy (expansion purpose as well)
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Team team;
    [SerializeField] private Color bulletColor = Color.white;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;

    public void Shoot(Vector3 direction)
    {
        if (bulletPool == null)
            return;

        Bullet bullet = bulletPool.Get();
        bullet.Launch(
            spawnPoint.position,
            direction,
            bulletSpeed,
            bulletLifetime,
            team,
            bulletColor,
            gameObject
        );
    }
}