using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Settings Panel")]
    public GameObject settingsPanel;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Toggle Buttons")]
    public Image musicToggleIcon;
    public Image sfxToggleIcon;

    // Sprites for on/off icons
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private bool musicMuted = false;
    private bool sfxMuted = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Load saved settings
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        float savedSfx = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        sfxMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSfx;

        ApplyMusicVolume(savedMusic);
        ApplySFXVolume(savedSfx);
        ApplyMusicMute(musicMuted);
        ApplySFXMute(sfxMuted);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OnMusicSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        ApplyMusicVolume(value);
    }

    public void OnSFXSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        ApplySFXVolume(value);
    }

    public void ToggleMusic()
    {
        musicMuted = !musicMuted;
        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        ApplyMusicMute(musicMuted);
    }

    public void ToggleSFX()
    {
        sfxMuted = !sfxMuted;
        PlayerPrefs.SetInt("SFXMuted", sfxMuted ? 1 : 0);
        ApplySFXMute(sfxMuted);
    }

    void ApplyMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(musicMuted ? 0 : value);
    }

    void ApplySFXVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(sfxMuted ? 0 : value);
    }

    void ApplyMusicMute(bool muted)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(muted ? 0 :
                PlayerPrefs.GetFloat("MusicVolume", 0.4f));
        if (musicToggleIcon != null)
            musicToggleIcon.sprite = muted ? soundOffSprite : soundOnSprite;
    }

    void ApplySFXMute(bool muted)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(muted ? 0 :
                PlayerPrefs.GetFloat("SFXVolume", 0.7f));
        if (sfxToggleIcon != null)
            sfxToggleIcon.sprite = muted ? soundOffSprite : soundOnSprite;
    }
}