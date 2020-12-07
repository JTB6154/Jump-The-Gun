using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using FMODUnity;

public enum SoundBus
{
    AirTravel,
    BackgroundMusic,
    GunSounds,
    PlayerSounds
}

public class AudioManager : Singleton<AudioManager>
{
    #region Fields

    private Dictionary<string, bool> loopingDict = new Dictionary<string, bool>();

    // Handle looping
    [SerializeField] private bool isLooping = false;
    private EventInstance loopInstance;
    private EventInstance musicInstance;

    private Bus airTravelBus;
    private Bus backgroundMusicBus;
    private Bus gunSoundsBus;
    private Bus playerSoundsBus;

    private bool isPlayerSoundsBusMuted;

    private float masterVolume = 1f;
    private float soundEffectsVolume = 3f;
    private float musicVolume = 3f;

    public float MasterVolume { get => masterVolume; set => masterVolume = value; }
    public float SoundEffectsVolume { get => soundEffectsVolume; set => soundEffectsVolume = value; }
    public float MusicVolume { get => musicVolume; set => musicVolume = value; }


    #endregion

    private void Start()
    {
        playerSoundsBus = RuntimeManager.GetBus("bus:/PlayerSounds");
        airTravelBus = RuntimeManager.GetBus("bus:/AirTravel");
        backgroundMusicBus = RuntimeManager.GetBus("bus:/BackgroundMusic");
        gunSoundsBus = RuntimeManager.GetBus("bus:/GunSounds");

        // Set default master volume
        MasterVolume = 1f;

        // Start off with playerSounds Bus muted
        SetPlayerSoundsBusMute(true);
    }

    private void OnDestroy()
    {
        loopInstance.release();
        musicInstance.release();
    }

    #region Custom Public Methods

    /// <summary>
    /// Play a one shot sound
    /// </summary>
    public void PlayOneShot(string path)
    {
        RuntimeManager.PlayOneShot(path);
    }

    // ================================ LOOP =================================
    // Play looping sound
    public void PlayLoop(string path)
    {
        if (!loopingDict.ContainsKey(path))
        {
            loopingDict.Add(path, false);
        }

        if (!loopingDict[path])
        {
            loopingDict[path] = true;

            loopInstance = RuntimeManager.CreateInstance(path);

            // Set volume level
            loopInstance.setVolume(GetVolumeLevel(soundEffectsVolume));

            loopInstance.start();
        }
    }

    public void StopLoop(string path, SoundBus busIndex)
    {
        if (!loopingDict.ContainsKey(path))
            return;

        loopingDict[path] = false;

        StopBusSounds(ConvertToFMODBus(busIndex));

        loopInstance.release();
    }

    public void PauseLoop(SoundBus busIndex)
    {
        ConvertToFMODBus(busIndex).setPaused(true);
    }

    public void UnpauseLoop(SoundBus busIndex)
    {
        ConvertToFMODBus(busIndex).setPaused(false);
    }
    // =======================================================================


    // ================================ MUSIC ================================
    /// <summary>
    /// Start ambient music.
    /// </summary>
    public void StartMusic(string path)
    {
        musicInstance = RuntimeManager.CreateInstance(path);

        // Set volume level
        musicInstance.setVolume(GetVolumeLevel(musicVolume));

        musicInstance.start();
    }

    public void StopMusic(SoundBus bus)
    {
        StopBusSounds(ConvertToFMODBus(bus));
        musicInstance.release();
    }
    // =======================================================================


    public void SetPlayerSoundsBusMute(bool mute)
    {
        if (isPlayerSoundsBusMuted ^ mute)
        {
            //Debug.Log((mute ? "Mute " : "Unmute ") + "Player Sounds Bus.");
            playerSoundsBus.setMute(mute);
            isPlayerSoundsBusMuted = mute;
        }
    }

    public void SetDynamicBusVolume(DynamicBusVolumeController controller, float inputValue)
    {
        ConvertToFMODBus(controller.SoundBusType).setVolume(controller.GetOutputVolume(inputValue));
    }

    #endregion

    #region Utility Private Methods

    private void StopBusSounds(Bus bus)
    {
        bus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private Bus ConvertToFMODBus(SoundBus busIndex)
    {
        switch (busIndex)
        {
            case SoundBus.AirTravel:
                return airTravelBus;
            case SoundBus.BackgroundMusic:
                return backgroundMusicBus;
            case SoundBus.GunSounds:
                return gunSoundsBus;
            case SoundBus.PlayerSounds:
                return playerSoundsBus;
            default:
                return airTravelBus;
        }
    }

    private bool IsEventInstanceNull(EventInstance instance)
    {
        return instance.Equals(default(EventInstance));
    }

    private float GetVolumeLevel(float typeVolume)
    {
        return masterVolume * typeVolume;
    }

    #endregion
}
