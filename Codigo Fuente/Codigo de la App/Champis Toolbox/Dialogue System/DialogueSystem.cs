using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;

#region Other Classes
public enum DialogueAlignment
{
    up, center, down
}

public enum DecisionSubmitAction
{
    None, NextDialogue, EndConversation, JumpToDialogue
}

public enum DialogueSkipAction
{
    NextDialogue, EndConversation
}

public interface IConversationStart
{
    public void OnConversationStart();
}
public interface IConversationEnd
{
    public void OnConversationEnd();
}

[System.Serializable]
public class Dialogue
{
    public TextAnchor dialogueAlignment = TextAnchor.LowerLeft;
    [Space]
    public DialogueCharacter character;
    public bool usePortrait;
    [Space]
    [TextArea(5, 100)]
    public string dialogueContent;
    public string methodToInvokeOnDialogueSkip;
    public DialogueSkipAction skipAction = DialogueSkipAction.NextDialogue;

    [Header("DECISIONS")]
    public bool useDecisions;
    [Space]
    public DialogueDecision[] dialogueDecisions;
}

[System.Serializable]
public class DialogueDecision
{
    [Tooltip("This name will be displayed in the UI.")]
    public string decisionName;
    [Tooltip("If the player selects this decision, a function with this name will be invoked in the GameObject containing the Dialogue System.")]
    public string functionToInvoke;
    [Space]
    [Tooltip("Select the action to execute if the user selects this decision.\n\nNone: No action will be executed.\n\nNext Dialogue: Jump to the next dialogue. If there is no next dialogue, the conversation will end.\n\nEnd Conversation: Ends the conversation directly.\n\nJump To Dialogue: Takes the user to the specified dialogue.")]
    public DecisionSubmitAction submitAction;
    [Tooltip("The dialogue to jump at if 'Submit Action' is set to 'Jump To Dialogue'.")]
    public int jumpToDialogue;
}

