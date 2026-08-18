using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody player;

    [Header("Shooting")]
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletSpawnDistance = 1f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Pooling")]
    [SerializeField] private int initialPoolSize = 10;

    private Vector2 moveInput;
    private PlayerInput playerInput;

    private List<GameObject> bulletPool;
    private Dictionary<GameObject, Coroutine> bulletCoroutines;

    private readonly Vector3 shootDirection = Vector3.up;
    // -- Unity methods -- 
    void Start()
    {
        SetupPlayer();
        InitializeBulletPool();
    }

    void OnEnable()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.onActionTriggered += OnActionTriggered;
    }

    void OnDisable()
    {
        if (playerInput != null) playerInput.onActionTriggered -= OnActionTriggered;
    }

    void Update()
    {
        MovePlayer();
        HandleShooting();
    }


    //-- Player logic --
    private void SetupPlayer()
    {
        player = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
            Debug.LogWarning("PlayerInput component not found on player GameObject.");
    }

    private void MovePlayer()
    {
        float moveX = moveInput.x;

        if (Mathf.Approximately(moveX, 0f) && Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveX = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveX = 1f;
        }

        if (Mathf.Approximately(moveX, 0f) && Gamepad.current != null)
        {
            float stickX = Gamepad.current.leftStick.ReadValue().x;
            if (!Mathf.Approximately(stickX, 0f))
                moveX = stickX;
        }

      

        float delta = moveX * speed * Time.deltaTime;
        Vector3 target = player.position + new Vector3(delta, 0f, 0f);
        player.MovePosition(target);
    }


    //-- Shooting Logics --
    private bool IsFirePressed()
    {
        return Keyboard.current?.spaceKey.wasPressedThisFrame == true
            || Gamepad.current?.buttonSouth.wasPressedThisFrame == true;
    }

    private void HandleShooting()
    {
        if (IsFirePressed()) SpawnBullet();
    }

    private void InitializeBulletPool()
    {
        bulletPool = new List<GameObject>(initialPoolSize);
        bulletCoroutines = new Dictionary<GameObject, Coroutine>();

        if (bulletPrefab == null)
            bulletPrefab = Resources.Load<GameObject>("Bullet");

        if (bulletPrefab == null)
        {
            Debug.LogError("Missing Bullet Prefab!!!");
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject go = Instantiate(bulletPrefab);
            go.SetActive(false);
            bulletPool.Add(go);
            bulletCoroutines[go] = null;
        }
    }

    private GameObject GetPooledBullet()
    {
        if (bulletPool == null) return null;

      
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
                return bulletPool[i];
        }

        return null;
    }

    private void SpawnBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Cannot spawn bullet – prefab is null.");
            return;
        }

        GameObject bullet = GetPooledBullet();
        if (bullet == null)
        {
            return;
        }

        if (bulletCoroutines.ContainsKey(bullet) && bulletCoroutines[bullet] != null)
        {
            StopCoroutine(bulletCoroutines[bullet]);
            bulletCoroutines[bullet] = null;
        }


        bullet.transform.position = player.position + shootDirection * bulletSpawnDistance;
        bullet.transform.rotation = Quaternion.identity;

        bullet.SetActive(true);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.linearVelocity = shootDirection * bulletSpeed;
        }

        bulletCoroutines[bullet] = StartCoroutine(ReturnBulletToPoolAfter(bullet, bulletLifetime));
    }

    private IEnumerator ReturnBulletToPoolAfter(GameObject bullet, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (bullet == null) yield break;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        bullet.SetActive(false);
        bulletCoroutines[bullet] = null;
    }


    //-- Debugging miscs -- 
    private void OnActionTriggered(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Action: {ctx.action.name} | Phase: {ctx.phase}");
    }
}