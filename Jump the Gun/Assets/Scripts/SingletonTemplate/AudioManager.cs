using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private readonly Dictionary<string, string> eventPathsDict = new Dictionary<string, string>
    {
        {"Rocket/RocketExploding",                   "event:/Rocket/RocketExploding" },
        {"Rocket/RocketShooting",                    "event:/Rocket/RocketShooting" },
        {"Rocket/RocketTraveling",                   "event:/Rocket/RocketTraveling" },

        {"Shotgun/ShotgunShooting",                  "event:/ShotgunShooting" },

        {"Movement/Walking",                         "event:/Walking" },
        {"Movement/Landing",                         "event:/Landing" },
        {"Movement/MovingThroughAir",                "event:/MovingThroughAir" },

        {"Ambience/Ambient1",                        "event:/Ambience/Ambient1" },
    };

    private Dictionary<string, bool> loopingDict = new Dictionary<string, bool>();

    // Handle looping
    [SerializeField] private bool isLooping = false;
    private EventInstance loopInstance;
    private EventInstance oneShotInstance;
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

    protected override void Awake()
    {
        base.Awake();

        playerSoundsBus = RuntimeManager.GetBus("bus:/PlayerSounds");
        airTravelBus = RuntimeManager.GetBus("bus:/AirTravel");
        backgroundMusicBus = RuntimeManager.GetBus("bus:/BackgroundMusic");
        gunSoundsBus = RuntimeManager.GetBus("bus:/GunSounds");

        MasterVolume = 1f;

        // Start off with playerSounds Bus muted
        SetPlayerSoundsBusMute(true);
    }

    // Play one-time sound
    public void PlaySound(string path)
    {
        oneShotInstance.release();
        oneShotInstance = RuntimeManager.CreateInstance(eventPathsDict[path]);

        // Set volume level
        oneShotInstance.setVolume(GetVolumeLevel(soundEffectsVolume));

        oneShotInstance.start();
        oneShotInstance.release();
    }

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

            loopInstance = RuntimeManager.CreateInstance(eventPathsDict[path]);

            // Set volume level
            loopInstance.setVolume(GetVolumeLevel(soundEffectsVolume));

            loopInstance.start();
            loopInstance.release();
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

    // Play looping ambient music
    public void PlayMusic(string path)
    {
        if (!loopingDict.ContainsKey(path))
        {
            loopingDict.Add(path, false);
        }

        if (!loopingDict[path])
        {
            Debug.Log("Start playing " + path);
            loopingDict[path] = true;

            musicInstance = RuntimeManager.CreateInstance(eventPathsDict[path]);

            // Set volume level
            musicInstance.setVolume(GetVolumeLevel(musicVolume));

            musicInstance.start();
            musicInstance.release();
        }
    }

    public void StopMusic(string path, SoundBus bus)
    {
        Debug.Log("Music stops");

        if (!loopingDict.ContainsKey(path))
            return;

        loopingDict[path] = false;

        //StopBusSounds(ConvertToFMODBus(bus));

        musicInstance.release();
    }

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

    #region Private Methods

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
