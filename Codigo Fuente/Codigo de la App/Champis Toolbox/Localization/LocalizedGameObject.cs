using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedGameObject : MonoBehaviour, ILanguageChange
{
    [SerializeField] GameObject english;
    [SerializeField] GameObject spanish;

    private void Start() => OnLanguageChange();

    public void OnLanguageChange()
    {
        english?.SetActive(false);
        spanish?.SetActive(false);

        switch (SettingsManager.currentLanguage)
        {
            case Language.Spanish:
                spanish?.SetActive(true);
                break;

            default:
                english?.SetActive(true);
                break;
        }
    }
}
