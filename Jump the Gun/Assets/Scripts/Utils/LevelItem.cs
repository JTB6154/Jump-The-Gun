using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelItem
{
    public string levelName;    // Name of the level
    public int platformCount;   // Number of platforms in the level

    public List<GameObject> platforms;

    public LevelItem(string _levelName)
    {
        levelName = _levelName;
        platforms = new List<GameObject>();
        UpdatePlatformCount();
    }

    public void UpdatePlatformCount()
    {
        platformCount = platforms.Count;
    }

    public void AddPlatform(GameObject _platform)
    {
        platforms.Add(_platform);
        UpdatePlatformCount();
    }

    public void UpdatePlatforms(List<GameObject> _platforms)
    {
        platforms.Clear();
        platforms = _platforms;
        UpdatePlatformCount();
    }
}
