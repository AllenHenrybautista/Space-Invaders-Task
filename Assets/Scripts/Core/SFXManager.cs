using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Clips")]
    [SerializeField] private AudioClip playerShootClip;
    [SerializeField] private AudioClip enemyShootClip;
    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip playerHitClip;
    [SerializeField] private AudioClip waveClearedClip;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 8;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private readonly Queue<AudioSource> availableSources = new();
    private int previousPlayerHealth = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePool();
    }

    private void OnEnable()
    {
        Enemy.AnyEnemyDied += HandleEnemyDied;

        if (playerHealth != null)
            playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
    }

    private void OnDisable()
    {
        Enemy.AnyEnemyDied -= HandleEnemyDied;

        if (playerHealth != null)
            playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;
    }

    public void PlayPlayerShoot() => PlayClip(playerShootClip);
    public void PlayEnemyShoot() => PlayClip(enemyShootClip);
    public void PlayWaveCleared() => PlayClip(waveClearedClip);

    private void HandleEnemyDied(Enemy enemy)
    {
        PlayClip(enemyDeathClip);
    }

    private void HandlePlayerHealthChanged(int currentHealth)
    {
        if (previousPlayerHealth == -1)
        {
            previousPlayerHealth = currentHealth;
            return;
        }

        if (currentHealth < previousPlayerHealth)
            PlayClip(playerHitClip);

        previousPlayerHealth = currentHealth;
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;

        AudioSource source = GetSource();
        source.clip = clip;
        source.volume = sfxVolume;
        source.Play();

        StartCoroutine(ReleaseAfterPlay(source, clip.length));
    }

    private AudioSource GetSource()
    {
        if (availableSources.Count > 0)
            return availableSources.Dequeue();

        return CreateSource();
    }

    private AudioSource CreateSource()
    {
        GameObject sourceObject = new GameObject("SFX_AudioSource");
        sourceObject.transform.SetParent(transform);

        AudioSource source = sourceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;

        return source;
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
            availableSources.Enqueue(CreateSource());
    }

    private IEnumerator ReleaseAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        availableSources.Enqueue(source);
    }
}
