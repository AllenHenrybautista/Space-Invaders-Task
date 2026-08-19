using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Tilt")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float maxTiltAngle = 25f;
    [SerializeField] private float tiltSpeed = 8f;

    [Header("Shooting")]
    [SerializeField] private BulletSpawner bulletSpawner;

    private Rigidbody body;
    private Vector2 moveInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        Tilt();
    }

    private void Move()
    {
        Vector3 movement =
            Vector3.right * moveInput.x * speed * Time.fixedDeltaTime;

        body.MovePosition(body.position + movement);
    }

    private void Tilt()
    {
        if (visualRoot == null)
            return;

        float targetAngle = -moveInput.x * maxTiltAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        visualRoot.localRotation = Quaternion.Slerp(
            visualRoot.localRotation,
            targetRotation,
            tiltSpeed * Time.deltaTime
        );
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnAttack(InputValue value)
    {
        if (!value.isPressed)
            return;

        bulletSpawner.Shoot(Vector3.forward);
    }
}