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

    private int displayIndex = 1;

    private void OnEnable()
    {
        //Gets the levelItems property in levelItemManager so we can access it. 
        levelItems = serializedObject.FindProperty("levelItems");

        //Initializes the ReorderableList. We are creating a Reorderable List from the "levelItems" property. 
        //In this, we want a ReorderableList that is draggable, with a display header, with add and remove buttons        
        list = new ReorderableList(serializedObject, levelItems, false, true, true, true);

        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;

        list.onRemoveCallback = (ReorderableList l) =>
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the level?", "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            }
        };

        list.onAddCallback = (ReorderableList l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("levelName").stringValue = "Level " + displayIndex++;
        };

    }

    private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
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

    private void DrawHeader(Rect rect)
    {
        string name = "Level List";
        EditorGUI.LabelField(rect, name);
    }

    #region Public Methods

    //This is the function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        list.DoLayoutList();
        //EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
    }

    public int GetCount() => list.count;
    public int GetSelectedIndex() => list.index;
    public string GetCurrentSelectedLevelName()
    {
        if (list.index < 0)
            return "No Level Selected";
        var element = list.serializedProperty.GetArrayElementAtIndex(list.index);
        return element.FindPropertyRelative("levelName").stringValue;
    }

    #endregion
}
