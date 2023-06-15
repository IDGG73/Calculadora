using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class AudioFile
{
    public string clipName;
    public AudioClip clip;
    [Space]
    [ConditionalHide(nameof(foldout), false, true)] public uint UniqueID;

    //[Header("Properties")]
    public bool randomVolume;
    public bool randomPitch;

    [ConditionalHide(nameof(randomVolume), true), MinMaxSlider(0f, 1f, false)] public Vector2 randomVolumeRange = new Vector2(0f, 1f);
    [ConditionalHide(nameof(randomVolume), true, true), Range(0f, 1f)] public float volume = 1;
    [ConditionalHide(nameof(randomPitch), true), MinMaxSlider(-3f, 3f, false)] public Vector2 randomPitchRange = new Vector2(-3f, 3f);
    [ConditionalHide(nameof(randomPitch), true, true), Range(-3f, 3f)] public float pitch = 1f;

    public bool foldout;
}

[System.Serializable]
public struct AudioManagerClip
{
    [AudioManagerSearchableClipEnum(typeof(AudioManager), nameof(AudioManager.GetSoundsInDatabase))] public uint clip;

    public void Play() => AudioManager.PlaySound(clip);
    public void Play(AudioSource source) => AudioManager.PlaySound(clip, source);

    #region Selector Attribute
    class AudioManagerSelector : PropertyAttribute
    {
        public delegate string[] GetStringList();

        public string[] List { get; private set; }

        public AudioManagerSelector(params string[] list)
        {
            List = list;
        }

        public AudioManagerSelector()
        {
            var method = typeof(AudioManager).GetMethod(nameof(AudioManager.GetSoundsInDatabase));

            if (method != null)
                List = method.Invoke(null, null) as string[];
            else
                Debug.LogError($"Method 'GetSoundsInDatabase' does not exist for 'AudioManager'");
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AudioManagerSelector))]
    class AudioManagerSelectorDrawer : PropertyDrawer
    {
        private string search;
        private string[] options;
        private GUIStyle searchTextFieldStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stringInList = attribute as AudioManagerSelector;
            var list = stringInList.List;

            #region Non-Searchable
            /*
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                uint selectedID = AudioManager.GetIDForIndex(EditorGUI.Popup(position, property.displayName, AudioManager.GetIndexForID((uint)property.intValue), list));
                property.intValue = (int)selectedID;
            }
            else
            {
                base.OnGUI(position, property, label);
            }*/
            #endregion

            #region Searchable
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (searchTextFieldStyle == null)
                    searchTextFieldStyle = GUI.skin.FindStyle("ToolbarSeachTextField");

                if (options == null)
                    UpdateOptions(list);

                Rect searchRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                DrawSearchBar(searchRect, label, list);

                Rect popupRect = new Rect(position.x, searchRect.y + searchRect.height + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
                DrawEnumPopup(popupRect, property);
            }
            else
            {
                base.OnGUI(position, property, label);
            }
            #endregion
        }

        private void DrawSearchBar(Rect position, GUIContent label, string[] allOptions)
        {
            EditorGUI.BeginChangeCheck();
            search = EditorGUI.TextField(position, label, search, searchTextFieldStyle);

            if (EditorGUI.EndChangeCheck())
                UpdateOptions(allOptions);
        }

        private void DrawEnumPopup(Rect position, SerializedProperty property)
        {
            var stringInList = attribute as AudioManagerSelector;
            var list = stringInList.List;

            Rect fieldRect = EditorGUI.PrefixLabel(position, new GUIContent(" "));
            int currentIndex = Array.IndexOf(options, list[AudioManager.GetIndexForID((uint)property.intValue)]);
            int selectedIndex = EditorGUI.Popup(fieldRect, currentIndex, options);

            if (selectedIndex >= 0)
            {
                int newIndex = Array.IndexOf(list, options[selectedIndex]);

                if (newIndex != currentIndex)
                {
                    property.intValue = (int)AudioManager.GetIDForIndex(newIndex);

                    search = string.Empty;
                    UpdateOptions(list);
                }
            }
        }

        private void UpdateOptions(string[] allOptions)
        {
            options = Array.FindAll(allOptions, name => string.IsNullOrEmpty(search) || name.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }
    }
