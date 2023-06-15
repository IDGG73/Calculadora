using UnityEngine;
using TMPro;

public class LocalizedItemDescription : MonoBehaviour
{
    [SerializeField] LocalizedString content;
    [SerializeField] TextMeshProUGUI descriptionDisplay;

    public void ShowDescription()
    {
        switch (SettingsManager.currentLanguage)
        {
            case Language.English:
                if (descriptionDisplay)
                    descriptionDisplay.text = content.englishText;
                break;
            case Language.Spanish:
                if (descriptionDisplay)
                    descriptionDisplay.text = content.spanishText;
                break;
        }
    }
    public void HideDescription()
    {
        if (descriptionDisplay)
            descriptionDisplay.text = string.Empty;
    }
}
