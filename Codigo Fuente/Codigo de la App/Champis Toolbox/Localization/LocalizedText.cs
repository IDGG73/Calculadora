using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour, ILanguageChange
{
    [SerializeField, TextArea(0, 20)] string english;
    [SerializeField, TextArea(0, 20)] string spanish;

    Text _text;
    TextMeshProUGUI _proText;
    TextMeshPro _proMesh;

    private void Start()
    {
        GetTextComponent();
        OnLanguageChange();
    }

    public void OnLanguageChange()
    {
        GetTextComponent();

        switch (SettingsManager.currentLanguage)
        {
            case Language.English:
                if (_text != null)
                    _text.text = english;
                else
                    _proText.text = english;
                break;

            case Language.Spanish:
                if (_text != null)
                    _text.text = spanish;
                else
                    _proText.text = spanish;
                break;
        }
    }

    public string SetEnglishContent(string newCont) => english = newCont;
    public string SetSpanishContent(string newCont) => spanish = newCont;

    public string GetEnglishContent() => english;
    public string GetSpanishContent() => spanish;

    void GetTextComponent()
    {
        if (_text == null)
            _text = GetComponent<Text>();

        if (_proText == null)
            _proText = GetComponent<TextMeshProUGUI>();
    }
}