#endif
    #endregion

    #region Searchable Attribute
    [AttributeUsage(AttributeTargets.Field)]
    public class AudioManagerSearchableClipEnumAttribute : PropertyAttribute
    {
        public string[] options;

        public AudioManagerSearchableClipEnumAttribute(string[] options = null)
        {
            this.options = options;
        }

        public AudioManagerSearchableClipEnumAttribute(Type type, string methodName)
        {
            var method = type.GetMethod(methodName);

            if (method != null)
                options = method.Invoke(null, null) as string[];
            else
                Debug.LogError($"Method '{methodName}' does not exist for '{type}'");
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AudioManagerSearchableClipEnumAttribute))]
    public class AudioManagerSearchableClipEnumDrawer : PropertyDrawer
    {
        private int idHash;
        string[] options;

        AudioManagerSearchableClipEnumAttribute searchableEnumAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            searchableEnumAttribute = attribute as AudioManagerSearchableClipEnumAttribute;

            options = searchableEnumAttribute.options;

            if (idHash == 0)
                idHash = "AudioManagerSearchableClipEnumDrawer".GetHashCode();

            int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText;

            if (property.intValue < 0 || property.intValue >= options.Length)
                buttonText = new GUIContent();
            else
                buttonText = new GUIContent(options[AudioManager.GetIndexForID((uint)property.intValue)]);

            if (DropdownButton(id, position, buttonText))
            {
                Action<int> onSelect = i =>
                {
                    property.intValue = (int)AudioManager.GetIDForIndex(i);
                    property.serializedObject.ApplyModifiedProperties();
                };

                SearchablePopup.Show(position, options, property.intValue, onSelect);
            }

            EditorGUI.EndProperty();
        }

        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;

                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;

                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;
        }
    }

    public class SearchablePopup : PopupWindowContent
    {
        #region -- Constants --------------------------------------------------
        /// <summary> Height of each element in the popup list. </summary>
        private const float ROW_HEIGHT = 16.0f;

        /// <summary> How far to indent list entries. </summary>
        private const float ROW_INDENT = 8.0f;

        /// <summary> Name to use for the text field for search. </summary>
        private const string SEARCH_CONTROL_NAME = "EnumSearchText";
        #endregion -- Constants -----------------------------------------------

        #region -- Static Functions -------------------------------------------
        /// <summary> Show a new SearchablePopup. </summary>
        /// <param name="activatorRect">
        /// Rectangle of the button that triggered the popup.
        /// </param>
        /// <param name="options">List of strings to choose from.</param>
        /// <param name="current">
        /// Index of the currently selected string.
        /// </param>
        /// <param name="onSelectionMade">
        /// Callback to trigger when a choice is made.
        /// </param>
        public static void Show(Rect activatorRect, string[] options, int current, Action<int> onSelectionMade)
        {
            SearchablePopup win =
                new SearchablePopup(options, current, onSelectionMade);
            PopupWindow.Show(activatorRect, win);
        }

        /// <summary>
        /// Force the focused window to redraw. This can be used to make the
        /// popup more responsive to mouse movement.
        /// </summary>
        private static void Repaint()
        { EditorWindow.focusedWindow.Repaint(); }

        /// <summary> Draw a generic box. </summary>
        /// <param name="rect">Where to draw.</param>
        /// <param name="tint">Color to tint the box.</param>
        private static void DrawBox(Rect rect, Color tint)
        {
            Color c = GUI.color;
            GUI.color = tint;
            GUI.Box(rect, "", Selection);
            GUI.color = c;
        }
        #endregion -- Static Functions ----------------------------------------

        #region -- Helper Classes ---------------------------------------------
        /// <summary>
        /// Stores a list of strings and can return a subset of that list that
        /// matches a given filter string.
        /// </summary>
        private class FilteredList
        {
            /// <summary>
            /// An entry in the filtererd list, mapping the text to the
            /// original index.
            /// </summary>
            public struct Entry
            {
                public int Index;
                public string Text;
            }

            /// <summary> All posibile items in the list. </summary>
            private readonly string[] allItems;

            /// <summary> Create a new filtered list. </summary>
            /// <param name="items">All The items to filter.</param>
            public FilteredList(string[] items)
            {
                allItems = items;
                Entries = new List<Entry>();
                UpdateFilter("");
            }

            /// <summary> The current string filtering the list. </summary>
            public string Filter { get; private set; }

            /// <summary> All valid entries for the current filter. </summary>
            public List<Entry> Entries { get; private set; }

            /// <summary> Total possible entries in the list. </summary>
            public int MaxLength
            { get { return allItems.Length; } }

            /// <summary>
            /// Sets a new filter string and updates the Entries that match the
            /// new filter if it has changed.
            /// </summary>
            /// <param name="filter">String to use to filter the list.</param>
            /// <returns>
            /// True if the filter is updated, false if newFilter is the same
            /// as the current Filter and no update is necessary.
            /// </returns>
            public bool UpdateFilter(string filter)
            {
                if (Filter == filter)
                    return false;

                Filter = filter;
                Entries.Clear();

                for (int i = 0; i < allItems.Length; i++)
                {
                    if (string.IsNullOrEmpty(Filter) || allItems[i].ToLower().Contains(Filter.ToLower()))
                    {
                        Entry entry = new Entry
                        {
                            Index = i,
                            Text = allItems[i]
                        };
                        if (string.Equals(allItems[i], Filter, StringComparison.CurrentCultureIgnoreCase))
                            Entries.Insert(0, entry);
                        else
                            Entries.Add(entry);
                    }
                }
                return true;
            }
        }
        #endregion -- Helper Classes ------------------------------------------

        #region -- Private Variables ------------------------------------------
        /// <summary> Callback to trigger when an item is selected. </summary>
        private readonly Action<int> onSelectionMade;

        /// <summary>
        /// Index of the item that was selected when the list was opened.
        /// </summary>
        private readonly int currentIndex;

        /// <summary>
        /// Container for all available options that does the actual string
        /// filtering of the content.  
        /// </summary>
        private readonly FilteredList list;

        /// <summary> Scroll offset for the vertical scroll area. </summary>
        private Vector2 scroll;

        /// <summary>
        /// Index of the item under the mouse or selected with the keyboard.
        /// </summary>
        private int hoverIndex;

        /// <summary>
        /// An item index to scroll to on the next draw.
        /// </summary>
        private int scrollToIndex;

        /// <summary>
        /// An offset to apply after scrolling to scrollToIndex. This can be
        /// used to control if the selection appears at the top, bottom, or
        /// center of the popup.
        /// </summary>
        private float scrollOffset;
        #endregion -- Private Variables ---------------------------------------

        #region -- GUI Styles -------------------------------------------------
        // GUIStyles implicitly cast from a string. This triggers a lookup into
        // the current skin which will be the editor skin and lets us get some
        // built-in styles.

        private static GUIStyle SearchBox = "ToolbarSeachTextField";
        private static GUIStyle CancelButton = "ToolbarSeachCancelButton";
        private static GUIStyle DisabledCancelButton = "ToolbarSeachCancelButtonEmpty";
        private static GUIStyle Selection = "SelectionRect";
        #endregion -- GUI Styles ----------------------------------------------

        #region -- Initialization ---------------------------------------------
        private SearchablePopup(string[] names, int currentIndex, Action<int> onSelectionMade)
        {
            list = new FilteredList(names);
            this.currentIndex = currentIndex;
            this.onSelectionMade = onSelectionMade;

            hoverIndex = currentIndex;
            scrollToIndex = currentIndex;
            scrollOffset = GetWindowSize().y - ROW_HEIGHT * 2;
        }
        #endregion -- Initialization ------------------------------------------

        #region -- PopupWindowContent Overrides -------------------------------
        public override void OnOpen()
        {
            base.OnOpen();
            // Force a repaint every frame to be responsive to mouse hover.
            EditorApplication.update += Repaint;
        }

        public override void OnClose()
        {
            base.OnClose();
            EditorApplication.update -= Repaint;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(base.GetWindowSize().x,
                Mathf.Min(600, list.MaxLength * ROW_HEIGHT +
                EditorStyles.toolbar.fixedHeight));
        }

        public override void OnGUI(Rect rect)
        {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }
        #endregion -- PopupWindowContent Overrides ----------------------------

        #region -- GUI --------------------------------------------------------
        private void DrawSearch(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
                EditorStyles.toolbar.Draw(rect, false, false, false, false);

            Rect searchRect = new Rect(rect);
            searchRect.xMin += 6;
            searchRect.xMax -= 6;
            searchRect.y += 2;
            searchRect.width -= CancelButton.fixedWidth;

            GUI.FocusControl(SEARCH_CONTROL_NAME);
            GUI.SetNextControlName(SEARCH_CONTROL_NAME);
            string newText = GUI.TextField(searchRect, list.Filter, SearchBox);

            if (list.UpdateFilter(newText))
            {
                hoverIndex = 0;
                scroll = Vector2.zero;
            }

            searchRect.x = searchRect.xMax;
            searchRect.width = CancelButton.fixedWidth;

            if (string.IsNullOrEmpty(list.Filter))
                GUI.Box(searchRect, GUIContent.none, DisabledCancelButton);
            else if (GUI.Button(searchRect, "x", CancelButton))
            {
                list.UpdateFilter("");
                scroll = Vector2.zero;
            }
        }

        private void DrawSelectionArea(Rect scrollRect)
        {
            Rect contentRect = new Rect(0, 0,
                scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                list.Entries.Count * ROW_HEIGHT);

            scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

            Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);

            for (int i = 0; i < list.Entries.Count; i++)
            {
                if (scrollToIndex == i &&
                    (Event.current.type == EventType.Repaint
                     || Event.current.type == EventType.Layout))
                {
                    Rect r = new Rect(rowRect);
                    r.y += scrollOffset;
                    GUI.ScrollTo(r);
                    scrollToIndex = -1;
                    scroll.x = 0;
                }

                if (rowRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseMove ||
                        Event.current.type == EventType.ScrollWheel)
                        hoverIndex = i;
                    if (Event.current.type == EventType.MouseDown)
                    {
                        onSelectionMade(list.Entries[i].Index);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                DrawRow(rowRect, i);

                rowRect.y = rowRect.yMax;
            }

            GUI.EndScrollView();
        }

        private void DrawRow(Rect rowRect, int i)
        {
            if (list.Entries[i].Index == currentIndex)
                DrawBox(rowRect, Color.cyan);
            else if (i == hoverIndex)
                DrawBox(rowRect, Color.white);

            Rect labelRect = new Rect(rowRect);
            labelRect.xMin += ROW_INDENT;

            GUI.Label(labelRect, list.Entries[i].Text);
        }

        /// <summary>
        /// Process keyboard input to navigate the choices or make a selection.
        /// </summary>
        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    hoverIndex = Mathf.Min(list.Entries.Count - 1, hoverIndex + 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    hoverIndex = Mathf.Max(0, hoverIndex - 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = -ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.Return)
                {
                    if (hoverIndex >= 0 && hoverIndex < list.Entries.Count)
                    {
                        onSelectionMade(list.Entries[hoverIndex].Index);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    EditorWindow.focusedWindow.Close();
                }
            }
        }
        #endregion -- GUI -----------------------------------------------------
    }
