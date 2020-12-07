using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider soundEffectsVolumeSlider;
    public Slider musicVolumeSlider;

    private void Start()
    {
        // Load volume sliders
        masterVolumeSlider.value = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 1f;
        soundEffectsVolumeSlider.value = PlayerPrefs.HasKey("soundEffectsVolume") ? PlayerPrefs.GetFloat("soundEffectsVolume") : 1f;
        musicVolumeSlider.value = PlayerPrefs.HasKey("musicVolume") ? PlayerPrefs.GetFloat("musicVolume") : 1f;
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.Instance.SetMasterMixerVolume(value);
    }

    public void SetSoundEffectsVolume(float value)
    {
        AudioManager.Instance.SetSFXMixerVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicMixerVolume(value);
    }
}