[System.Serializable]
public class ConversationData
{
    public bool isTalking;
    public DialogueSystem currentDialogueSystem;
    public int currentDialogueIndex;
}
#endregion

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] ConversationFile conversationFile;
    [Space]
    public GameObject trigger;
    [Tooltip("When starting a dialogue, this animator's update mode will be changed to 'Unscaled Time'. When ending the conversation, the update mode will return to its default.\n\nNOTE: The default update mode is taken in 'Start()'")]
    public Animator talkerAnimator;

    [Header("EVENTS")]
    [SerializeField] bool useEvents = true;
    [SerializeField] UnityEvent onConversationStart;
    [SerializeField] UnityEvent onConversationEnd;

    bool canSkipDialogue;
    float updateLimiter;

    AnimatorUpdateMode defaultUpdateMode;

    private void Start()
    {
        if (talkerAnimator != null)
            defaultUpdateMode = talkerAnimator.updateMode;
    }

    private void Update()
    {
        if (!DialogueManager.data.isTalking || DialogueManager.data.currentDialogueSystem != this)
        {
            updateLimiter = 0f;
            return;
        }

        updateLimiter += Time.unscaledDeltaTime;

        if (updateLimiter < 0.3f)
            return;

        updateLimiter = 0.3f;

        if (SettingsManager.GetInputAction(DialogueManager.current.nextDialogueInputAction).WasPressedThisFrame())
        {
            if (conversationFile.dialogues[GetCurrentDialogueIndex()].useDecisions)
                SkipWritingEffect();
            else
                SkipDialogue();
        }
    }

    public void StartConversation()
    {
        if (DialogueManager.data.isTalking)
        {
            ChampisConsole.LogWarning("Can't start this conversation as there is another conversation in course.");
            return;
        }

        DialogueManager.data.isTalking = true;
        DialogueManager.data.currentDialogueIndex = 0;
        DialogueManager.data.currentDialogueSystem = this;

        RefreshDialogue(0);

        if (DialogueManager.current.useDialogueAnimations)
            DoAnimation("Appear", true);
        else
            DialogueManager.current.rootLayoutHolder.gameObject.SetActive(true);

        if (useEvents)
            onConversationStart.Invoke();

        StartCoroutine(CanSkipDialogue(true));

        if (talkerAnimator != null)
            talkerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        CallEveryConversationStart();

        if (DialogueManager.current.freezeTime)
            DialogueManager.current.SetTimeScale(0f);
    }

    public void EndConversation()
    {
        if (useEvents)
            onConversationEnd.Invoke();

        ClearDecisionButtons();
        EventSystem.current.SetSelectedGameObject(null);

        DialogueManager.data.isTalking = false;
        DialogueManager.data.currentDialogueSystem = null;
        CallEveryConversationEnd();

        StartCoroutine(CanSkipDialogue(false, 0.1f));

        DialogueManager.current.DialogueEndSFX();

        if (trigger != null)
            trigger.gameObject.SetActive(true);

        if (talkerAnimator != null)
            talkerAnimator.updateMode = defaultUpdateMode;

        if (DialogueManager.current.useDialogueAnimations)
            DoAnimation("Disappear", true);
        else
            DialogueManager.current.rootLayoutHolder.gameObject.SetActive(false);

        if (DialogueManager.current.freezeTime)
            DialogueManager.current.ResetTimeScale();
    }

    public void SkipDialogue()
    {
        if (DialogueManager.writeAnimationCoroutine != null)
        {
            SkipWritingEffect();
        }
        else
        {
            if (!canSkipDialogue)
                return;

            canSkipDialogue = false;
            StartCoroutine(CanSkipDialogue(true));

            if (GetCurrentDialogueIndex() != conversationFile.dialogues.Length - 1)
            {
                if (conversationFile.dialogues[GetCurrentDialogueIndex()].skipAction == DialogueSkipAction.EndConversation)
                {
                    EndConversation();
                    return;
                }

                DialogueManager.current.DialogueNextSFX();

                if (conversationFile.dialogues[GetCurrentDialogueIndex()].methodToInvokeOnDialogueSkip != string.Empty)
                    gameObject.SendMessage(conversationFile.dialogues[GetCurrentDialogueIndex()].methodToInvokeOnDialogueSkip);

                DialogueManager.data.currentDialogueIndex++;
                DialogueManager.current.dialogueCanvas.gameObject.SetActive(true);
                RefreshDialogue(GetCurrentDialogueIndex());

                DoAnimation("Next Dialogue", true);
            }
            else
            {
                EndConversation();
            }
        }
    }

    public void ForceNextDialogue()
    {
        if (!canSkipDialogue)
            return;

        canSkipDialogue = false;
        StartCoroutine(CanSkipDialogue(true));

        SkipWritingEffect();

        if (GetCurrentDialogueIndex() != conversationFile.dialogues.Length - 1)
        {
            DialogueManager.data.currentDialogueIndex++;

            DialogueManager.current.dialogueCanvas.gameObject.SetActive(true);
            RefreshDialogue(GetCurrentDialogueIndex());

            DoAnimation("Next Dialogue", true);
        }
        else
        {
            EndConversation();
        }

        if (conversationFile.dialogues[GetCurrentDialogueIndex()].methodToInvokeOnDialogueSkip != string.Empty)
            gameObject.SendMessage(conversationFile.dialogues[GetCurrentDialogueIndex()].methodToInvokeOnDialogueSkip);
    }

    public void GoToDialogue(int jumpTo)
    {
        if(jumpTo < 0)
        {
            ChampisConsole.LogError("You can't jump to negative dialogues.");
            return;
        }

        if (!canSkipDialogue)
            return;

        canSkipDialogue = false;
        StartCoroutine(CanSkipDialogue(true));

        SkipWritingEffect();

        if (conversationFile.dialogues.Length > jumpTo)
        {
            DialogueManager.data.currentDialogueIndex = jumpTo;

            DialogueManager.current.dialogueCanvas.gameObject.SetActive(true);
            RefreshDialogue(GetCurrentDialogueIndex());

            DoAnimation("Next Dialogue", true);
        }
        else
        {
            ChampisConsole.LogError($"Dialogue {jumpTo} is outside of the dialogues range. The conversation file '{conversationFile.name}' has a maximum of {conversationFile.dialogues.Length} dialogues.");
            EndConversation();
        }
    }

    public void SkipWritingEffect()
    {
        if(DialogueManager.writeAnimationCoroutine != null)
        {
            StopCoroutine(DialogueManager.writeAnimationCoroutine);
            DialogueManager.writeAnimationCoroutine = null;
        }

        DialogueManager.current.dialogueDisplay.maxVisibleCharacters = DialogueManager.current.dialogueDisplay.text.Length;
    }

    public void RefreshDialogue(int dialogueIndex)
    {
        DialogueManager.current.rootLayoutHolder.childAlignment = conversationFile.dialogues[dialogueIndex].dialogueAlignment;

        if (DialogueManager.current.decisionDisplayList != null)
            DialogueManager.current.decisionDisplayList.childAlignment = conversationFile.dialogues[dialogueIndex].dialogueAlignment;

        int errorIndex = dialogueIndex + 1;
        string dialogueToShow = conversationFile.dialogues[dialogueIndex].dialogueContent;

        if(conversationFile.dialogues[dialogueIndex].character == null)
        {
            ChampisConsole.LogError($"Dialogue '{dialogueIndex}' in Conversation File '{conversationFile.name}' does not have a character. The dialogue cannot be displayed.");
            return;
        }

        if (conversationFile.dialogues[dialogueIndex].usePortrait)
        {
            if (DialogueManager.current.portraitHolder != null)
                DialogueManager.current.portraitHolder.SetActive(true);

            if (DialogueManager.current.portraitDisplay == null)
                ChampisConsole.LogWarning("Attempted to set a portrait image when 'Portrait Display' is null.\nReference an Image component in 'Portrait Display' or disable 'Use Portrait' in dialogue " + errorIndex + " of the conversation file '" + conversationFile.name + "'.");
            else
                DialogueManager.current.portraitDisplay.sprite = conversationFile.dialogues[dialogueIndex].character.portrait;
        }
        else
        {
            if (DialogueManager.current.portraitHolder != null)
                DialogueManager.current.portraitHolder.SetActive(false);
        }

        if (DialogueManager.current.speakerNameDisplay != null)
        {
            if (DialogueManager.current.twoPointsAfterName)
                DialogueManager.current.speakerNameDisplay.text = conversationFile.dialogues[dialogueIndex].character.speakerName + ":";
            else
                DialogueManager.current.speakerNameDisplay.text = conversationFile.dialogues[dialogueIndex].character.speakerName;
        }

        #region STYLE
        //SET TEXT COLORS
        foreach (TextColoring tc in DialogueManager.current.colorTags)
            if (dialogueToShow.Contains(tc.key))
                dialogueToShow = dialogueToShow.Replace(tc.key, "#" + ColorUtility.ToHtmlStringRGB(tc.color));

        foreach (TextColoring tc in DialogueManager.current.coloredWords)
            if (dialogueToShow.Contains(tc.key))
                dialogueToShow = dialogueToShow.Replace(tc.key, $"<color=#{ColorUtility.ToHtmlStringRGB(tc.color)}>{tc.key}</color>");

        //Set Tokens
        foreach (DialogueManagerDynamicToken st in DialogueManager.current.tokens)
            if (dialogueToShow.Contains("{" + st.key + "}"))
                dialogueToShow = dialogueToShow.Replace("{" + st.key + "}", st.value);

        //SPEAKER NAME STYLE
        if (DialogueManager.current.speakerNameDisplay != null)
            DialogueManager.current.speakerNameDisplay.color = conversationFile.dialogues[dialogueIndex].character.nameColor;

        //DIALOGUE TEXT STYLE
        DialogueManager.current.dialogueDisplay.text = dialogueToShow;
        DialogueManager.current.dialogueDisplay.color = conversationFile.dialogues[dialogueIndex].character.dialogueColor;

        //DIALOGUE BOX STYLE
        DialogueManager.current.dialogueBox.color = conversationFile.dialogues[dialogueIndex].character.boxColor;

        if (DialogueManager.current.portraitBackground != null)
        {
            //PORTRAIT BACKGROUND STYLE
            if (conversationFile.dialogues[dialogueIndex].character.friendly)
                DialogueManager.current.portraitBackground.sprite = DialogueManager.current.friendlyBackground;
            else
                DialogueManager.current.portraitBackground.sprite = DialogueManager.current.foeBackground;

            DialogueManager.current.portraitBackground.color = conversationFile.dialogues[dialogueIndex].character.portraitBackgroundColor;
        }
        #endregion

        if (DialogueManager.current.useTypewriterAnimation)
        {
            DialogueManager.current.dialogueDisplay.maxVisibleCharacters = 0;

            if (DialogueManager.writeAnimationCoroutine != null)
                StopCoroutine(DialogueManager.writeAnimationCoroutine);

            DialogueManager.writeAnimationCoroutine = StartCoroutine(DialogueManager.current.TypewriterEffect(DialogueManager.current.dialogueDisplay, conversationFile.dialogues[dialogueIndex].character.timeBetweenCharacters, conversationFile.dialogues[dialogueIndex].character.useVoiceClips));
        }

        if (conversationFile.dialogues[dialogueIndex].useDecisions)
        {
            if (conversationFile.dialogues[dialogueIndex].dialogueDecisions.Length > 0)
            {
                ClearDecisionButtons();
                PopulateDecisionButtons(dialogueIndex);

                if (DialogueManager.current.decisionDisplayList != null)
                    DialogueManager.current.decisionDisplayList.gameObject.SetActive(true);

                if (DialogueManager.current.pressToSkipPrompt != null)
                    DialogueManager.current.pressToSkipPrompt.SetActive(false);

                if (DialogueManager.decisionButtons.Count > 0)
                    EventSystem.current.SetSelectedGameObject(DialogueManager.decisionButtons[0].gameObject);
            }
            else
                ChampisConsole.LogWarning("'Display Decisions' is active in dialogue " + dialogueIndex + " of the conversation file '" + conversationFile.name +"', but no decisions where given.");
        }
        else
        {
            if (DialogueManager.current.decisionDisplayList != null)
                DialogueManager.current.decisionDisplayList.gameObject.SetActive(false);

            if (DialogueManager.current.pressToSkipPrompt != null)
                DialogueManager.current.pressToSkipPrompt.SetActive(true);

            ClearDecisionButtons();
        }
    }

    void ClearDecisionButtons()
    {
        foreach (DialogueDecisionButton b in DialogueManager.decisionButtons)
        {
            Destroy(b.gameObject);
        }

        DialogueManager.decisionButtons.Clear();
    }
    void PopulateDecisionButtons(int dialogueIndex)
    {
        if (conversationFile.dialogues[dialogueIndex].dialogueDecisions.Length > 0 && DialogueManager.current.decisionDisplayList == null)
        {
            ChampisConsole.LogError("[Dialogue System] Tried to generate 'Decision Buttons' with no 'Decision Display List' assigned");
            return;
        }

        if (conversationFile.dialogues[dialogueIndex].dialogueDecisions.Length > 0 && DialogueManager.current.decisionButtonTemplate == null)
        {
            ChampisConsole.LogError("[Dialogue System] Tried to generate 'Decision Buttons' with no 'Decision Buttons Template' assigned");
            return;
        }

        if (DialogueManager.current.decisionDisplayList == null || DialogueManager.current.decisionButtonTemplate == null)
            return;

        foreach (DialogueDecision d in conversationFile.dialogues[dialogueIndex].dialogueDecisions)
        {
            DialogueDecisionButton b = Instantiate(DialogueManager.current.decisionButtonTemplate, DialogueManager.current.decisionDisplayList.transform);
            DialogueManager.decisionButtons.Add(b);

            b.SetDialogueSystem(this);
            b.SetDecision(d);
            b.SetStyle(conversationFile.dialogues[dialogueIndex].character.boxColor, conversationFile.dialogues[dialogueIndex].character.dialogueColor);
        }
    }

    public void ChangeConversationFile(ConversationFile newFile)
    {
        conversationFile = newFile;
    }

    #region Data Getters
    public bool GetTalkingState()
    {
        return DialogueManager.data.isTalking;
    }
    public int GetCurrentDialogueIndex()
    {
        return DialogueManager.data.currentDialogueIndex;
    }
    public ConversationFile GetCurrentConversationFile()
    {
        return conversationFile;
    }
    public bool GetUseEvents()
    {
        return useEvents;
    }
    public void SetUseEvents(bool newUseEventsState)
    {
        useEvents = newUseEventsState;
    }
    #endregion

    #region C# Interfaces
    void CallEveryConversationStart() => StartCoroutine(_callConvStart());
    void CallEveryConversationEnd() => StartCoroutine(_callConvEnd());

    IEnumerator _callConvStart()
    {
        yield return new WaitForEndOfFrame();

        var conversationers = UniversalFunctions.FindInterface<IConversationStart>(false);

        foreach (IConversationStart s in conversationers)
            s.OnConversationStart();
    }
    IEnumerator _callConvEnd()
    {
        yield return new WaitForEndOfFrame();

        var conversationers = UniversalFunctions.FindInterface<IConversationEnd>(false);

        foreach (IConversationEnd s in conversationers)
            s.OnConversationEnd();
    }
    #endregion

    IEnumerator CanSkipDialogue(bool canSkip, float delay = 0.1f)
    {
        yield return new WaitForSecondsRealtime(delay);

        canSkipDialogue = canSkip;
    }

    void DoAnimation(string stateName, bool isTrigger)
    {
        if (!DialogueManager.current.useDialogueAnimations)
            return;

        if(DialogueManager.current.dialogueAnimator != null)
        {
            if (isTrigger)
                DialogueManager.current.dialogueAnimator.SetTrigger(stateName);
            else
                DialogueManager.current.dialogueAnimator.Play(stateName);
        }
    }
}
