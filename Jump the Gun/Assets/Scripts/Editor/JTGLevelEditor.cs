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
using OdinSerializer;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;

public class JTGLevelEditor : EditorWindow
{
    // Editor window instance
    private static JTGLevelEditor instance;

    // Toolbar (tabs)
    private int toolbarInt = 1;
    private readonly string[] toolbarStrings = { "Level Editing", "Level Save/Load" };

    // Level Editing - Variables
    private readonly string localPathToController = "Assets/Prefabs/LevelEditorPrefabs/SpriteShapeController.prefab";
    private readonly string localPathToLevelItemManager = "Assets/Prefabs/LevelEditorPrefabs/LevelItemManager.prefab";
    private static int platformCounter = 0;
    private GameObject spriteShapePrefab;
    private string spriteShapeName = "Platform";

    // Level Save/Load - Variables
    private readonly string pathToSaveLoad = "/Resources/Levels/";

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
        //SaveLevelData();
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
        if (sceneName != "LevelEditorTest" && sceneName != "JTGFullGame")
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

                GUILayout.Space(10);

                // 3 - Clear Scene
                GUILayout.Label("Clear Sprite Shapes in Hierarchy", EditorStyles.boldLabel);

                if (GUILayout.Button("Clear", GUILayout.MaxWidth(250)))
                {
                    ClearSpriteShapesInScene();
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
                if (GUILayout.Button("Load Levels from File", GUILayout.MaxWidth(200)))
                {
                    LoadLevelData();
                }

                GUILayout.Space(20);

                // 2 - Level files list
                GUILayout.Label("Level List Info", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Total Level Count", manager.levelItems.Count.ToString(), GUILayout.MaxWidth(400));
                EditorGUILayout.TextField("Current Selected Level", levelItemManagerEditor.GetCurrentSelectedLevelName(), GUILayout.MaxWidth(400));

                //EditorGUILayout.TextField("DEBUG: Current Selected Index", levelItemManagerEditor.GetSelectedIndex().ToString(), GUILayout.MaxWidth(400));

                EditorGUILayout.TextField("Current Loaded Level", currentLoadedLevelName, GUILayout.MaxWidth(400));
                EditorGUI.EndDisabledGroup();

                //  Display Reorderable list GUI
                levelItemManagerEditor.OnInspectorGUI();

                bool areButtonsDisabled = levelItemManagerEditor.GetSelectedIndex() == -1;
                EditorGUI.BeginDisabledGroup(areButtonsDisabled
                    || levelItemManagerEditor.GetCurrentSelectedLevelName().Equals(currentLoadedLevelName));
                if (GUILayout.Button("Load Current Selected Level", GUILayout.MaxWidth(250)))
                {
                    if (SceneManager.GetActiveScene().isDirty)
                    {
                        bool isCurrentLevelSaved = EditorUtility.DisplayDialog("Warning!",
                        "There are unsaved changes in " + currentLoadedLevelName + ". Do you want to save changes before loading another level?",
                        "Yes", "No");
                        if (isCurrentLevelSaved)
                        {
                            SaveDataToCurrentLevel();
                        }
                    }
                    LoadCurrentSelectedLevel();

                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(areButtonsDisabled);
                if (GUILayout.Button("Save Current Selected Level To File", GUILayout.MaxWidth(250)))
                {
                    bool isCurrentLevelSaved = EditorUtility.DisplayDialog("Warning!",
                                            "Are you sure you want to update data in " + levelItemManagerEditor.GetCurrentSelectedLevelName() + "? This will overwrite any previous data in files.",
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

    private List<GameObject> FindAllSpriteShapesInScene()
    {
        GameObject[] spriteShapesInScene = GameObject.FindGameObjectsWithTag("SpriteShape");

        List<GameObject> output = new List<GameObject>();
        foreach (GameObject spriteShape in spriteShapesInScene)
        {
            output.Add(SerializationUtility.CreateCopy(spriteShape) as GameObject);
        }

        return output;
    }

    /// <summary>
    /// Save current sprite shapes in scene to file.
    /// </summary>
    private void SaveDataToCurrentLevel()
    {
        // First, check if each level's name is valid for using as a filename
        if (IsInvalidFilename(levelItemManagerEditor.GetCurrentSelectedLevelName()))
        {
            Debug.LogError(levelItemManagerEditor.GetCurrentSelectedLevelName() + " is not a valid file name. Please fix it and retry!");
            return;
        }

        List<GameObject> spriteShapesInScene = FindAllSpriteShapesInScene();
        manager.levelItems[levelItemManagerEditor.GetSelectedIndex()].UpdatePlatforms(spriteShapesInScene);

        // Create a list of level game objects
        List<GameObject> levelsToSave = new List<GameObject>();
        foreach (LevelItem level in manager.levelItems)
        {
            GameObject newLevel = new GameObject();
            newLevel.name = level.levelName;
            foreach (GameObject platform in level.platforms)
            {
                GameObject tempPlatform = Instantiate(platform, newLevel.transform);
                tempPlatform.name = tempPlatform.name.Replace("(Clone)", "");
            }
            levelsToSave.Add(newLevel);
        }

        // Check for directory for save/load
        string tempPermPath = Application.dataPath + pathToSaveLoad;
        // If path to save doesn't exist
        if (!Directory.Exists(tempPermPath))
        {
            //Debug.Log("Path not existed");
            Directory.CreateDirectory(tempPermPath);
        }
        else
        {
            // If existed, clear the path and recreate the directory
            FileUtil.DeleteFileOrDirectory(tempPermPath);
            Directory.CreateDirectory(tempPermPath);
        }

        // Save level gameobjects in folder
        foreach (GameObject level in levelsToSave)
        {
            string pathToSave = pathToSaveLoad + level.name;

            bool success;
            //Debug.Log(level);
            PrefabUtility.SaveAsPrefabAsset(level, "Assets" + pathToSave + ".prefab", out success);
            //Debug.Log("Saved " + (success ? "successfully" : "unsuccessfully"));
        }

        for (int i = 0; i < levelsToSave.Count; i++)
        {
            DestroyImmediate(levelsToSave[i]);
        }

        AssetDatabase.Refresh();
    }

    private void LoadLevelData()
    {
        // Clear the list if it was prepopulated
        if (manager.levelItems.Count > 0)
            manager.levelItems.Clear();

        List<GameObject> levelPrefabs = PrefabLoader.LoadAllPrefabsAt("Assets" + pathToSaveLoad);

        foreach (GameObject level in levelPrefabs)
        {
            //Debug.Log(level);
            LevelItem levelItem = new LevelItem(level.name);
            foreach (Transform platformT in level.transform)
            {
                levelItem.AddPlatform(platformT.gameObject);
            }
            manager.levelItems.Add(levelItem);
        }
    }

    private int GetLevelNumInFile()
    {
        return PrefabLoader.LoadAllPrefabsAt("Assets" + pathToSaveLoad).Count;
    }

    private void LoadCurrentSelectedLevel()
    {
        if (levelItemManagerEditor.GetCount() <= GetLevelNumInFile())
            LoadLevelData();

        // Update current loaded level
        currentLoadedLevelName = levelItemManagerEditor.GetCurrentSelectedLevelName();

        // Destroy all sprite shapes in hierarchy
        List<GameObject> ssInScene = FindAllSpriteShapesInScene();
        foreach (var ss in ssInScene)
        {
            DestroyImmediate(ss);
        }

        // Load platforms from level item
        var platforms = manager.levelItems[levelItemManagerEditor.GetSelectedIndex()].platforms;
        foreach (var platform in platforms)
        {
            var newPlatform = Instantiate(platform);
            newPlatform.name = newPlatform.name.Replace("(Clone)", "");
        }

        // Save the scene and make it not "dirty"
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private void ClearSpriteShapesInScene()
    {
        List<GameObject> platformsInScene = FindAllSpriteShapesInScene();
        foreach (GameObject p in platformsInScene)
        {
            DestroyImmediate(p);
        }
    }

    // Ref: https://stackoverflow.com/questions/12590969/determining-whether-a-string-is-a-file-name-in-c-sharp
    private bool IsInvalidFilename(string fileName)
    {
        char[] invalidFileChars = Path.GetInvalidFileNameChars();
        return invalidFileChars.Any(s => fileName.Contains(s));
    }

    #endregion

}
