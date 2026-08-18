using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Shooting")]
    [SerializeField] private MonoBehaviour bulletSpawnerBehaviour;
    [SerializeField] private float bulletSpawnDistance = 1f;

    private Rigidbody body;
    private IBulletSpawner bulletSpawner;
    private Vector2 moveInput;

    private readonly Vector3 shootDirection = Vector3.up;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();

        bulletSpawner = bulletSpawnerBehaviour as IBulletSpawner;

        if (bulletSpawner == null)
        {
            Debug.LogError(
                $"{nameof(bulletSpawnerBehaviour)} must implement {nameof(IBulletSpawner)}.",
                this
            );
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 movement =
            Vector3.right * moveInput.x * speed * Time.fixedDeltaTime;

        body.MovePosition(body.position + movement);
    }

    private void Shoot()
    {
        if (bulletSpawner == null)
            return;

        Vector3 spawnPosition =
            body.position + shootDirection * bulletSpawnDistance;

        bulletSpawner.Spawn(spawnPosition, shootDirection);
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnFire(InputValue value)
    {
        if (!value.isPressed)
            return;

        Shoot();
    }
}