using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptions : MonoBehaviour
{
    [SerializeField] Dropdown resolutionDropDown;
    Resolution[] resolutions;
    void Start()
    {
        resolutions = Screen.resolutions;
        int resolutionIndex = -1;

        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                resolutionIndex = i;
            }

        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = resolutionIndex;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width,res.height,Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

 
}
