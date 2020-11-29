using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

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
        {"Movement/MovingThroughAir",                "event:/Landing" },

        {"Ambience/Ambient1",                        "event:/Ambience/Ambient1" },
    };

    // Handle looping
    private bool isLooping = false;
    private EventInstance loopInstance;
    private EventInstance instance;
    private EventInstance manyMusic;

    private Bus bus;

    [SerializeField] private float masterVolume = 3f;

    #endregion


    public void PlayMusicLoop(string key)
    {
        manyMusic = RuntimeManager.CreateInstance(eventPathsDict[key]);

        // Set volume level
        manyMusic.setVolume(masterVolume);

        manyMusic.start();
    }

    public void StopMusicLoop()
    {
        manyMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        manyMusic.release();
    }

    public void PlaySound(string key)
    {
        instance.release();
        instance = RuntimeManager.CreateInstance(eventPathsDict[key]);

        // Set volume level
        instance.setVolume(masterVolume);

        instance.start();
        instance.release();
    }

    public void PlayLoop(string key)
    {
        if (!isLooping)
        {
            isLooping = true;
            loopInstance = RuntimeManager.CreateInstance(eventPathsDict[key]);

            // Set volume level
            loopInstance.setVolume(masterVolume);

            loopInstance.start();
        }
    }
    private void EndLoopBySettingParameter(string parameterName, float value)
    {
        // Play the end track
        loopInstance.setParameterByName(parameterName, value);
    }

    public void StopLoop()
    {
        if (isLooping)
        {
            EndLoopBySettingParameter("End", 0.5f);

            loopInstance.release();
            isLooping = false;
        }
    }

    public void StopAllSounds()
    {
        bus = RuntimeManager.GetBus("event:/GunSounds");
        bus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
