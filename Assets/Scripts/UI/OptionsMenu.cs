using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Visual Settings")]
    public Toggle postProcessingToggle;
    public TMP_Dropdown resolutionDropdown;

    private int baseWidth = 960;
    private int baseHeight = 720;
    private float[] scaleFactors = { 1f, 1.5f, 2f, 2.5f }; // Scales of the base resolution

    private void Start()
    {
        LoadSettings();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void TogglePostProcessing(bool isEnabled)
    {
        // Assuming you have a post-processing volume to enable/disable
        PlayerPrefs.SetInt("PostProcessing", isEnabled ? 1 : 0);
    }

    public void SetResolution(int index)
    {
        float scale = scaleFactors[index];
        int width = Mathf.RoundToInt(baseWidth * scale);
        int height = Mathf.RoundToInt(baseHeight * scale);
        Screen.SetResolution(width, height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    public void LoadSettings()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        postProcessingToggle.isOn = PlayerPrefs.GetInt("PostProcessing", 1) == 1;
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", 0);
    }
}
