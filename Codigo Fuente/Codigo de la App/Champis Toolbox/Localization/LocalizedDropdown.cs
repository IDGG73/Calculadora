using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class LocalizedDropdown : MonoBehaviour, ILanguageChange
{
    [SerializeField] string[] englishValues;
    [SerializeField] string[] spanishValues;

    TMP_Dropdown dropdown;
    List<TMP_Dropdown.OptionData> currentDropdownData = new List<TMP_Dropdown.OptionData>();

    public void OnLanguageChange()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();

        for(int i = 0; i < dropdown.options.Count; i++)
        {
            switch (SettingsManager.currentLanguage)
            {
                case Language.English: dropdown.options[i].text = englishValues[i]; break;
                case Language.Spanish: dropdown.options[i].text = spanishValues[i]; break;
            }
        }

        dropdown.captionText.text = dropdown.options[dropdown.value].text;
    }
}
