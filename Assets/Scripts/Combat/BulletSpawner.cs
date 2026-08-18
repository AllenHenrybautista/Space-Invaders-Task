using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour, IBulletSpawner
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int initialPoolSize = 1;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float bulletLifetime = 3f;

    private readonly Queue<Bullet> availableBullets = new();
    private readonly HashSet<Bullet> activeBullets = new();

    private void Awake()
    {
        InitializePool();
    }

    public void Spawn(Vector3 position, Vector3 direction)
    {
        Bullet bullet = GetBullet();
        Debug.Log($"Spawn called at {position}, dir {direction}");

        if (bullet == null)
            return;

        activeBullets.Add(bullet);

        bullet.Launch(
            position,
            direction,
            bulletSpeed,
            bulletLifetime
        );
    }

    private void InitializePool()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is not assigned.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            Bullet bullet = CreateBullet();
            availableBullets.Enqueue(bullet);
        }
    }

    private Bullet GetBullet()
    {
        if (availableBullets.Count > 0)
            return availableBullets.Dequeue();

        return null;
    }

    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform);
        bullet.Initialize(ReleaseBullet);

        return bullet;
    }

    private void ReleaseBullet(Bullet bullet)
    {
        if (!activeBullets.Remove(bullet))
            return;

        availableBullets.Enqueue(bullet);
    }
}