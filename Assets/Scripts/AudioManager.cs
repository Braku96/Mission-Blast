using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Effects")]
    public AudioClip placeSound;
    public AudioClip clearSound;
    public AudioClip comboSound;
    public AudioClip gameOverSound;
    public AudioClip levelCompleteSound;

    [Header("Music")]
    public AudioClip backgroundMusic;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        // Only one AudioManager exists across all scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.volume = 0.4f;
        musicSource.priority = 0; // Highest priority
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlayPlace()
    {
        if (placeSound != null)
            sfxSource.PlayOneShot(placeSound, 0.7f);
    }

    public void PlayClear(int comboCount)
    {
        if (comboCount > 1 && comboSound != null)
            sfxSource.PlayOneShot(comboSound, 1f);
        else if (clearSound != null)
            sfxSource.PlayOneShot(clearSound, 1f);
    }

    public void PlayGameOver()
    {
        if (gameOverSound != null)
            sfxSource.PlayOneShot(gameOverSound, 1f);
    }
    public void PlayLevelComplete()
    {
        if (levelCompleteSound != null)
            sfxSource.PlayOneShot(levelCompleteSound, 1f);
    }
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}