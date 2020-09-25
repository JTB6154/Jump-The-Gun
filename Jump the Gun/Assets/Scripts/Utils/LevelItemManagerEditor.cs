using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// Ref: https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
// Tells the Editor class that this will be the Editor for the WaveManager
[CustomEditor(typeof(LevelItemManager))]
public class LevelItemManagerEditor : Editor
{
    private Vector2 scrollPos;

    //The array property we will edit
    SerializedProperty levelItems;

    //The Reorderable list we will be working with
    ReorderableList list;

    private void OnEnable()
    {
        //Gets the levelItems property in levelItemManager so we can access it. 
        levelItems = serializedObject.FindProperty("levelItems");

        //Initializes the ReorderableList. We are creating a Reorderable List from the "levelItems" property. 
        //In this, we want a ReorderableList that is draggable, with a display header, with add and remove buttons        
        list = new ReorderableList(serializedObject, levelItems)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = false,           

            onAddCallback = list =>
            {
                list.serializedProperty.arraySize++;
                int newIndex = list.serializedProperty.arraySize - 1;
                SerializedProperty newElement = list.serializedProperty.GetArrayElementAtIndex(newIndex);

                SerializedProperty levelName = newElement.FindPropertyRelative("levelName");
                levelName.stringValue = "";

                SerializedProperty platformCount = newElement.FindPropertyRelative("platformCount");
                platformCount.intValue = 0;
            },

            drawElementCallback = DrawListItems,
            drawHeaderCallback = DrawHeader,
            elementHeight = EditorGUIUtility.singleLineHeight
        };
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); //The element in the list

        // Create a property field and label field for each property. 

        // The 'levelName' property
        // The label field for level (width 100, height of a single line)
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 70, EditorGUIUtility.singleLineHeight), "Level Name");

        //The property field for level (width 80, height of a single line)
        EditorGUI.PropertyField(
            new Rect(rect.x + 75, rect.y, 100, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("levelName"),
            GUIContent.none
        );


        // The 'platformCount' property
        // The label field for platformCount (width 100, height of a single line)
        EditorGUI.LabelField(new Rect(rect.x + 200, rect.y, 95, EditorGUIUtility.singleLineHeight), "Platforms Count");

        EditorGUI.BeginDisabledGroup(true);
        //The property field for platformCount (width 20, height of a single line)
        EditorGUI.PropertyField(
            new Rect(rect.x + 300, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("platformCount"),
            GUIContent.none
        );
        EditorGUI.EndDisabledGroup();

    }

    void DrawHeader(Rect rect)
    {
        string name = "Level List";
        EditorGUI.LabelField(rect, name);
    }

    #region Public Methods

    //This is the function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        list.DoLayoutList();
        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();

        //Debug.Log(list.index);
    }

    public int GetIndex() => list.index;
    public int GetCount() => list.count;
    public void AddItem()
    {
        list.serializedProperty.arraySize++;
        int newIndex = list.serializedProperty.arraySize - 1;
        SerializedProperty newElement = list.serializedProperty.GetArrayElementAtIndex(newIndex);

        SerializedProperty levelName = newElement.FindPropertyRelative("levelName");
        levelName.stringValue = "";

        SerializedProperty platformCount = newElement.FindPropertyRelative("platformCount");
        platformCount.intValue = 0;
    }

    #endregion
}
