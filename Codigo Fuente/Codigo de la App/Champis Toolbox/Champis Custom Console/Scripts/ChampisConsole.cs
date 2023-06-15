using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampisConsole : MonoBehaviour
{
    enum LogMessageType { Message, Warning, Error }

    [Foldout("General")]
    [SerializeField] bool allowConsole = true;
    [SerializeField]
    [Tooltip("Maximum amount of messages than can be printed.\n\nIf this limit is met, older messages will be removed to add-in new ones.")]
    int maximumMessages = 15;

    [SerializeField]
    [Tooltip("Open the console OnAwake()\n\nIf false, console will be hidden until manually opening it.")]
    bool showOnAwake;

    [SerializeField]
    [Tooltip("While in Unity Editor, messages will appear in Unity's built-in Console instead.")]
    bool sendMessagesToEditorConsole = false;

    [Space, SerializeField]
    [Tooltip("Press the following keys to open/close the console.")]
    BMInputAction[] toggleCombo = new BMInputAction[]
    { 
        new BMInputAction("CTRL / LS", KeyCode.LeftControl, KeyCode.None, GamepadInputSystemButtons.leftStickButton, "Console"),
        new BMInputAction("ALT / RS", KeyCode.LeftAlt, KeyCode.None, GamepadInputSystemButtons.rightStickButton, "Console")
    };

    [Foldout("UI")]
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI consoleTitle;
    [SerializeField] TextMeshProUGUI openingComboPrompt;
    [SerializeField] Image notificationArea;
    [Space]
    [SerializeField] ChampisConsoleMessage messageTemplate;
    [SerializeField] Transform messageList;

    [Foldout("Style")]
    [SerializeField] Sprite messageIcon;
    [SerializeField] Sprite warningIcon;
    [SerializeField] Sprite errorIcon;
    [Space]
    [SerializeField] Color messageColor = Color.white;
    [SerializeField] Color warningColor = Color.yellow;
    [SerializeField] Color errorColor = Color.red;

    Coroutine notificationCoroutine;
    Color notificationAreaDefaultColor;

    //STATIC
    public static bool isShowing;

    public static ChampisConsole current;
    static List<ChampisConsoleMessage> printedMessages = new List<ChampisConsoleMessage>();

    void Awake()
    {
        if (current == null)
        {
            current = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!animator)
            animator = current.GetComponent<Animator>();

        consoleTitle.text = $"{Application.productName}'s Runtime Debug Console";
        notificationAreaDefaultColor = notificationArea.color;

        Application.logMessageReceived += HookToDebug;

        if (showOnAwake)
            Show();
        else
            Hide();

        if (!allowConsole)
            gameObject.SetActive(false);
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        SetOpeningPrompt(SettingsManager.currentInputSource, SettingsManager.currentGamepadType);

        yield return new WaitUntil(() => SettingsManager.current != null);
        yield return new WaitForEndOfFrame();

        SetOpeningPrompt(SettingsManager.currentInputSource, SettingsManager.currentGamepadType);
        SettingsManager.current.onInputSourceChange += SetOpeningPrompt;

        yield return null;
    }

    private void HookToDebug(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log: Log(condition); break;
            case LogType.Warning: LogWarning(condition); break;
            case LogType.Error: LogError(condition); break;

            case LogType.Assert: LogError(condition); break;
            case LogType.Exception: LogError(condition); break;
        }
    }

    void OnDestroy()
    {
        if (current == this)
            SettingsManager.current.onInputSourceChange -= SetOpeningPrompt;
    }

    void LateUpdate()
    {
        bool holdingAll = true;

        for (int i = 0; i < toggleCombo.Length - 1; i++)
            if (!toggleCombo[i].IsPressed())
            {
                holdingAll = false;
                return;
            }

        if (holdingAll)
            if (toggleCombo[toggleCombo.Length - 1].WasPressedThisFrame())
                ToggleVisibility();
    }

    #region VISIBILITY CONTROLS
    public static void ToggleVisibility()
    {
        if (isShowing)
            Hide();
        else
            Show();
    }
    public static void Show()
    {
        isShowing = true;
        current.animator.SetBool("Showing", true);
    }
    public static void Hide()
    {
        isShowing = false;
        current.animator.SetBool("Showing", false);
    }
    #endregion

    public void SetOpeningPrompt(InputSource source, GamepadLayout layout)
    {
        if (layout == GamepadLayout.Auto)
            layout = SettingsManager.currentGamepadType;

        string bake = string.Empty;

        if (source == InputSource.Gamepad)
            for (int i = 0; i < toggleCombo.Length; i++)
                bake = bake + $"{toggleCombo[i].GetGamepadButtonName(layout)}" + ((i != toggleCombo.Length - 1) ? " + " : string.Empty);
        else
            for (int i = 0; i < toggleCombo.Length; i++)
                bake = bake + $"{toggleCombo[i].GetKeyCodeName()}" + ((i != toggleCombo.Length - 1) ? " + " : string.Empty);

        openingComboPrompt.text = bake;
    }

    public static void CreateConsole()
    {
        if (current == null)
            Instantiate(Resources.Load("Champis Custom Console/Custom Console"));
    }

    public static void Clear()
    {
        if (current == null || Application.isEditor)
            return;

        foreach (ChampisConsoleMessage cm in printedMessages)
            Destroy(cm.gameObject);

        printedMessages.Clear();
    }

    public static void Print(object message) => Log(message);
    public static void Print(object message, Sprite icon) => Log(message, icon);

    public static void Log(object message) => PrintMessage(message, LogMessageType.Message, null);
    public static void LogWarning(object message) => PrintMessage(message, LogMessageType.Warning, null);
    public static void LogError(object message) => PrintMessage(message, LogMessageType.Error, null);

    public static void Log(object message, Sprite icon) => PrintMessage(message, LogMessageType.Message, icon);
    public static void LogWarning(object message, Sprite icon) => PrintMessage(message, LogMessageType.Warning, icon);
    public static void LogError(object message, Sprite icon) => PrintMessage(message, LogMessageType.Error, icon);

    static void PrintMessage(object message, LogMessageType type, Sprite icon)
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            switch (type)
            {
                case LogMessageType.Message:
                    Debug.Log(message);
                    break;

                case LogMessageType.Warning:
                    Debug.LogWarning(message);
                    break;

                case LogMessageType.Error:
                    Debug.LogError(message);
                    break;
            }

            return;
        }
        #endif

        if (current == null)
            return;

        if (Application.isEditor && current.sendMessagesToEditorConsole)
        {
            switch (type)
            {
                case LogMessageType.Message:
                    Debug.Log(message);
                    break;

                case LogMessageType.Warning:
                    Debug.LogWarning(message);
                    break;

                case LogMessageType.Error:
                    Debug.LogError(message);
                    break;
            }

            return;
        }

        if (current == null || !current.gameObject.activeInHierarchy)
            return;

        //Look if there is a message identical to this one
        ChampisConsoleMessage repeatedMessage = null;

        foreach (ChampisConsoleMessage ccm in printedMessages)
        {
            if (ccm.messageDisplay.text.Contains(message.ToString()))
            {
                repeatedMessage = ccm;
                break;
            }
        }

        ChampisConsoleMessage currentMessage;

        if (!repeatedMessage)
        {
            if (printedMessages.Count >= current.maximumMessages)
            {
                ChampisConsoleMessage _dM = printedMessages[0];
                printedMessages.Remove(_dM);

                Destroy(_dM.gameObject);
            }

            currentMessage = Instantiate(current.messageTemplate, current.messageList);
            printedMessages.Add(currentMessage);
        }
        else { currentMessage = repeatedMessage; }

        switch (type)
        {
            case LogMessageType.Message:
                currentMessage.SetMessage(message.ToString(), repeatedMessage ? currentMessage.Count + 1 : 1, icon ? icon : current.messageIcon, current.messageColor);
                currentMessage.messageDisplay.color = current.messageColor;

                current.DoNotification(current.messageColor);
                break;

            case LogMessageType.Warning:
                currentMessage.SetMessage(message.ToString(), repeatedMessage ? currentMessage.Count + 1 : 1, icon ? icon : current.warningIcon, current.warningColor);
                currentMessage.messageDisplay.color = current.warningColor;

                current.DoNotification(current.warningColor);
                break;

            case LogMessageType.Error:
                currentMessage.SetMessage(message.ToString(), repeatedMessage ? currentMessage.Count + 1 : 1, icon ? icon : current.errorIcon, current.errorColor);
                currentMessage.messageDisplay.color = current.errorColor;

                current.DoNotification(current.errorColor);
                break;
        }
    }

    void DoNotification(Color targetColor)
    {
        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(MessageNotification(targetColor));
    }

    IEnumerator MessageNotification(Color targetColor)
    {
        float t = 0f;
        Color baked = new Color(targetColor.r, targetColor.g, targetColor.b, notificationAreaDefaultColor.a);

        while(t < 1)
        {
            t += Time.deltaTime * 5f;
            notificationArea.color = Color.Lerp(notificationArea.color, baked, t);

            yield return null;
        }

        t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            notificationArea.color = Color.Lerp(baked, notificationAreaDefaultColor, t);

            yield return null;
        }

        notificationCoroutine = null;
        yield return null;
    }
}
