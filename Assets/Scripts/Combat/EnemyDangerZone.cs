using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyDangerZone : MonoBehaviour
{
    public event Action OnEnemyReachedZone;

    private bool hasTriggered;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.TryGetComponent(out Enemy enemy))
            return;

        hasTriggered = true;
        OnEnemyReachedZone?.Invoke();
    }
}