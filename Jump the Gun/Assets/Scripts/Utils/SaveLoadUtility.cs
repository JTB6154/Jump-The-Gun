using UnityEngine;

public static class SaveLoadUtility
{
    public static string ConvertToJsonString(object obj)
    {
        return JsonUtility.ToJson(obj, false);
    }
}
