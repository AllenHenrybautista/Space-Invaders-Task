using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour, ITeamMember
{
    [SerializeField] private Renderer bulletRenderer;
    [SerializeField] private int damage = 1;
    [SerializeField] private int bulletSpeed = 10;

    private Rigidbody body;
    private BulletPool pool;
    private Coroutine lifetimeRoutine;
    private Team ownerTeam;
    private GameObject ownerObject;
    private MaterialPropertyBlock propertyBlock;

    public Team OwnerTeam => ownerTeam;
    public Team Team => Team.Enemy;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void Initialize(BulletPool bulletPool)
    {
        pool = bulletPool;
        gameObject.SetActive(false);
    }

    public void Launch(
        Vector3 position,
        Vector3 direction,
        float bulletSpeed,
        float lifetime,
        Team team,
        Color color,
        GameObject owner)
    {
        ownerTeam = team;
        ownerObject = owner;
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(direction);
        SetColor(color);

        gameObject.SetActive(true);

        body.linearVelocity = Vector3.zero;
        body.linearVelocity = direction.normalized * bulletSpeed;

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
        pool.Release(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject == ownerObject)
            return;

        Bullet otherBullet = other.GetComponent<Bullet>();
        if (otherBullet != null)
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            bool isFriendly = other.TryGetComponent(out ITeamMember targetMember)
                && targetMember.Team == ownerTeam;

            if (isFriendly)
                return;

            damageable.TakeDamage(damage);
            Release();
            return;
        }

        Release();
    }

    private void SetColor(Color color)
    {
        bulletRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", color);
        bulletRenderer.SetPropertyBlock(propertyBlock);
    }

    private IEnumerator LifetimeRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        lifetimeRoutine = null;
        Release();
    }
}