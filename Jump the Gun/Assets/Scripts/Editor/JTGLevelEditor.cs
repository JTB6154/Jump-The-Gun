using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class JTGLevelEditor : EditorWindow
{
    [MenuItem("Window/JTG Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<JTGLevelEditor>("JTG Level Editor");
    }

    private void OnGUI()
    {
        // Window code
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "LevelEditorTest")
            return;

        GUILayout.Label("Official 2D Level Editor for Jump The Gun", EditorStyles.miniLabel);

        if (GUILayout.Button("Create a 2D Platform"))
        {
            Debug.Log("A platform has been created!");
        }
    }

    #region Custom Methods

    private void CreatePlatform()
    {

    }

    #endregion
}
