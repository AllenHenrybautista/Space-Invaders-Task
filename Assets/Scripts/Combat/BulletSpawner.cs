using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Team team;
    [SerializeField] private Color bulletColor = Color.white;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;

    private Bullet activeBullet;

    private void Awake()
    {
        if (bulletPool == null)
            bulletPool = FindObjectOfType<BulletPool>();
    }

    public void Shoot(Vector3 direction)
    {
        if (bulletPool == null)
            return;

        if (activeBullet != null && activeBullet.gameObject.activeSelf)
            return;

        activeBullet = bulletPool.Get();

        activeBullet.Launch(
            spawnPoint.position,
            direction,
            bulletSpeed,
            bulletLifetime,
            team,
            bulletColor,
            gameObject
        );

        SFXManager.Instance?.ShootClip();
    }
}