#endif

    #endregion

    #region Property Drawer
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AudioManagerClip))]
    public class AudioManagerClipDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("clip"), label, true);
        }
    }
#endif
    #endregion
}

[Serializable]
public struct AudioManagerClips
{
    public List<AudioManagerClip> clips;

    public AudioManagerClips(List<AudioManagerClip> clips = null)
    {
        this.clips = clips == null ? new List<AudioManagerClip>() : clips;
    }

    public void PlayRandomClip(AudioSource source = null)
    {
        if (clips.Count != 0)
            AudioManager.PlaySound(GetRandomClipID(), source);
    }
    public uint GetRandomClipID()
    {
        if (clips.Count != 0)
            return (uint)clips[UnityEngine.Random.Range(0, clips.Count)].clip;
        else
            throw new IndexOutOfRangeException("AvailableSounds is 0");
    }

    #region Property Drawer
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AudioManagerClips))]
    public class AudioManagerClipsDrawer : PropertyDrawer
    {
        bool initialized = false;
        UnityEditorInternal.ReorderableList reorderableList;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialized)
            {
                reorderableList = new UnityEditorInternal.ReorderableList(property.serializedObject, property.FindPropertyRelative("clips"), true, true, true, true);

                reorderableList.multiSelect = true;

                reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2.5f, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("clip"), GUIContent.none);
                };

                reorderableList.drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, new GUIContent(property.displayName));
                    EditorGUI.IntField(new Rect(rect.width - 1.25f, rect.y + 1.25f, EditorGUIUtility.fieldWidth, rect.height - 2.5f), reorderableList.count);
                };

                initialized = true;
            }

            reorderableList.DoLayoutList();
        }
    }
