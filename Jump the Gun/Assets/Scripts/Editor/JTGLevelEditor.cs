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
    private static JTGLevelEditor instance;

    // Toolbar (tabs)
    private int toolbarInt = 0;
    private readonly string[] toolbarStrings = { "Level Editing", "Level Save/Load" };

    // Level Editing - Variables
    private readonly string localPathToController = "Assets/Prefabs/LevelEditorPrefabs/SpriteShapeController.prefab";
    private readonly string localPathToLevelItemManager = "Assets/Prefabs/LevelEditorPrefabs/LevelItemManager.prefab";
    private static int platformCounter = 0;
    private GameObject spriteShapePrefab;
    private string spriteShapeName = "Platform";

    // Level Save/Load - Variables
    private readonly string pathToSaveLoad = "/Resources/Levels/";
    private readonly string fileName = "levels.dat";
    private Level_Data levelData;
    private int levelCount = 5;
    private int currentLevelIndex = 0;      // Level number - 1

    // Level Item Manager
    private GameObject managerObj;
    private LevelItemManager manager;
    private LevelItemManagerEditor levelItemManagerEditor;
    private string currentLoadedLevelName;

    [MenuItem("Tools/JTG Level Editor")]
    [Shortcut("Refresh Window", KeyCode.F12)]
    public static void ShowWindow()
    {
        if (instance) instance.Close();

        instance = GetWindow<JTGLevelEditor>("JTG Level Editor");
    }

    private void Awake()
    {
        // Initialization
        var managerPrefab = AssetDatabase.LoadAssetAtPath(localPathToLevelItemManager, typeof(GameObject)) as GameObject;
        managerObj = PrefabUtility.InstantiatePrefab(managerPrefab) as GameObject;
        PrefabUtility.UnpackPrefabInstance(managerObj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        // Set parent
        var doNotDeleteParent = GameObject.Find("DO NOT DELETE");
        managerObj.transform.parent = doNotDeleteParent.transform;

        manager = managerObj.AddComponent<LevelItemManager>();
        // FindObjectOfType<LevelItemManager>();

        // Load level data from file
        LoadLevelData();

        // Display loaded data in Reorderable list
        levelItemManagerEditor = (LevelItemManagerEditor)Editor.CreateEditor(manager);        
    }

    private void OnDestroy()
    {
        DestroyImmediate(managerObj);
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

                // 1 - Save/Load buttons
                GUILayout.Label("Level File Management", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Relative File Path", pathToSaveLoad, GUILayout.MaxWidth(400));
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Save Levels to File", GUILayout.MaxWidth(200)))
                {
                    SaveLevelData();
                }
                if (GUILayout.Button("Load Levels from File", GUILayout.MaxWidth(200)))
                {
                }

                GUILayout.Space(20);

                // 2 - Level files list
                GUILayout.Label("Level List Info", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Total Level Count", levelItemManagerEditor.GetCount().ToString(), GUILayout.MaxWidth(400));
                EditorGUILayout.TextField("Current Selected Level", levelItemManagerEditor.GetCurrentSelectedLevelName(), GUILayout.MaxWidth(400));
                EditorGUILayout.TextField("Current Loaded Level", currentLoadedLevelName, GUILayout.MaxWidth(400));
                EditorGUI.EndDisabledGroup();

                //  Display Reorderable list GUI
                levelItemManagerEditor.OnInspectorGUI();

                bool areButtonsDisabled = levelItemManagerEditor.GetSelectedIndex() == -1;
                EditorGUI.BeginDisabledGroup(areButtonsDisabled);
                if (GUILayout.Button("Load Data from Current Selected Level", GUILayout.MaxWidth(250)))
                {
                    LoadCurrentSelectedLevel();
                }

                if (GUILayout.Button("Save Data to Current Selected Level \n (Does Not Save to File)", GUILayout.MaxWidth(250)))
                {
                    bool isCurrentLevelSaved = EditorUtility.DisplayDialog("Warning!",
                                            "Are you sure you want to update data in " + levelItemManagerEditor.GetCurrentSelectedLevelName() + "? This will overwrite any previous data.",
                                            "Yes", "No");
                    if (isCurrentLevelSaved)
                    {
                        SaveDataToCurrentLevel();
                    }
                }
                EditorGUI.EndDisabledGroup();

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
        // Find all GameObjects (SpriteShapes) in scene
        List<GameObject> ssInScene = FindAllSpriteShapesInScene();

        // Retrieve Sprite Shape Controller components data from scene
        if (levelData == null)  // no level file was found and data is null
        {
            levelData = new Level_Data();
            levelCount = 5;
            levelData.Levels = new string[levelCount];
        }

        var platformsInScene = FindAllPlatformsInScene(ssInScene);

        levelData.Levels[currentLevelIndex] = JsonHelper.ToJson(platformsInScene, true);
        Debug.Log(levelData.Levels[currentLevelIndex]);

        // TEST CODE

        // ============================ Serialization Step Below ============================
        // Serialize to xml
        DataContractSerializer s = new DataContractSerializer(typeof(Level_Data));

        // Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
        using (FileStream fs = File.Open(Application.dataPath + pathToSaveLoad + fileName, FileMode.Create))
        {
            s.WriteObject(fs, levelData);
            Debug.Log("Saved to file");
        }

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

    private List<Platform> FindAllPlatformsInScene(List<GameObject> ssInScene)
    {
        if (ssInScene == null) return null;
        var output = new List<Platform>();
        foreach (var ss in ssInScene)
        {
            Platform pf = new Platform();
            pf.name = ss.name;
            pf.ssControllerData = JsonUtility.ToJson(ss.GetComponent<SpriteShapeController>(), true);
            Debug.Log(pf.ssControllerData);
            output.Add(pf);
        }
        return output;
    }

    private void LoadLevelData()
    {
        Level_Data loadedLevelData;
        DataContractSerializer s = new DataContractSerializer(typeof(Level_Data));

        try
        {
            FileStream fs = File.Open(Application.dataPath + pathToSaveLoad + fileName, FileMode.Open);
            loadedLevelData = s.ReadObject(fs) as Level_Data;
            if (loadedLevelData == null)
                Debug.LogError("Deserialized level file is NULL");
            else
            {
                levelData = loadedLevelData;

                //string[] s_loadedLevels = JsonHelper.FromJson<string[]> 
            }

            fs.Close();
            Debug.Log("Level Data Loaded!");
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    private void LoadCurrentSelectedLevel()
    {
        // Destroy all sprite shapes in hierarchy
        List<GameObject> ssInScene = FindAllSpriteShapesInScene();
        foreach (var ss in ssInScene)
        {
            DestroyImmediate(ss);
        }

        // Load platforms from level item
        var levelItem = manager.levelItems[levelItemManagerEditor.GetSelectedIndex()];
        var platforms = new List<GameObject>();
        foreach (Platform pf in levelItem.platforms)
        {
            platforms.Add(ConvertPlatformToGameObject(pf));
        }

        foreach (GameObject pfObj in platforms)
        {
            Instantiate(pfObj);
        }

        currentLoadedLevelName = levelItemManagerEditor.GetCurrentSelectedLevelName();  // Update textbox
        Debug.Log(currentLoadedLevelName + " loaded in Scene");
    }

    private void SaveDataToCurrentLevel()
    {
        // Find all GameObjects (SpriteShapes) in scene
        List<GameObject> ssInScene = FindAllSpriteShapesInScene();

        var currentLevelItem = manager.levelItems[levelItemManagerEditor.GetSelectedIndex()];

        currentLevelItem.platforms = FindAllPlatformsInScene(ssInScene);
        currentLevelItem.UpdatePlatformCount();

        Debug.Log("Saved current level");
    }

    private GameObject ConvertPlatformToGameObject(Platform pf)
    {
        var result = new GameObject();

        result.name = pf.name;
        var controllerData = JsonUtility.FromJson<Spline>(pf.ssControllerData);
        result.AddComponent<SpriteShapeRenderer>();
        result.AddComponent<SpriteShapeController>();
        result.GetComponent<SpriteShapeController>().spline = controllerData;

        return result;
    }

    #endregion

}

/// <summary>
/// Class to be serialized to a saved level file (.dat)
/// </summary>
[DataContract]
class Level_Data
{
    [DataMember]
    private string[] _levels;

    public string[] Levels { get => _levels; set => _levels = value; }
}

