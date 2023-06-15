using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class AudioManagerRandomClip : MonoBehaviour
{
    [SerializeField] List<AudioManagerClip> availableClips = new List<AudioManagerClip>();

    public void PlayRandomClip(AudioSource source = null)
    {
        if (availableClips.Count != 0)
            AudioManager.PlaySound(GetRandomClipID(), source);
    }
    public uint GetRandomClipID()
    {
        if (availableClips.Count != 0)
            return (uint)availableClips[UnityEngine.Random.Range(0, availableClips.Count - 1)].clip;
        else
            throw new System.IndexOutOfRangeException("AvailableSounds is 0");
    }

#if UNITY_EDITOR
    #region Drawer
    [CustomEditor(typeof(AudioManagerRandomClip))]
    class AudioManagerRandomEditor : Editor
    {
        AudioManagerRandomClip randomClip;
        ReorderableList reorderableList;

        private void OnEnable()
        {
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("availableClips"), true, true, true, true);

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2.5f, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("clip"));
            };

            reorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, new GUIContent("Available Clips"));
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion
#endif
}