#endif
#endregion
}

#region SHARED KEYS
/*
[Serializable]
public class AudioManagerSharedKey
{
    [StringInList(typeof(AudioManager), nameof(AudioManager.GetSharedKeys))]
    public int key;

    #region Shared Keys
    public class StringInList : PropertyAttribute
    {
        public delegate string[] GetStringList();

        public StringInList(params string[] list)
        {
            List = list;
        }

        public StringInList(Type type, string methodName)
        {
            var method = type.GetMethod(methodName);

            if (method != null)
            {
                List = method.Invoke(null, null) as string[];
            }
            else
            {
                Debug.LogError($"Method '{methodName}' does not exist for '{type}'");
            }
        }

        public string[] List
        {
            get;
            private set;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(StringInList))]
    class StringInListDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stringInList = attribute as StringInList;
            var list = stringInList.List;

            if (property.propertyType == SerializedPropertyType.String)
            {
                int index = Mathf.Max(0, Array.IndexOf(list, property.stringValue));
                index = EditorGUI.Popup(position, property.displayName, index, list);

                property.stringValue = list[index];
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.Popup(position, property.displayName, property.intValue, list);
            }
            else
            {
                base.OnGUI(position, property, label);
            }
        }
    }
#endif
    #endregion
}*/
#endregion

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioManagerDatabase database;
    [Space]
    public List<SharedKeyHolder> sharedKeys = new List<SharedKeyHolder>();

    int loopRegistryAttempts;

    Coroutine lerpChildrenVolumeCoroutine;
    Coroutine lerpChildrenPitchCoroutine;

    AudioSource source;
    List<AudioManagerChildSource> childSources = new List<AudioManagerChildSource>();

    public static float globalVolumeFactor = 1f;
    public static AudioManager current;

    static AudioManager am = null;
    HideFlags keyObjectsFlag = HideFlags.HideInHierarchy;

    bool localAudioSourceIsAvailable;

    private void Awake()
    {
        current = this;

        localAudioSourceIsAvailable = false;
        Invoke(nameof(EnableLocalAudioSource), 1f);

        source = GetComponent<AudioSource>();
    }

    void EnableLocalAudioSource() => localAudioSourceIsAvailable = true;

    #region Play Sound
    public void PlaySound_Local(string clipIdentifier) => PlaySound(clipIdentifier);
    public void PlaySound_Local(int clipIndex) => PlaySound(clipIndex);
    public void PlaySound_Local(AudioClip clip) => PlaySound(clip);

    /// <summary>
    /// Plays the specified sound at the specified Audio Source. If no Audio Source is specified, the clip will be played in 2D space with the default Audio Source.
    /// </summary>
    /// <param name="clipIdentifier">The 'Identifier' value of the Audio Clip you want to play.</param>
    public static void PlaySound(string clipIdentifier, AudioSource source = null)
    {
        if(clipIdentifier == string.Empty)
        {
            ChampisConsole.LogWarning("Clip name was empty.");
            return;
        }

        AudioFile sf = Array.Find(current.database.clips, clip => clip.clipName == clipIdentifier);

        if (sf != null)
        {
            if (source == null)
            {
                if (current.localAudioSourceIsAvailable)
                {
                    current.source.pitch = sf.randomPitch ? UnityEngine.Random.Range(sf.randomPitchRange.x, sf.randomPitchRange.y) : sf.pitch;
                    current.source.PlayOneShot(sf.clip, sf.randomVolume ? UnityEngine.Random.Range(sf.randomVolumeRange.x, sf.randomVolumeRange.y) : sf.volume);
                }
            }
            else
            {
                source.pitch = sf.randomPitch ? UnityEngine.Random.Range(sf.randomPitchRange.x, sf.randomPitchRange.y) : sf.pitch;
                source.PlayOneShot(sf.clip, sf.randomVolume ? UnityEngine.Random.Range(sf.randomVolumeRange.x, sf.randomVolumeRange.y) : sf.volume);
            }

            return;
        }

        ChampisConsole.LogWarning($"Audio Manager does not contain a clip called '{clipIdentifier}'");
    }

    /// <summary>
    /// Plays the specified sound at the specified Audio Source. If no Audio Source is specified, the clip will be played in 2D space with the default Audio Source.
    /// </summary>
    /// <param name="index">The array index of the Audio Clip you want to play.</param>
    /// <param name="source">Audio source to use for playing this clip. If null, the clip will be played in 2D.</param>
    /// <returns></returns>
    public static void PlaySound(int index, AudioSource source = null)
    {
        AudioFile sf = current.database.clips[index];
        PlaySound(sf.clipName, source);
    }

    public static void PlaySound(uint id, AudioSource source = null)
    {
        AudioFile sf = Array.Find(current.database.clips, clip => clip.UniqueID == id);

        if (sf != null)
        {
            if (source == null)
            {
                if (current.localAudioSourceIsAvailable)
                {
                    current.source.pitch = sf.randomPitch ? UnityEngine.Random.Range(sf.randomPitchRange.x, sf.randomPitchRange.y) : sf.pitch;
                    current.source.PlayOneShot(sf.clip, sf.randomVolume ? UnityEngine.Random.Range(sf.randomVolumeRange.x, sf.randomVolumeRange.y) : sf.volume);
                }
            }
            else
            {
                source.pitch = sf.randomPitch ? UnityEngine.Random.Range(sf.randomPitchRange.x, sf.randomPitchRange.y) : sf.pitch;
                source.PlayOneShot(sf.clip, sf.randomVolume ? UnityEngine.Random.Range(sf.randomVolumeRange.x, sf.randomVolumeRange.y) : sf.volume);
            }

            return;
        }

        ChampisConsole.LogWarning($"Audio Manager does not contain a clip with ID '{id}'");
    }

    #region SHARED KEYS
    /*
    /// <summary>
    /// Plays the specified sound at the specified Audio Source. If no Audio Source is specified, the clip will be played in 2D space with the default Audio Source.
    /// </summary>
    /// <param name="key">The Shared Key you want to play.</param>
    /// <param name="source">Audio source to use for playing this clip. If null, the clip will be played in 2D.</param>
    /// <returns></returns>
    public static void PlaySound(AudioManagerSharedKey key, AudioSource source = null)
    {
        PlaySound(current.sharedKeys[key.key].clipName, source);
    }*/
    #endregion

    public static void PlaySound(AudioClip clip, AudioSource source = null)
    {
        if (source == null)
        {
            if (current.localAudioSourceIsAvailable)
            {
                current.source.pitch = 1f;
                current.source.PlayOneShot(clip);
            }
        }
        else
        {
            source.pitch = 1f;
            source.PlayOneShot(clip);
        }
    }
    #endregion

    #region Get Audio Clip
    /// <summary>
    /// Returns the Audio Clip requested by its Identifier.
    /// </summary>
    /// <param name="clipIdentifier">The 'Identifier' value of the clip you want to get.</param>
    /// <returns></returns>
    public static AudioClip GetClip(string clipIdentifier)
    {
        AudioFile sf = Array.Find(current.database.clips, clip => clip.clipName == clipIdentifier);

        if(sf != null)
            return sf.clip;

        ChampisConsole.LogWarning($"Audio Manager does not contain a clip called '{clipIdentifier}'");
        return null;
    }
    /// <summary>
    /// Returns the Audio Clip requested by its index.
    /// </summary>
    /// <param name="index">The array index of the Audio Clip you want to get.</param>
    /// <returns></returns>
    public static AudioClip GetClip(int index)
    {
        return current.database.clips[index].clip;
    }
    #endregion

    #region Get Audio File
    /// <summary>
    /// Returns the Audio File requested by its Identifier. Audio Files contain variables such as identifier, clip and volume.
    /// </summary>
    /// <param name="identifier">The 'Identifier' value of the Audio File you want to get.</param>
    /// <returns></returns>
    public static AudioFile GetAudioFile(string identifier)
    {
        AudioFile sf = Array.Find(current.database.clips, clip => clip.clipName == identifier);

        if (sf != null)
            return sf;

        ChampisConsole.LogWarning($"Audio Manager does not contain an Audio File called '{identifier}'");
        return null;
    }
    /// <summary>
    /// Returns the Audio File requested by its index. Audio Files contain variables such as identifier, clip and volume.
    /// </summary>
    /// <param name="index">The array index of the Audio File you want to get.</param>
    /// <returns></returns>
    public static AudioFile GetAudioFile(int index)
    {
        return current.database.clips[index];
    }
    #endregion

    #region Child Sources
    public static void RegisterChildSource(AudioManagerChildSource childSource)
    {
        if (!ChildAlreadyListed(childSource.Identifier))
            current.childSources.Add(childSource);
        else
        {
            ChampisConsole.LogWarning($"Tried to register the '{childSource.Identifier}' Audio Source twice. Changing name...");
            LoopRegistrationAttempts(childSource);
        }
    }
    public static void UnregisterChildSource(AudioManagerChildSource childSource)
    {
        if (ChildAlreadyListed(childSource.Identifier))
            current.childSources.Remove(childSource);
    }

    public static AudioSource GetChildSource(string identifier)
    {
        foreach (AudioManagerChildSource amcs in current.childSources)
            if (amcs.Identifier == identifier)
                return amcs.AudioSource;

        return null;
    }

    public static void ChildrenSetVolumeFactor(string[] excludedChildren = null)
    {
        foreach (AudioManagerChildSource amcs in current.childSources)
            if (!ChildIsExcluded(excludedChildren, amcs.Identifier))
                amcs.ApplyGlobalVolumeFactor();
    }

    public static void ChildrenSetVolume(float volume, string[] excludedChildren)
    {
        foreach (AudioManagerChildSource amcs in current.childSources)
            if (!ChildIsExcluded(excludedChildren, amcs.Identifier))
                amcs.AudioSource.volume = volume;
    }
    public static void ChildrenSetPitch(float pitch, string[] excludedChildren)
    {
        foreach (AudioManagerChildSource amcs in current.childSources)
            if (!ChildIsExcluded(excludedChildren, amcs.Identifier))
                amcs.AudioSource.pitch = pitch;
    }

    public static void ChildrenLerpVolume(float from, float to, float lerpSpeed, string[] excludedChildren)
    {
        if (!current)
            return;

        if (current.lerpChildrenVolumeCoroutine != null)
            current.StopCoroutine(current.lerpChildrenVolumeCoroutine);

        current.lerpChildrenVolumeCoroutine = current.StartCoroutine(current._childrenLerpVol(from, to, lerpSpeed, excludedChildren));
    }
    public static void ChildrenLerpPitch(float from, float to, float lerpSpeed, string[] excludedChildren)
    {
        if (current.lerpChildrenPitchCoroutine != null)
            current.StopCoroutine(current.lerpChildrenPitchCoroutine);

        current.lerpChildrenPitchCoroutine = current.StartCoroutine(current._childrenLerpPitch(from, to, lerpSpeed, excludedChildren));
    }

    IEnumerator _childrenLerpVol(float from, float vol, float speed, string[] excludedChildren)
    {
        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * speed;

            foreach (AudioManagerChildSource amcs in current.childSources)
                if (excludedChildren == null || !ChildIsExcluded(excludedChildren, amcs.Identifier))
                    amcs.AudioSource.volume = Mathf.Lerp(from == -1f ? amcs.AudioSource.volume : from, vol == -1 ? amcs.DefaultVolume : vol, t);

            yield return null;
        }
    }
    IEnumerator _childrenLerpPitch(float from, float pitch, float speed, string[] excludedChildren)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            foreach (AudioManagerChildSource amcs in current.childSources)
                if (excludedChildren == null || !ChildIsExcluded(excludedChildren, amcs.Identifier))
                    amcs.AudioSource.pitch = Mathf.Lerp(from == -10f ? amcs.AudioSource.pitch : from, pitch, t);

            yield return null;
        }
    }

    static void LoopRegistrationAttempts(AudioManagerChildSource childSource)
    {
        current.loopRegistryAttempts++;
        string attemptedName = childSource.Identifier + $" {current.loopRegistryAttempts}";

        foreach (AudioManagerChildSource s in current.childSources)
        {
            if (attemptedName == s.Identifier)
            {
                LoopRegistrationAttempts(childSource);
                return;
            }
        }

        current.loopRegistryAttempts = 0;

        childSource.SetIdentifier(attemptedName);
        current.childSources.Add(childSource);
    }

    static bool ChildIsExcluded(string[] excludedChildren, string childToCompare)
    {
        if (excludedChildren == null)
            return false;

        foreach (string s in excludedChildren)
            if (childToCompare == s)
                return true;

        return false;
    }
    static bool ChildAlreadyListed(string childToCompare)
    {
        if (!current)
            return false;

        foreach (AudioManagerChildSource s in current.childSources)
            if (childToCompare == s.Identifier)
                return true;

        return false;
    }
    #endregion

    #region Weird Editor Things
    public void NewSharedKey(string keyName, string connection)
    {
        foreach(SharedKeyHolder sk in sharedKeys)
        {
            if(sk.gameObject.name == keyName)
            {
                Debug.LogWarning("A Shared Key with that name already exists");
                return;
            }
        }

        SharedKeyHolder g = new GameObject().AddComponent<SharedKeyHolder>();

        g.gameObject.name = keyName;
        g.gameObject.hideFlags = keyObjectsFlag;
        g.transform.SetParent(gameObject.transform);
        g.clipName = connection;

        sharedKeys.Add(g);
    }
    public void EditSharedKey(string keyName, string newConnection)
    {
        SharedKeyHolder skh = null;

        foreach (SharedKeyHolder sk in sharedKeys)
        {
            if (sk.gameObject.name == keyName)
            {
                skh = sk;
            }
        }

        if (skh == null)
        {
            Debug.LogWarning("That Shared Key does not exist");
            return;
        }

        skh.clipName = newConnection;
    }
    public void DeleteSharedKey(string keyName)
    {
        SharedKeyHolder skh = null;

        foreach (SharedKeyHolder sk in sharedKeys)
        {
            if (sk.gameObject.name == keyName)
            {
                skh = sk;
            }
        }

        if (skh == null)
        {
            Debug.Log("That Shared Key does not exist");
            return;
        }

        DestroyImmediate(skh.gameObject);
        sharedKeys.Remove(skh);

        Debug.Log($"Shared Key '{keyName}' successfully deleted");
    }
    public void DeleteAll(bool result)
    {
        if (result == true)
        {
            foreach(SharedKeyHolder skh in sharedKeys)
            {
                DestroyImmediate(skh.gameObject);
            }

            sharedKeys.Clear();
        }
    }

    public static string[] GetSharedKeys()
    {
        if (am == null)
            am = FindObjectOfType<AudioManager>();

        var temp = new List<string>();

        for(int i = 0; i < am.sharedKeys.Count; i++)
        {
            temp.Add(am.sharedKeys[i].gameObject.name);
        }

        return temp.ToArray();
    }
    public static string[] GetClipsToList()
    {
        AudioManager am = FindObjectOfType<AudioManager>();

        var temp = new List<string>();

        for (int i = 0; i < am.database.clips.Length; i++)
        {
            temp.Add(am.database.clips[i].clipName);
        }

        return temp.ToArray();
    }

    public static string[] GetSoundsInDatabase()
    {
        if (am == null)
            am = FindObjectOfType<AudioManager>();

        List<string> collection = new List<string>();

        foreach (AudioFile af in am.database.clips)
            collection.Add(af.clipName);

        return collection.ToArray();
    }

    public string GetSharedKeysNamesForDisplay()
    {
        string result = string.Empty;

        if (sharedKeys.Count == 0)
            return "There are no Shared Keys";

        for (int i = 0; i < sharedKeys.Count; i++)
        {
            string progress = $"- {sharedKeys[i].gameObject.name}";

            if(result == string.Empty)
                result = $"{progress}";
            else
                result = $"{result}\n{progress}";
        }

        return result;
    }
    public string GetSharedKeysConnectionsForDisplay()
    {
        string result = string.Empty;

        if (sharedKeys.Count == 0)
            return "There are no Shared Keys";

        for (int i = 0; i < sharedKeys.Count; i++)
        {
            string progress = $"- {sharedKeys[i].clipName}";

            if (result == string.Empty)
                result = $"{progress}";
            else
                result = $"{result}\n{progress}";
        }

        return result;
    }

