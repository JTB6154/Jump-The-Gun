using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using System.Linq;
using UnityEditor.ShortcutManagement;

public class JTGLevelEditor : EditorWindow
{
    // Editor window instance
    private static JTGLevelEditor _editorInstance;

    // Toolbar (tabs)
    private int toolbarInt = 0;
    private readonly string[] toolbarStrings = { "Level Editing", "Level Save/Load" };

    // Level Editing - Variables
    private readonly string localPathToController = "Assets/Prefabs/LevelEditorPrefabs/SpriteShapeController.prefab";
    private static int platformCounter = 0;
    private GameObject spriteShapePrefab;
    private string spriteShapeName = "Platform";

    // Level Save/Load - Variables
    private readonly string pathToSaveLoad = "/Resources/Levels/";
    private Level_Data levelData;
    private int currentLevelIndex = 0;      // Level number - 1
    private LevelItemManagerEditor levelItemEditor;

    [MenuItem("Tools/JTG Level Editor")]
    [Shortcut("Refresh Window", KeyCode.F12)]
    public static void ShowWindow()
    {
        if (_editorInstance) _editorInstance.Close();

        _editorInstance = GetWindow<JTGLevelEditor>("JTG Level Editor");

        // Initialization
        _editorInstance.levelData = new Level_Data();
        _editorInstance.levelData.levelCount = 5;
        var ff = FindObjectOfType<LevelItemManager>();
        _editorInstance.levelItemEditor = (LevelItemManagerEditor)Editor.CreateEditor(ff);
    }

    void OnInspectorUpdate()
    {
        // Called at 10 frames per second
        Repaint();
    }

    private void OnGUI()
    {
        #region Only Run in LevelEditorTest Scene
        // Make sure the editor only runs in level editor scene
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "LevelEditorTest")
            return;
        #endregion

        GUILayout.BeginVertical();

        // A short description of this editor window
        GUILayout.BeginVertical("box");
        GUILayout.Label("Official 2D Level Editor for Jump The Gun", EditorStyles.miniLabel);
        GUILayout.Label("© 2020 Cardboard Gamers", EditorStyles.miniLabel);
        GUILayout.Label("Press F12 to Refresh the Window", EditorStyles.label);
        GUILayout.EndVertical();

        // Switching tab
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

