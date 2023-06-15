using UnityEngine;
using UnityEditor;

public class SmartSelector : EditorWindow
{
    static string selectedTag;
    static int selectedLayer;

    [MenuItem("Champis Toolbox/Smart Selector")]
    public static void OpenSmartSelector()
    {
        EditorWindow.GetWindow(typeof(SmartSelector), false, "Smart Selector", true);
    }

    private void OnGUI()
    {
        selectedTag = EditorGUILayout.TagField("Tag", selectedTag);
        selectedLayer = EditorGUILayout.LayerField("Layer", selectedLayer);

        GUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 2);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Select Tag"))
            SelectObjectsWithTag();

        if (GUILayout.Button("Select Layer"))
            SelectObjectsInLayer();

        GUILayout.EndHorizontal();
    }

    public static void SelectObjectsWithTag()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(selectedTag);
        Selection.objects = objs;

        Debug.Log($"{objs.Length} objects found in '{selectedTag}'");
    }
    public static void SelectObjectsInLayer()
    {
        GameObject[] objs = UniversalFunctions.FindObjectsInLayer(selectedLayer);
        Selection.objects = objs;

        Debug.Log($"{objs.Length} objects found in '{LayerMask.LayerToName(selectedLayer)}'");
    }
}
