using UnityEditor;
using UnityEngine;

public class EditorDuplicateCommand : MonoBehaviour
{

    [MenuItem("Champis Toolbox/Duplicate Selected Adjacently #%d")]
    static void DoSomethingWithAShortcutKey()
    {
        GameObject duped = Instantiate(Selection.activeGameObject, Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.rotation, Selection.activeGameObject.transform.parent);
        duped.transform.SetSiblingIndex(Selection.activeGameObject.transform.GetSiblingIndex() + 1);
        duped.name = Selection.activeGameObject.name + " - duplicate";

        Undo.RegisterCreatedObjectUndo(duped, "'" + Selection.activeGameObject.name + "' duplication");

        Selection.SetActiveObjectWithContext(duped, duped);
    }
}
