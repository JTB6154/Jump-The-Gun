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

    private string globalTextureName = "_GlobalRefractionTex";

    private void Update()
    {
        GenerateRT();
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
