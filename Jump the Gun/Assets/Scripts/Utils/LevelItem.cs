[System.Serializable]
public struct LevelItem
{
    public string levelName;    // Name of the level
    public int platformCount;   // Number of platforms in the level

    public LevelItem(string _levelName, int _platformCount)
    {
        this.levelName = _levelName;
        this.platformCount = _platformCount;
    }
}
