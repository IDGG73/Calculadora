using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TextColoring
{
    public string key;
    public Color color = Color.white;
}
[System.Serializable]
public class DialogueManagerDynamicToken
{
    public string key;
    public string value;

    public DialogueManagerDynamicToken(string tName, string tVal)
    {
        key = tName;
        value = tVal;
    }
}

public class DialogueManager : MonoBehaviour
{
    [Foldout("UI")]
    public string nextDialogueInputAction = "Interact";
    [Space]
    public Canvas dialogueCanvas;
    public HorizontalLayoutGroup rootLayoutHolder;
    [Space]
    public TextMeshProUGUI dialogueDisplay;
    public Image dialogueBox;
    [Header("Optional")]
    public TextMeshProUGUI speakerNameDisplay;
    public GameObject portraitHolder;
    public Image portraitDisplay;
    public Image portraitBackground;
    public GameObject pressToSkipPrompt;
    [Space]
    public VerticalLayoutGroup decisionDisplayList;
    public DialogueDecisionButton decisionButtonTemplate;

    [Foldout("Animation")]
    public Animator dialogueAnimator;
    [Space]
    public string dialogueOpenAnimatorTrigger = "Appear";
    public string dialogueCloseAnimatorTrigger = "Disappear";
    public string dialogueNextAnimatorTrigger = "Next Dialogue";

    [Foldout("Audio")]
    [SerializeField] AudioManagerClip dialogueNextSFX;
    [SerializeField] AudioManagerClip dialogueEndSFX;

    [Foldout("Style")]
    public Sprite friendlyBackground;
    public Sprite foeBackground;
    [Space]
    [Tooltip("Add ':' after the speaker's name.")]
    public bool twoPointsAfterName = true;
    [Tooltip("Wheter to use the Dialogue Animator to play animations .")]
    public bool useDialogueAnimations = true;
    [Tooltip("Use an effect to simulate real-time writing. Just like a text revealer.")]
    public bool useTypewriterAnimation = true;
    [Tooltip("Whether to stop time while a conversation is active. Time will automatically return to its default value (taken on Awake()) once the conversation is over.")]
    public bool freezeTime = true;

    [Foldout("Dynamic Strings")]
    public List<DialogueManagerDynamicToken> tokens = new List<DialogueManagerDynamicToken>();

    [Foldout("Text Coloring")]
    [Tooltip("Special color tags. Write '<color=KEY>' and the dialogue system will automatically replace 'KEY' with the actual color.")]
    public TextColoring[] colorTags;
    [Tooltip("If the dialogue contains these words, those words will be colored")]
    public TextColoring[] coloredWords;

    float defaultTimeScale;

    public static ConversationData data;
    public static Coroutine writeAnimationCoroutine;
    public static List<DialogueDecisionButton> decisionButtons = new List<DialogueDecisionButton>();

    public static DialogueManager current;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        defaultTimeScale = Time.timeScale;
        data = new ConversationData();
    }

    public void DialogueNextSFX() => AudioManager.PlaySound(dialogueNextSFX.clip);
    public void DialogueEndSFX() => AudioManager.PlaySound(dialogueEndSFX.clip);

    public void SetTimeScale(float newTimeScale) => Time.timeScale = newTimeScale;
    public void ResetTimeScale() => Time.timeScale = defaultTimeScale;

    #region DYNAMIC TOKENS
    public string GetToken(string tokenName)
    {
        foreach (DialogueManagerDynamicToken dt in tokens)
            if (dt.key == tokenName)
                return dt.value;

        return null;
    }
    public void SetToken(string tokenName, string tokenValue)
    {
        foreach (DialogueManagerDynamicToken dt in tokens)
            if (dt.key == tokenName)
            {
                dt.value = tokenValue;
                return;
            }

        tokens.Add(new DialogueManagerDynamicToken(tokenName, tokenValue));
    }
    #endregion

    public IEnumerator TypewriterEffect(TextMeshProUGUI textDisplay, float timeBetweenChars, bool useVoiceClips)
    {
        yield return new WaitForEndOfFrame();

        while (textDisplay.maxVisibleCharacters < textDisplay.text.Length)
        {
            if (useVoiceClips)
            {
                AudioClip[] voiceClips = data.currentDialogueSystem.GetCurrentConversationFile().dialogues[data.currentDialogueIndex].character.voiceClips;

                if (voiceClips.Length > 0)
                    AudioManager.PlaySound(voiceClips[Random.Range(0, voiceClips.Length)]);
            }

            textDisplay.maxVisibleCharacters++;
            yield return new WaitForSecondsRealtime(timeBetweenChars);
        }

        writeAnimationCoroutine = null;
    }
}
