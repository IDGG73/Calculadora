using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueDecisionButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Image[] images;
    [SerializeField] TextMeshProUGUI[] labels;

    [Foldout("Optional")]
    [SerializeField] GameObject submitButtonPrompt;

    DialogueSystem dialogueSystem;
    DialogueDecision desition;

    public void SubmitDesition()
    {
        if (desition.functionToInvoke != string.Empty)
            dialogueSystem.gameObject.SendMessage(desition.functionToInvoke);
        else
            ChampisConsole.LogWarning("Decision '" + desition.decisionName + "' in dialogue " + dialogueSystem.GetCurrentDialogueIndex() + " from conversation file '" + dialogueSystem.GetCurrentConversationFile().name + "' has an empty function.");

        switch (desition.submitAction)
        {
            case DecisionSubmitAction.NextDialogue:
                dialogueSystem.ForceNextDialogue();
                break;
            case DecisionSubmitAction.EndConversation:
                dialogueSystem.EndConversation();
                break;
            case DecisionSubmitAction.JumpToDialogue:
                dialogueSystem.GoToDialogue(desition.jumpToDialogue);
                break;
        }
    }

    public void SetDialogueSystem(DialogueSystem newDialogueSystem)
    {
        dialogueSystem = newDialogueSystem;
    }

    public void SetStyle(Color boxColor, Color textColor)
    {
        foreach (Image image in images)
            image.color = boxColor;

        foreach (TextMeshProUGUI label in labels)
            label.color = textColor;
    }

    public void SetDecision(DialogueDecision newDesition)
    {
        desition = newDesition;

        foreach (TextMeshProUGUI label in labels)
            label.text = newDesition.decisionName;

        gameObject.name = desition.decisionName + " - BTN";
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (submitButtonPrompt)
            submitButtonPrompt.SetActive(true);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (submitButtonPrompt)
            submitButtonPrompt.SetActive(false);
    }
}
