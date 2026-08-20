using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Clips")]
    [SerializeField] private AudioClip Shootclip;
    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip playerHitClip;
    [SerializeField] private AudioClip waveClearedClip;

    [Header("BGM")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private bool playBGMOnStart = true;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.6f;
    [SerializeField] private float bgmFadeDuration = 1f;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 8;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private readonly Queue<AudioSource> availableSources = new();
    private int previousPlayerHealth = -1;

    private AudioSource bgmSource;
    private Coroutine bgmFadeRoutine;

    public void ShootClip() => PlayClip(Shootclip);
    public void PlayWaveCleared() => PlayClip(waveClearedClip);
    public void PauseBGM() => bgmSource.Pause();
    public void ResumeBGM() => bgmSource.UnPause();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePool();
        SetUpBGMSource();
    }

    private void Start()
    {
        SetupBGM();
    }

    private void OnEnable()
    {
        Enemy.AnyEnemyDied += HandleEnemyDied;
        SceneManager.sceneLoaded += HandleSceneLoaded;

        if (playerHealth != null)
            playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
    }

    private void OnDisable()
    {
        Enemy.AnyEnemyDied -= HandleEnemyDied;
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (playerHealth != null)
            playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;

        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
            previousPlayerHealth = -1;
        }
    }

    private void SetupBGM()
    {
        if (playBGMOnStart && bgmClip != null)
            PlayBGM(bgmClip);
    }

    public void PlayBGM(AudioClip clip = null)
    {
        AudioClip clipToPlay = clip != null ? clip : bgmClip;

        if (clipToPlay == null)
            return;

        if (bgmFadeRoutine != null)
            StopCoroutine(bgmFadeRoutine);

        bgmFadeRoutine = StartCoroutine(CrossfadeBGM(clipToPlay));
    }

    public void StopBGM()
    {
        if (bgmFadeRoutine != null)
            StopCoroutine(bgmFadeRoutine);

        bgmFadeRoutine = StartCoroutine(FadeOutAndStop());
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

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

    private void SetUpBGMSource()
    {
        GameObject bgmObject = new GameObject("BGM_AudioSource");
        bgmObject.transform.SetParent(transform);

        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = 0f;
    }

    private IEnumerator CrossfadeBGM(AudioClip clip)
    {
        if (bgmSource.isPlaying)
            yield return FadeVolume(bgmSource, bgmSource.volume, 0f, bgmFadeDuration);

        bgmSource.clip = clip;
        bgmSource.Play();

        yield return FadeVolume(bgmSource, 0f, bgmVolume, bgmFadeDuration);
    }

    private IEnumerator FadeOutAndStop()
    {
        yield return FadeVolume(bgmSource, bgmSource.volume, 0f, bgmFadeDuration);
        bgmSource.Stop();
    }

    private IEnumerator FadeVolume(AudioSource source, float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            source.volume = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        source.volume = to;
    }
}