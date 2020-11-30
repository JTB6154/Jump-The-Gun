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
        masterVolumeSlider.value = AudioManager.Instance.MasterVolume;
        soundEffectsVolumeSlider.value = AudioManager.Instance.SoundEffectsVolume;
        musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.Instance.MasterVolume = value;
    }

    public void SetSoundEffectsVolume(float value)
    {
        AudioManager.Instance.SoundEffectsVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.MusicVolume = value;
    }
}
