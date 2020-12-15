using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GraphicsOptions : MonoBehaviour
{
    [SerializeField] Dropdown resolutionDropDown;
    [SerializeField] Toggle fullscreenCheck;
    Resolution[] resolutions;


    void Start()
    {
        resolutions = Screen.resolutions.Select(Res => new Resolution { width = Res.width, height = Res.height }).Distinct().ToArray();
        int resolutionIndex = -1;

        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Options.Instance.CurrentRes.width && resolutions[i].height == Options.Instance.CurrentRes.height)
            {
                resolutionIndex = i;
            }

        }

        fullscreenCheck.isOn = Options.Instance.IsFullScreen;

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = resolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Options.Instance.CurrentRes = res;
        Options.Instance.SaveResolution(res);
        Screen.SetResolution(res.width,res.height,Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Options.Instance.IsFullScreen = isFullscreen;
        Options.Instance.SaveFullScreen();
        Screen.fullScreen = isFullscreen;
    }

 
}