#if UNITY_EDITOR
    [ContextMenu("Hide Key Objects")]
    void HideKeyObjects()
    {
        keyObjectsFlag = HideFlags.HideInHierarchy;

        foreach(SharedKeyHolder skh in sharedKeys)
        {
            skh.gameObject.hideFlags = keyObjectsFlag;
        }

        EditorApplication.RepaintHierarchyWindow();
    }
    [ContextMenu("Show Key Objects")]
    void ShowKeyObjects()
    {
        keyObjectsFlag = HideFlags.NotEditable;

        foreach (SharedKeyHolder skh in sharedKeys)
        {
            skh.gameObject.hideFlags = keyObjectsFlag;
        }

        EditorApplication.RepaintHierarchyWindow();
    }

    [MenuItem("Champis Toolbox/Audio Manager/All Audio Sources To Children")]
    public static void AllAudioSourcesToChildren()
    {
        int conversionCount = 0;

        AudioSource[] sources = FindObjectsOfType<AudioSource>(true);
        List<AudioManagerChildSource> editorChildSources = new List<AudioManagerChildSource>();

        foreach (AudioSource s in sources)
        {
            if (!s.gameObject.GetComponent<AudioManagerChildSource>())
            {
                AudioManagerChildSource tmp = s.gameObject.AddComponent<AudioManagerChildSource>();

                tmp.SetIdentifier(GetUniqueChildIdentifier(s.gameObject.name + (s.gameObject.name.EndsWith("Audio Source") ? string.Empty : " Audio Source"), editorChildSources));
                tmp.DefaultVolume = s.gameObject.GetComponent<AudioSource>().volume;

                editorChildSources.Add(tmp);

                conversionCount++;
            }
            else
                editorChildSources.Add(s.gameObject.GetComponent<AudioManagerChildSource>());
        }

        Debug.Log($"Converted {conversionCount} out of {sources.Length} Audio Sources to Child Sources");
    }
    static string GetUniqueChildIdentifier(string initialName, List<AudioManagerChildSource> sources)
    {
        int attempts = 0;

        bool invalidName = true;
        bool allowContinue = false;

        string attemptedName = string.Empty;

        while (invalidName)
        {
            attempts++;
            attemptedName = initialName + $" {attempts}";

            foreach (AudioManagerChildSource s in sources)
            {
                if (allowContinue)
                {
                    if (attemptedName == s.Identifier)
                    {
                        invalidName = true;
                        allowContinue = false;
                    }
                }
            }

            if (allowContinue)
                invalidName = false;

            allowContinue = true;
        }

        return attemptedName;
    }

    [MenuItem("Champis Toolbox/Audio Manager/All Children to Audio Sources")]
    public static void AllChildrenToAudioSources()
    {
        AudioManagerChildSource[] children = FindObjectsOfType<AudioManagerChildSource>(true);

        foreach (AudioManagerChildSource c in children)
            DestroyImmediate(c);
    }

    [MenuItem("Champis Toolbox/Audio Manager/Update Unique IDs")]
    public static void UpdateUniqueIDs()
    {
        bool result = EditorUtility.DisplayDialog("Warning", "This should be used ONLY ONCE PER PROJECT, if used more than once everything could break.\n\nIf unsure, DON'T USE IT", "Get IDs", "Cancel");

        if (result)
        {
            if (am == null)
                am = FindObjectOfType<AudioManager>();

            for (uint i = 0; i < am.database.clips.Length; i++)
                am.database.clips[i].UniqueID = i;
        }
    }
    public static int GetIndexForID(uint id)
    {
        if (am == null)
            am = FindObjectOfType<AudioManager>();

        for (int i = 0; i < am.database.clips.Length; i++)
            if (am.database.clips[i].UniqueID == id)
                return i;

        ChampisConsole.LogError($"ID '{id}' does not exist. Returning -1...");
        return -1;
    }
    public static uint GetIDForIndex(int index)
    {
        if (am == null)
            am = FindObjectOfType<AudioManager>();

        return am.database.clips[index].UniqueID;
    }
    public static bool IDExists(uint id)
    {
        if (am == null)
            am = FindObjectOfType<AudioManager>();

        foreach(AudioFile af in am.database.clips)
            if (af.UniqueID == id)
                return true;

        return false;
    }
    public static string GetIDName(uint id)
    {
        if (am == null)
            am = FindObjectOfType<AudioManager>();

        foreach (AudioFile af in am.database.clips)
            if (af.UniqueID == id)
                return af.clipName;

        return string.Empty;
    }

    #region CUSTOM EDITOR
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : Editor
    {
        AudioManager manager;
        string keyName = string.Empty;
        string keyClip = string.Empty;

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            manager = target as AudioManager;
            //manager.SetEditorInstance();

            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AudioManager.database.clips)), true);

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("database"));

            #region SHARED KEYS
            /*
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Shared Keys are like a variable, you just need to set their content once and every piece of code referencing it will get its value.", MessageType.Info);
            keyName = EditorGUILayout.TextField("Shared Key Name", keyName);

            keyClip = EditorGUILayout.TextField("Clip Name to Connect", keyClip);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Shared Key"))
            {
                if(keyName == string.Empty)
                {
                    Debug.LogError("You must provide a Shared Key name");
                    return;
                }
                if (keyClip == string.Empty)
                {
                    Debug.LogError("You must provide a Clip Name to connect with.");
                    return;
                }

                manager.NewSharedKey(keyName, keyClip);
                keyName = string.Empty;
                keyClip = string.Empty;
            }

            if (GUILayout.Button("Edit Shared Key"))
            {
                if (keyName == string.Empty)
                {
                    Debug.LogError("You must provide a Shared Key name");
                    return;
                }
                if (keyClip == string.Empty)
                {
                    Debug.LogError("You must provide a Clip Name to connect with.");
                    return;
                }

                manager.EditSharedKey(keyName, keyClip);
                keyName = string.Empty;
                keyClip = string.Empty;
            }

            if (GUILayout.Button("Delete Shared Key"))
            {
                if (keyName == string.Empty)
                {
                    Debug.LogError("You must provide a Shared Key name");
                    return;
                }

                manager.DeleteSharedKey(keyName);
                keyName = string.Empty;
                keyClip = string.Empty;
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Delete All Shared Keys"))
            {
                manager.DeleteAll(EditorUtility.DisplayDialog("Are you sure?", "All Shared Keys will be deleted. This action can't be undone!", "Delete All", "Cancel"));

                keyName = string.Empty;
                keyClip = string.Empty;
            }

            EditorGUILayout.Space();

            //Headers
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key:");
            EditorGUILayout.LabelField("Connection:");
            GUILayout.EndHorizontal();


            //KeysList
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(manager.GetSharedKeysNamesForDisplay(), MessageType.None);
            EditorGUILayout.HelpBox(manager.GetSharedKeysConnectionsForDisplay(), MessageType.None);
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            */
            #endregion
        }
    }
    #endregion
#endif
    #endregion
}
