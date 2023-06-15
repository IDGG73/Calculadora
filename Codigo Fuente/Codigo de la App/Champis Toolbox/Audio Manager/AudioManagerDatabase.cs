using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Audio Database", menuName = "Champis/Audio Manager/Database")]
public class AudioManagerDatabase : ScriptableObject
{
    public AudioFile[] clips;

#if UNITY_EDITOR
    [CustomEditor(typeof(AudioManagerDatabase))]
    public class AudioManagerDatabaseEditor : Editor
    {
        Texture2D diceOff = null;
        Texture2D diceOn = null;

        GUIStyle diceOffStyle;
        GUIStyle diceOnStyle;

        GUIContent diceGUIContent;

        UnityEditorInternal.ReorderableList reorderableList;

        private void Awake()
        {
            if (diceOff == null)
            {
                diceOff = Resources.Load<Texture2D>("DiceOff");
                diceOn = Resources.Load<Texture2D>("DiceOn");
            }

            diceOffStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,

                normal = new GUIStyleState()
                {
                    background = diceOff
                },
                hover = new GUIStyleState()
                {
                    background = diceOff
                },
                active = new GUIStyleState()
                {
                    background = diceOff
                },
                onActive = new GUIStyleState()
                {
                    background = diceOff
                }
            };
            diceOnStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,

                normal = new GUIStyleState()
                {
                    background = diceOn
                },
                hover = new GUIStyleState()
                {
                    background = diceOn
                },
                active = new GUIStyleState()
                {
                    background = diceOn
                },
                onActive = new GUIStyleState()
                {
                    background = diceOn
                }
            };

            diceGUIContent = new GUIContent(string.Empty, "Random Between A Range");
        }

        public void OnEnable()
        {
            reorderableList = new UnityEditorInternal.ReorderableList(serializedObject, serializedObject.FindProperty("clips"), true, true, true, true);
            reorderableList.multiSelect = true;

            reorderableList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "Database Clips");
                EditorGUI.IntField(new Rect(rect.width - 15f, rect.y + 1.25f, 40f, rect.height - 2.5f), reorderableList.count);
            };

            reorderableList.elementHeightCallback += (index) =>
            {
                bool foldout = reorderableList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("foldout").boolValue;

                return foldout ? EditorGUIUtility.singleLineHeight * 4.5f : EditorGUIUtility.singleLineHeight + 1f;
            };

            reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                bool foldout = element.FindPropertyRelative("foldout").boolValue;
                bool randomVol = element.FindPropertyRelative("randomVolume").boolValue;
                bool randomPitch = element.FindPropertyRelative("randomPitch").boolValue;

                foldout = EditorGUI.Foldout(new Rect(rect.x + 15f, rect.y, rect.width, EditorGUIUtility.singleLineHeight), foldout, new GUIContent(element.displayName), true);

                if (foldout)
                {
                    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y - 8f, 30f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("UniqueID"), new GUIContent(string.Empty, "Unique ID"), true);
                    EditorGUI.PropertyField(new Rect(rect.x + 40f, rect.y, rect.width - 145f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("clipName"), GUIContent.none, true);
                    EditorGUI.PropertyField(new Rect(rect.x + 40f + (rect.width - 145) + 5f, rect.y, 100f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("clip"), GUIContent.none, true);

                    //Volume Random Bools
                    if(!randomVol)
                        randomVol = GUI.Toggle(new Rect(rect.x + 60f, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 20f, EditorGUIUtility.singleLineHeight), randomVol, diceGUIContent, diceOffStyle);
                    else
                        randomVol = GUI.Toggle(new Rect(rect.x + 60f, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 20f, EditorGUIUtility.singleLineHeight), randomVol, diceGUIContent, diceOnStyle);

                    //Pitch Random Bools
                    if (!randomPitch)
                        randomPitch = GUI.Toggle(new Rect(rect.x + 60f, rect.y + EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing, 20f, EditorGUIUtility.singleLineHeight), randomPitch, diceGUIContent, diceOffStyle);
                    else
                        randomPitch = GUI.Toggle(new Rect(rect.x + 60f, rect.y + EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing, 20f, EditorGUIUtility.singleLineHeight), randomPitch, diceGUIContent, diceOnStyle);

                    //Volume & Pitch Labels
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 80f, EditorGUIUtility.singleLineHeight), "Volume");
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing, 80f, EditorGUIUtility.singleLineHeight), "Pitch");

                    //Volume & Pitch Sliders
                    EditorGUI.PropertyField(new Rect(rect.x + 85f, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width - 85f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("volume"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 85f, rect.y + EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 2f, rect.width - 85f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pitch"), GUIContent.none);

                    //Volume & Pitch Range Sliders
                    EditorGUI.PropertyField(new Rect(rect.x + 85f, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width - 85f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("randomVolumeRange"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 85f, rect.y + EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 2f, rect.width - 85f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("randomPitchRange"), GUIContent.none);

                    //Separator Line
                    //EditorGUI.DrawRect(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3.5f, rect.width, 1.25f), new Color(0.35f, 0.35f, 0.35f, 1f));
                }

                element.FindPropertyRelative("foldout").boolValue = foldout;
                element.FindPropertyRelative("randomVolume").boolValue = randomVol;
                element.FindPropertyRelative("randomPitch").boolValue = randomPitch;
            };

            reorderableList.onAddCallback += (list) =>
            {
                int index = list.serializedProperty.arraySize;

                list.serializedProperty.arraySize++;
                list.index = index;

                var element = list.serializedProperty.GetArrayElementAtIndex(index);

                element.FindPropertyRelative("UniqueID").intValue = index;
            };
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
