using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenSpaceRefractions : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private Camera camera;
    private int downResFactor = 1;

    [SerializeField]
    [Range(0, 1)]
    private float refractionVisibility = 0;

    [SerializeField]
    [Range(0, 1)]
    private float refractionMagnitude = 0;

    private string globalTextureName = "_GlobalRefractionTex";
    private string globalVisibilityName = "_GlobalVisibility";
    private string globalMagnitudeName = "_GlobalRefractionMag";

    private void OnEnable()
    {
        GenerateRT();
        Shader.SetGlobalFloat(globalVisibilityName, refractionVisibility);
        Shader.SetGlobalFloat(globalMagnitudeName, refractionMagnitude);
    }

    private void Update()
    {
        Shader.SetGlobalFloat(globalVisibilityName, refractionVisibility);
        Shader.SetGlobalFloat(globalMagnitudeName, refractionMagnitude);
    }

    void GenerateRT()
    {
        camera = GetComponent<Camera>();

        if (camera.targetTexture != null)
        {
            RenderTexture temp = camera.targetTexture;

            camera.targetTexture = null;
            DestroyImmediate(temp);
        }

        camera.targetTexture = new RenderTexture(camera.pixelWidth >> downResFactor, camera.pixelHeight >> downResFactor, 16);
        camera.targetTexture.filterMode = FilterMode.Bilinear;

        Shader.SetGlobalTexture(globalTextureName, camera.targetTexture);
    }
}
