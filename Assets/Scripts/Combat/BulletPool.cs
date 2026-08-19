using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int initialPoolSize = 20;

    private readonly Queue<Bullet> availableBullets = new();

    private void Awake()
    {
        InitializePool();
    }

    public Bullet Get()
    {
        if (availableBullets.Count > 0)
            return availableBullets.Dequeue();

        return CreateBullet();
    }

    public void Release(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        availableBullets.Enqueue(bullet);
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            Bullet bullet = CreateBullet();
            availableBullets.Enqueue(bullet);
        }
    }

    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform);
        bullet.Initialize(this);
        bullet.gameObject.SetActive(false);

        return bullet;
    }
}