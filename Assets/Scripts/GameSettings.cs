using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider cameraSlider;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource[] sfxSources;

    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel; // THIS panel
    [SerializeField] private GameObject pauseMenuPanel; // parent pause menu panel

    public static float CameraSensitivity { get; private set; } = 1f;

    void Start()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        float camera = PlayerPrefs.GetFloat("CameraSensitivity", 1f);

        musicSlider.value = music;
        sfxSlider.value = sfx;
        cameraSlider.value = camera;

        SetMusicVolume(music);
        SetSFXVolume(sfx);
        SetCameraSensitivity(camera);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        cameraSlider.onValueChanged.AddListener(SetCameraSensitivity);
    }

    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        foreach (var sfx in sfxSources)
        {
            if (sfx != null)
                sfx.volume = value;
        }

        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetCameraSensitivity(float value)
    {
        CameraSensitivity = value;
        PlayerPrefs.SetFloat("CameraSensitivity", value);
    }

    public void BackToPauseMenu()
    {
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}