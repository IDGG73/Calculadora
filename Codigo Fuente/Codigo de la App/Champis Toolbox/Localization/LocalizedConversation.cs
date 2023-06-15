using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedConversation : MonoBehaviour, ILanguageChange
{
    [SerializeField] DialogueSystem dialogueSystem;
    [Space]
    [SerializeField] ConversationFile[] spanishFiles;
    [SerializeField] ConversationFile[] englishFiles;

    int currentIndex;

    private void Start() => SetConversationFile(0);

    public void OnLanguageChange() => SetConversationFile(currentIndex);

    public void SetConversationFile(int index)
    {
        switch (SettingsManager.currentLanguage)
        {
            case Language.Spanish:
                dialogueSystem.ChangeConversationFile(spanishFiles[index]);
                break;

            default:
                dialogueSystem.ChangeConversationFile(englishFiles[index]);
                break;
        }

        currentIndex = index;
    }
}
