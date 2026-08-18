using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody body;
    private Coroutine lifetimeRoutine;
    private Action<Bullet> releaseAction;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Initialize(Action<Bullet> releaseCallback)
    {
        releaseAction = releaseCallback;
        gameObject.SetActive(false);
    }

    public void Launch(Vector3 position, Vector3 direction, float speed, float lifetime)
    {
        transform.position = position;
        transform.rotation = Quaternion.identity;

        gameObject.SetActive(true);

        body.linearVelocity = Vector3.zero;
        body.linearVelocity = direction.normalized * speed;

        if (lifetimeRoutine != null)
            StopCoroutine(lifetimeRoutine);

        lifetimeRoutine = StartCoroutine(LifetimeRoutine(lifetime));
    }

    public void Release()
    {
        if (!gameObject.activeSelf)
            return;

        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }

        body.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);

        releaseAction?.Invoke(this);
    }

    private IEnumerator LifetimeRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        lifetimeRoutine = null;
        Release();
    }
}