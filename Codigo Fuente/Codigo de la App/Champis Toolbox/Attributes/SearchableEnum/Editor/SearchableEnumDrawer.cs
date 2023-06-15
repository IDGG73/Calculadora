// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/01/2018
// ----------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

namespace RoboRyanTron.SearchableEnum.Editor
{
    /// <summary>
    /// Draws the custom enum selector popup for enum fileds using the
    /// SearchableEnumAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumDrawer : PropertyDrawer
    {
        /// <summary>
        /// Cache of the hash to use to resolve the ID for the drawer.
        /// </summary>
        private int idHash;

        bool isEnum;
        string[] options;

        SearchableEnumAttribute searchableEnumAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            searchableEnumAttribute = attribute as SearchableEnumAttribute;
            isEnum = property.type == "Enum";

            if (!isEnum)
                options = searchableEnumAttribute.options;
            else
                options = property.enumDisplayNames;
            
            if (idHash == 0)
                idHash = "SearchableEnumDrawer".GetHashCode();

            int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);
            
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText;

            if ((isEnum ? property.enumValueIndex : property.intValue) < 0 || (isEnum ? property.enumValueIndex : property.intValue) >= options.Length)
                buttonText = new GUIContent();
            else
                buttonText = new GUIContent(options[(isEnum ? property.enumValueIndex : property.intValue)]);

            if (DropdownButton(id, position, buttonText))
            {
                Action<int> onSelect = i =>
                {
                    if (isEnum)
                        property.enumValueIndex = i;
                    else
                        property.intValue = i;

                    property.serializedObject.ApplyModifiedProperties();
                };
                
                SearchablePopup.Show(position, options,
                    (isEnum ? property.enumValueIndex : property.intValue), onSelect);
            }

            EditorGUI.EndProperty();
        }
        
        /// <summary>
        /// A custom button drawer that allows for a controlID so that we can
        /// sync the button ID and the label ID to allow for keyboard
        /// navigation like the built-in enum drawers.
        /// </summary>
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
                    if (GUIUtility.keyboardControl == id && current.character =='\n')
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
}