        switch (toolbarStrings[toolbarInt])
        {
            case "Level Editing":
                GUILayout.BeginVertical("box");

                // 1 - Selecting Prefab
                GUILayout.Label("Select Prefab", EditorStyles.boldLabel);
                spriteShapePrefab = EditorGUILayout.ObjectField("Sprite Shape Prefab", spriteShapePrefab, typeof(GameObject), true, GUILayout.MaxWidth(400)) as GameObject;

                if (GUILayout.Button("Get Default Sprite Shape", GUILayout.MaxWidth(250)))
                {
                    // Load the sprite shape prefab from the Assets folder
                    spriteShapePrefab = AssetDatabase.LoadAssetAtPath(localPathToController, typeof(GameObject)) as GameObject;
                }

                GUILayout.Space(10);

                // 2 - Create Sprite Shape
                GUILayout.Label("Add Sprite Shape to Scene", EditorStyles.boldLabel);
                spriteShapeName = EditorGUILayout.TextField("Sprite Shape Name", spriteShapeName, GUILayout.MaxWidth(400));

                if (GUILayout.Button("Create a Sprite Shape", GUILayout.MaxWidth(250)))
                {
                    CreatePlatform(spriteShapePrefab, spriteShapeName);
                }

                GUILayout.EndVertical();
                break;

            case "Level Save/Load":
                GUILayout.BeginVertical("box");

                // 1 - Directory path to store level files
                GUILayout.Label("Path to Save/Load", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Relative Path", pathToSaveLoad, GUILayout.MaxWidth(400));
                EditorGUI.EndDisabledGroup();

                // 2 - Level files list
                GUILayout.Label("Level List Info", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Total Level Count", levelData.levelCount.ToString(), GUILayout.MaxWidth(400));
                EditorGUI.EndDisabledGroup();
                // ============================ List code below ============================
                levelItemEditor.OnInspectorGUI();


                // 3 - Save/Load buttons
                if (GUILayout.Button("Save Level", GUILayout.MaxWidth(250)))
                {
                    Debug.Log("Saved");
                    SaveLevelData();
                }
                if (GUILayout.Button("Load Level", GUILayout.MaxWidth(250)))
                {
                    Debug.Log("Loaded");
                }

                GUILayout.EndVertical();
                break;
        }

        GUILayout.EndVertical();
    }


    #region Custom Methods

    /// <summary>
    /// Create a platform sprite shape in the active scene.
    /// </summary>
    private void CreatePlatform(GameObject _platformPrefab, string _name)
    {
        // Check if the prefab is null
        if (_platformPrefab == null)
        {
            Debug.LogError("Sprite Shape Prefab is null.");
            return;
        }

        // Check if the prefab is valid
        var ssr = _platformPrefab.GetComponent<SpriteShapeRenderer>();
        var ssc = _platformPrefab.GetComponent<SpriteShapeController>();
        var ec2D = _platformPrefab.GetComponent<EdgeCollider2D>();
        if (!ssr || !ssc || !ec2D)
        {
            Debug.LogError("The Sprite Shape Prefab needs to have SpriteShapeRenderer, SpriteShapeController, and EdgeCollider2D components.");
            return;
        }

        // Instantiate the prefab in active scene
        string platformName = _name + "_" + (++platformCounter).ToString("00"); // Increment the platform counter to avoid naming conflicts
        GameObject newPlatform = PrefabUtility.InstantiatePrefab(_platformPrefab) as GameObject;
        newPlatform.name = platformName;

        // Unpack the prefab completely so that the user won't mess up with the default asset
        PrefabUtility.UnpackPrefabInstance(newPlatform, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        //Debug.Log(platformName + " was created.");   // testing
    }

    /// <summary>
    /// Save the serialized GameObjects as xml file.
    /// </summary>
    /// https://stackoverflow.com/questions/36852213/how-to-serialize-and-save-a-gameobject-in-unity
    private void SaveLevelData()
    {
        // Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
        FileStream file = File.Create(Application.dataPath + pathToSaveLoad + "/Levels.dat");

        // Find all GameObjects (SpriteShapes) in scene
        List<GameObject> ssInScene = FindAllSpriteShapesInScene();

        // Get Sprite Shape Renderer/Controller components info
        levelData.SpriteShapeControllers = new List<string>[levelData.levelCount];
        foreach (GameObject ss in ssInScene)
        {
            List<string> currSSControllerList = levelData.SpriteShapeControllers[currentLevelIndex];
            if (currSSControllerList == null)
                currSSControllerList = new List<string>();
            currSSControllerList.Add(SaveLoadUtility.ConvertToJsonString(ss.GetComponent<SpriteShapeController>()));
        }

        // Save the data

        // ============================ Serialization Step Below ============================
        // Serialize to xml
        DataContractSerializer bf = new DataContractSerializer(levelData.GetType());
        MemoryStream streamer = new MemoryStream();

        // Serialize the file
        bf.WriteObject(streamer, levelData);
        streamer.Seek(0, SeekOrigin.Begin);

        // Save to disk
        file.Write(streamer.GetBuffer(), 0, streamer.GetBuffer().Length);

        // Close the file to prevent any corruptions
        file.Close();

        // PRINT RESULT
        //string result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer()).Replace("\0", "")).ToString();
        //Debug.Log("Serialized Result: " + result);
    }

    private List<GameObject> FindAllSpriteShapesInScene()
    {
        GameObject[] all = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        IEnumerable<GameObject> spriteShapesInScene = all.Where(obj => obj.CompareTag("SpriteShape"));

        List<GameObject> output = spriteShapesInScene.ToList();

        return output;
    }

    #endregion

}

/// <summary>
/// Class to be serialized to a saved level file
/// </summary>
[DataContract]
class Level_Data
{
    [DataMember]
    private List<GameObject>[] _levels;

    [DataMember]
    private List<string>[] _ssControllers;

    public List<GameObject>[] Levels { get => _levels; set => _levels = value; }
    public List<string>[] SpriteShapeControllers { get => _ssControllers; set => _ssControllers = value; }

    [DataMember]
    public int levelCount;
}