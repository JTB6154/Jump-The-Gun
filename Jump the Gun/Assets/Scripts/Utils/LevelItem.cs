using System;
using System.Collections.Generic;

[Serializable]
public class LevelItem
{
    public string levelName;    // Name of the level
    public int platformCount;   // Number of platforms in the level

    public List<Platform> platforms;

    public LevelItem(string _levelName)
    {
        levelName = _levelName;
        platforms = new List<Platform>();
        UpdatePlatformCount();
    }

    public void UpdatePlatformCount()
    {
        platformCount = platforms.Count;
    }
}

[Serializable]
public struct Platform
{
    public string name;
    public string ssControllerData;
}