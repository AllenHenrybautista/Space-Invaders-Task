using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
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

    private void Move()
    {
        Vector3 movement =
            Vector3.right * moveInput.x * speed * Time.fixedDeltaTime;

        body.MovePosition(body.position + movement);
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnAttack(InputValue value)
    {
        if (!value.isPressed)
            return;
        Debug.Log("Fire");
        bulletSpawner.Shoot(Vector3.up);
    }
}