using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using RoboRyanTron.SearchableEnum;

using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

using UnityEngine.EventSystems;
using TMPro;
using System.IO;
using System;

#region OTHER CLASSES
public enum Language { English, Spanish }

public interface ILanguageChange
{
    public void OnLanguageChange();
}

[System.Serializable]
public class GameSettings
{
    [Header("GAMEPLAY")]
    public int language;
    [Space]
    public float joystickDeadZone = 0.4f;
    public bool gamepadRumble = true;
    public bool holdToRun = true;

    [Header("VIDEO")]
    public int windowWidth;
    public int windowHeight;
    public int refreshRate;
    public int resolutionIndex;
    public FullScreenMode displayMode = FullScreenMode.FullScreenWindow;
    [Space]
    public float renderScale = 1f;

    [Header("AUDIO")]
    public float BGMVolume = 1f;
    public float SFXVolume = 1f;
    public float cutscenesVolume = 1f;

    [Header("KEYBINDINGS")]
    public GamepadLayout preferredGamepadLayout = GamepadLayout.Auto;
    public List<BMInputAction> actions = new List<BMInputAction>();

    public BMInputAction GetInputAction(string actionName)
    {
        foreach (BMInputAction ia in actions)
            if (ia.actionName == actionName)
                return ia;

        return null;
    }

    public void CloneActions(List<BMInputAction> toClone)
    {
        actions = new List<BMInputAction>();
        actions.Clear();

        foreach (BMInputAction ia in toClone)
            actions.Add(new BMInputAction(ia));
    }
}

[System.Serializable]
public class BMInputAction
{
    public string actionName;
    [Space]
    [SearchableEnum] public KeyCode keyBinding;
    [SearchableEnum] public KeyCode secondaryKeyBinding;
    public GamepadInputSystemButtons gamepadBinding;
    [Space]
    public string map;

    public BMInputAction(string _name, KeyCode _main, KeyCode _sec, GamepadInputSystemButtons _game, string _map)
    {
        actionName = _name;
        keyBinding = _main;
        secondaryKeyBinding = _sec;
        gamepadBinding = _game;
        map = _map;
    }
    public BMInputAction(BMInputAction actionToClone)
    {
        actionName = actionToClone.actionName;
        keyBinding = actionToClone.keyBinding;
        secondaryKeyBinding = actionToClone.secondaryKeyBinding;
        gamepadBinding = actionToClone.gamepadBinding;
        map = actionToClone.map;
    }

    public bool IsPressed()
    {
        bool result = Input.GetKey(keyBinding);

        if (result)
            return result;

        result = Input.GetKey(secondaryKeyBinding);

        if (result)
            return result;

        if (Gamepad.current != null && gamepadBinding != GamepadInputSystemButtons.none)
        {
            switch (gamepadBinding)
            {
                case GamepadInputSystemButtons.yButton:
                    if (Gamepad.current.yButton.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.aButton:
                    if (Gamepad.current.aButton.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.bButton:
                    if (Gamepad.current.bButton.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.xButton:
                    if (Gamepad.current.xButton.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.leftBumper:
                    if (Gamepad.current.leftShoulder.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.rightBumper:
                    if (Gamepad.current.rightShoulder.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.leftTrigger:
                    if (Gamepad.current.leftTrigger.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.rightTrigger:
                    if (Gamepad.current.rightTrigger.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.dpadUp:
                    if (Gamepad.current.dpad.up.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.dpadDown:
                    if (Gamepad.current.dpad.down.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.dpadLeft:
                    if (Gamepad.current.dpad.left.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.dpadRight:
                    if (Gamepad.current.dpad.right.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.leftStickButton:
                    if (Gamepad.current.leftStickButton.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.rightStickButton:
                    if (Gamepad.current.rightStickButton.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.select:
                    if (Gamepad.current.selectButton.isPressed)
                        result = true;
                    break;
                case GamepadInputSystemButtons.start:
                    if (Gamepad.current.startButton.isPressed)
                        result = true;
                    break;
            }
        }

        return result;
    }
    public bool WasPressedThisFrame()
    {
        bool result = Input.GetKeyDown(keyBinding);

        if (result)
            return result;

        result = Input.GetKeyDown(secondaryKeyBinding);

        if (result)
            return result;

        if (Gamepad.current != null && gamepadBinding != GamepadInputSystemButtons.none)
        {
            switch (gamepadBinding)
            {
                case GamepadInputSystemButtons.yButton:
                    if (Gamepad.current.yButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.aButton:
                    if (Gamepad.current.aButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.bButton:
                    if (Gamepad.current.bButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.xButton:
                    if (Gamepad.current.xButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftBumper:
                    if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightBumper:
                    if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftTrigger:
                    if (Gamepad.current.leftTrigger.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightTrigger:
                    if (Gamepad.current.rightTrigger.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadUp:
                    if (Gamepad.current.dpad.up.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadDown:
                    if (Gamepad.current.dpad.down.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadLeft:
                    if (Gamepad.current.dpad.left.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadRight:
                    if (Gamepad.current.dpad.right.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftStickButton:
                    if (Gamepad.current.leftStickButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightStickButton:
                    if (Gamepad.current.rightStickButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.select:
                    if (Gamepad.current.selectButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.start:
                    if (Gamepad.current.startButton.wasPressedThisFrame)
                        return true;
                    break;
            }
        }

        return result;
    }
    public bool WasReleasedThisFrame()
    {
        bool result = Input.GetKeyUp(keyBinding);

        if (result)
            return result;

        result = Input.GetKeyUp(secondaryKeyBinding);

        if (result)
            return result;

        if (Gamepad.current != null && gamepadBinding != GamepadInputSystemButtons.none)
        {
            switch (gamepadBinding)
            {
                case GamepadInputSystemButtons.yButton:
                    if (Gamepad.current.yButton.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.aButton:
                    if (Gamepad.current.aButton.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.bButton:
                    if (Gamepad.current.bButton.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.xButton:
                    if (Gamepad.current.xButton.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftBumper:
                    if (Gamepad.current.leftShoulder.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightBumper:
                    if (Gamepad.current.rightShoulder.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftTrigger:
                    if (Gamepad.current.leftTrigger.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightTrigger:
                    if (Gamepad.current.rightTrigger.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadUp:
                    if (Gamepad.current.dpad.up.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadDown:
                    if (Gamepad.current.dpad.down.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadLeft:
                    if (Gamepad.current.dpad.left.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadRight:
                    if (Gamepad.current.dpad.right.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftStickButton:
                    if (Gamepad.current.leftStickButton.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightStickButton:
                    if (Gamepad.current.rightStickButton.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.select:
                    if (Gamepad.current.selectButton.wasReleasedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.start:
                    if (Gamepad.current.startButton.wasReleasedThisFrame)
                        return true;
                    break;
            }
        }

        return result;
    }

    public string GetKeyCodeName(bool useSecondary = false)
    {
        switch (useSecondary ? secondaryKeyBinding : keyBinding)
        {
            case KeyCode.None: return "";
            case KeyCode.Backspace: return "Backspace";
            case KeyCode.Tab: return "Tab";
            case KeyCode.Clear: return "Clr";
            case KeyCode.Return: return "Return";
            case KeyCode.Pause: return "Pause";
            case KeyCode.Escape: return "Esc";
            case KeyCode.Space: return "Space";
            case KeyCode.Exclaim: return "!";
            case KeyCode.DoubleQuote: return "\"";
            case KeyCode.Hash: return "#";
            case KeyCode.Dollar: return "$";
            case KeyCode.Ampersand: return "&";
            case KeyCode.Quote: return "'";
            case KeyCode.LeftParen: return "(";
            case KeyCode.RightParen: return ")";
            case KeyCode.Asterisk: return "*";
            case KeyCode.Plus: return "+";
            case KeyCode.Comma: return ",";
            case KeyCode.Minus: return "-";
            case KeyCode.Period: return ".";
            case KeyCode.Slash: return "/";
            case KeyCode.Alpha0: return "0";
            case KeyCode.Alpha1: return "1";
            case KeyCode.Alpha2: return "2";
            case KeyCode.Alpha3: return "3";
            case KeyCode.Alpha4: return "4";
            case KeyCode.Alpha5: return "5";
            case KeyCode.Alpha6: return "6";
            case KeyCode.Alpha7: return "7";
            case KeyCode.Alpha8: return "8";
            case KeyCode.Alpha9: return "9";
            case KeyCode.Colon: return ":";
            case KeyCode.Semicolon: return ";";
            case KeyCode.Less: return "<";
            case KeyCode.Equals: return "=";
            case KeyCode.Greater: return ">";
            case KeyCode.Question: return "?";
            case KeyCode.At: return "@";
            case KeyCode.LeftBracket: return "[";
            case KeyCode.Backslash: return "\\";
            case KeyCode.RightBracket: return "]";
            case KeyCode.Caret: return "^";
            case KeyCode.Underscore: return "_";
            case KeyCode.BackQuote: return "`";
            case KeyCode.A: return "A";
            case KeyCode.B: return "B";
            case KeyCode.C: return "C";
            case KeyCode.D: return "D";
            case KeyCode.E: return "E";
            case KeyCode.F: return "F";
            case KeyCode.G: return "G";
            case KeyCode.H: return "H";
            case KeyCode.I: return "I";
            case KeyCode.J: return "J";
            case KeyCode.K: return "K";
            case KeyCode.L: return "L";
            case KeyCode.M: return "M";
            case KeyCode.N: return "N";
            case KeyCode.O: return "O";
            case KeyCode.P: return "P";
            case KeyCode.Q: return "Q";
            case KeyCode.R: return "R";
            case KeyCode.S: return "S";
            case KeyCode.T: return "T";
            case KeyCode.U: return "U";
            case KeyCode.V: return "V";
            case KeyCode.W: return "W";
            case KeyCode.X: return "X";
            case KeyCode.Y: return "Y";
            case KeyCode.Z: return "Z";
            case KeyCode.Delete: return "Del";
            case KeyCode.Keypad0: return "0";
            case KeyCode.Keypad1: return "1";
            case KeyCode.Keypad2: return "2";
            case KeyCode.Keypad3: return "3";
            case KeyCode.Keypad4: return "4";
            case KeyCode.Keypad5: return "5";
            case KeyCode.Keypad6: return "6";
            case KeyCode.Keypad7: return "7";
            case KeyCode.Keypad8: return "8";
            case KeyCode.Keypad9: return "9";
            case KeyCode.KeypadPeriod: return ".";
            case KeyCode.KeypadDivide: return "/";
            case KeyCode.KeypadMultiply: return "*";
            case KeyCode.KeypadMinus: return "-";
            case KeyCode.KeypadPlus: return "+";
            case KeyCode.KeypadEnter: return "Enter";
            case KeyCode.KeypadEquals: return "=";
            case KeyCode.UpArrow: return "Up";
            case KeyCode.DownArrow: return "Down";
            case KeyCode.RightArrow: return "Left";
            case KeyCode.LeftArrow: return "Right";
            case KeyCode.Insert: return "Ins";
            case KeyCode.Home: return "Home";
            case KeyCode.End: return "End";
            case KeyCode.PageUp: return "PgUp";
            case KeyCode.PageDown: return "PgDn";
            case KeyCode.F1: return "F1";
            case KeyCode.F2: return "F2";
            case KeyCode.F3: return "F3";
            case KeyCode.F4: return "F4";
            case KeyCode.F5: return "F5";
            case KeyCode.F6: return "F6";
            case KeyCode.F7: return "F7";
            case KeyCode.F8: return "F8";
            case KeyCode.F9: return "F9";
            case KeyCode.F10: return "F10";
            case KeyCode.F11: return "F11";
            case KeyCode.F12: return "F12";
            case KeyCode.F13: return "F13";
            case KeyCode.F14: return "F14";
            case KeyCode.F15: return "F15";
            case KeyCode.Numlock: return "NumLock";
            case KeyCode.CapsLock: return "Caps";
            case KeyCode.ScrollLock: return "Scr";
            case KeyCode.RightShift: return "Shift";
            case KeyCode.LeftShift: return "Shift";
            case KeyCode.RightControl: return "Ctrl";
            case KeyCode.LeftControl: return "Ctrl";
            case KeyCode.RightAlt: return "Alt";
            case KeyCode.LeftAlt: return "Alt";
            case KeyCode.Mouse0: return SettingsManager.currentLanguage == Language.English ? "Left Click" : "Clic Izquierdo";
            case KeyCode.Mouse1: return SettingsManager.currentLanguage == Language.English ? "Right Click" : "Clic Derecho";
            case KeyCode.Mouse2: return SettingsManager.currentLanguage == Language.English ? "Middle Click" : "Clic Medio";
            case KeyCode.Mouse3: return "Mouse3";
            case KeyCode.Mouse4: return "Mouse4";
            case KeyCode.Mouse5: return "Mouse5";
            case KeyCode.Mouse6: return "Mouse6";
            case KeyCode.JoystickButton0: return "(A)";
            case KeyCode.JoystickButton1: return "(B)";
            case KeyCode.JoystickButton2: return "(X)";
            case KeyCode.JoystickButton3: return "(Y)";
            case KeyCode.JoystickButton4: return "(RB)";
            case KeyCode.JoystickButton5: return "(LB)";
            case KeyCode.JoystickButton6: return "(Back)";
            case KeyCode.JoystickButton7: return "(Start)";
            case KeyCode.JoystickButton8: return "(LS)";
            case KeyCode.JoystickButton9: return "(RS)";
            case KeyCode.JoystickButton10: return "J10";
            case KeyCode.JoystickButton11: return "J11";
            case KeyCode.JoystickButton12: return "J12";
            case KeyCode.JoystickButton13: return "J13";
            case KeyCode.JoystickButton14: return "J14";
            case KeyCode.JoystickButton15: return "J15";
            case KeyCode.JoystickButton16: return "J16";
            case KeyCode.JoystickButton17: return "J17";
            case KeyCode.JoystickButton18: return "J18";
            case KeyCode.JoystickButton19: return "J19";
        }
        return null;
    }
    public string GetGamepadButtonName(GamepadLayout layout)
    {
        switch (gamepadBinding)
        {
            case GamepadInputSystemButtons.yButton: return layout == GamepadLayout.PlayStation ? "Triangle" : "Y";
            case GamepadInputSystemButtons.aButton: return layout == GamepadLayout.PlayStation ? "Cross" : "A";
            case GamepadInputSystemButtons.bButton: return layout == GamepadLayout.PlayStation ? "Circle" : "B";
            case GamepadInputSystemButtons.xButton: return layout == GamepadLayout.PlayStation ? "Square" : "X";

            case GamepadInputSystemButtons.leftBumper:
                switch (layout)
                {
                    default: return "LB";
                    case GamepadLayout.PlayStation: return "L1";
                    case GamepadLayout.Switch: return "L";
                }
            case GamepadInputSystemButtons.rightBumper:
                switch (layout)
                {
                    default: return "RB";
                    case GamepadLayout.PlayStation: return "R1";
                    case GamepadLayout.Switch: return "R";
                }

            case GamepadInputSystemButtons.leftTrigger:
                switch (layout)
                {
                    default: return "LT";
                    case GamepadLayout.PlayStation: return "L2";
                    case GamepadLayout.Switch: return "ZL";
                }
            case GamepadInputSystemButtons.rightTrigger:
                switch (layout)
                {
                    default: return "RT";
                    case GamepadLayout.PlayStation: return "R2";
                    case GamepadLayout.Switch: return "ZR";
                }

            case GamepadInputSystemButtons.dpadUp: return "Up";
            case GamepadInputSystemButtons.dpadDown: return "Down";
            case GamepadInputSystemButtons.dpadLeft: return "Left";
            case GamepadInputSystemButtons.dpadRight: return "Right";

            case GamepadInputSystemButtons.leftStickButton: return layout == GamepadLayout.PlayStation ? "L3" : "LS";
            case GamepadInputSystemButtons.rightStickButton: return layout == GamepadLayout.PlayStation ? "R3" : "RS";

            case GamepadInputSystemButtons.select:
                switch (layout)
                {
                    default: return "View";
                    case GamepadLayout.PlayStation: return "Share";
                    case GamepadLayout.Switch: return "Minus";
                }
            case GamepadInputSystemButtons.start:
                switch (layout)
                {
                    default: return "Menu";
                    case GamepadLayout.PlayStation: return "Options";
                    case GamepadLayout.Switch: return "Plus";
                }

            default: return "[None]";
        }
    }
}
#endregion

public class SettingsManager : MonoBehaviour
{
    [Foldout("Asset References")]
    [SerializeField] UniversalRenderPipelineAsset pipelineAsset;
    [SerializeField] UniversalRendererData rendererData;
    [SerializeField] Renderer2DData renderer2DData;
    [Space]
    [SerializeField] AudioMixer BGMMixer;
    [SerializeField] AudioMixer SFXMixer;

    [Foldout("Settings UI")]
    [Space]
    [SerializeField] Slider DeadzoneSlider;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider cinematicsSlider;
    [SerializeField] Slider renderScaleSlider;
    [SerializeField] Toggle rumbleToggle;
    [SerializeField] Toggle runToggle;
    [SerializeField] TMP_Dropdown languageDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown displayModeDropdown;
    [Space]
    [SerializeField] Transform deadzoneRepresentation;
    [Space]
    [SerializeField] bool collectLanguageInterfacesOnAwake;

    [Foldout("Resolution Revertion")]
    [SerializeField] GameObject resolutionRevertWarn;
    [SerializeField] TextMeshProUGUI resolutionRevertCountDisplay;
    [SerializeField] float resolutionRevertCountdownTime = 10f;

    [Foldout("PC Input Icons")]
    [SerializeField] Sprite keyboardKey;
    [Space]
    [SerializeField] Sprite mouseIcon;
    [SerializeField] Sprite leftClickIcon;
    [SerializeField] Sprite rightClickIcon;
    [SerializeField] Sprite middleClickIcon;
    [Space]
    [SerializeField] Sprite upArrowIcon;
    [SerializeField] Sprite downArrowIcon;
    [SerializeField] Sprite leftArrowIcon;
    [SerializeField] Sprite rightArrowIcon;

    [Foldout("Xbox Input Icons")]
    [SerializeField] Sprite xboxAButtonIcon;
    [SerializeField] Sprite xboxBButtonIcon;
    [SerializeField] Sprite xboxXButtonIcon;
    [SerializeField] Sprite xboxYButtonIcon;
    [SerializeField] Sprite LBButtonIcon;
    [SerializeField] Sprite RBButtonIcon;
    [SerializeField] Sprite LTButtonIcon;
    [SerializeField] Sprite RTButtonIcon;
    [SerializeField] Sprite xboxLSButtonIcon;
    [SerializeField] Sprite xboxRSButtonIcon;
    [SerializeField] Sprite viewButtonIcon;
    [SerializeField] Sprite menuButtonIcon;

    [Foldout("PlayStation Input Icons")]
    [SerializeField] Sprite crossButtonIcon;
    [SerializeField] Sprite circleButtonIcon;
    [SerializeField] Sprite squareButtonIcon;
    [SerializeField] Sprite triangleButtonIcon;
    [SerializeField] Sprite L1ButtonIcon;
    [SerializeField] Sprite R1ButtonIcon;
    [SerializeField] Sprite L2ButtonIcon;
    [SerializeField] Sprite R2ButtonIcon;
    [SerializeField] Sprite L3ButtonIcon;
    [SerializeField] Sprite R3ButtonIcon;
    [SerializeField] Sprite shareButtonIcon;
    [SerializeField] Sprite optionsButtonIcon;

    [Foldout("Switch Input Icons")]
    [SerializeField] Sprite switchAButtonIcon;
    [SerializeField] Sprite switchBButtonIcon;
    [SerializeField] Sprite switchXButtonIcon;
    [SerializeField] Sprite switchYButtonIcon;
    [SerializeField] Sprite LButtonIcon;
    [SerializeField] Sprite RButtonIcon;
    [SerializeField] Sprite ZLButtonIcon;
    [SerializeField] Sprite ZRButtonIcon;
    [SerializeField] Sprite switchLSButtonIcon;
    [SerializeField] Sprite switchRSButtonIcon;
    [SerializeField] Sprite minusButtonIcon;
    [SerializeField] Sprite plusButtonIcon;

    [Foldout("Input Bindings")]
    [SerializeField] GameObject waitingForKeyWarn;
    [SerializeField] GameObject duplicateKeyWarn;
    [Space]
    [SerializeField] TextMeshProUGUI countdownDisplay;
    [SerializeField] float bindingCountdown = 5f;
    [Space]
    [SerializeField] List<BMInputAction> baseBindings = new List<BMInputAction>();

    [Foldout("Joystick Debugging")]
    [SerializeField] Transform trackball;
    [SerializeField] Transform joystick;
    [SerializeField] Transform joystickBounds;
    [SerializeField] float joystickRadius;

    [Foldout("Game Settings (Read-Only)", readOnly = true)]
    [SerializeField] GameSettings debugSettings;

    bool joystickDemo;

    GameObject eventSystem;

    List<Resolution> availableResolutions = new List<Resolution>();
    List<ILanguageChange> languageChangers = new List<ILanguageChange>();

    Coroutine rumbleCoroutine;
    Coroutine waitForRebindCoroutine;
    Coroutine resolutionCountdownCoroutine;

    CinematicVolumeBinder[] cinematicVolumeBinders;

    public System.Action<InputSource, GamepadLayout> onInputSourceChange;

    Vector2 lastMousePos;

    public static Gamepad latestCurrentGamepad { get; private set; }
    public static bool mouseIsCurrentSource { get; private set; }
    public static InputSource currentInputSource { get; private set; }
    public static GamepadLayout currentGamepadType { get; private set; }
    public static Language currentLanguage { get; private set; }

    static GameSettings dummySettings;
    public static GameSettings gameSettings { get { return dummySettings; } set { dummySettings = value; current.debugSettings = value; } }
    public static SettingsManager current { get; private set; }

    #region UNITY VOIDS
    [ContextMenu("Generate Empty Settings File")]
    public void _genEmptSettFile()
    {
        CreateNewSettings();
        SaveSettingsToJson();
    }

    private void Awake()
    {
        current = this;
        try { eventSystem = EventSystem.current.gameObject; } catch { }

        onInputSourceChange = (s, t) => { };
        onInputSourceChange += UpdateDialogueTokens;

        try
        {
            if (collectLanguageInterfacesOnAwake)
                CollectILanguageChange();
        } catch { }

        try { GetCinematicVolumeBinders(); } catch { }
        try { RetrieveAvailableResolutions(); } catch { }

        LoadSettingsFromJson();
    }

    private void Start()
    {
        ChampisConsole.CreateConsole();
    }

    private void Update()
    {
        InputSource prevSource = currentInputSource;

        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current != null && GetLeftJoystickSquare(true) != Vector2.zero || Gamepad.current != null && GetRightJoystickSquare() != Vector2.zero)
        {
            currentInputSource = InputSource.Gamepad;

            #region Gamepad Type Recognition
            if(latestCurrentGamepad == null || latestCurrentGamepad != Gamepad.current)
            {
                if (Gamepad.current is UnityEngine.InputSystem.DualShock.DualShockGamepad)
                    currentGamepadType = GamepadLayout.PlayStation;
                /*
                else if (Gamepad.current is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
                    currentGamepadType = GamepadLayout.Switch;*/
                else
                    currentGamepadType = GamepadLayout.Xbox;
            }
            #endregion

            latestCurrentGamepad = Gamepad.current;
        }

        if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
            currentInputSource = InputSource.Keyboard;

        if (Mouse.current != null && Mouse.current.position.ReadValue() != lastMousePos)
        {
            currentInputSource = InputSource.Mouse;
            lastMousePos = Mouse.current.position.ReadValue();
        }

        if (Touchscreen.current != null && Touchscreen.current.IsPressed())
            currentInputSource = InputSource.TouchScreen;

        if (currentInputSource != InputSource.Gamepad)
            currentGamepadType = GamepadLayout.Auto;

        mouseIsCurrentSource = currentInputSource == InputSource.Mouse;

        if (prevSource != currentInputSource)
            onInputSourceChange?.Invoke(currentInputSource, currentGamepadType);

        if (joystickDemo)
            MoveJoystickImage();
    }
    #endregion

    #region CLASSES COLLECTORS
    public void CollectILanguageChange()
    {
        languageChangers = UniversalFunctions.FindInterface<ILanguageChange>(true);
    }
    void GetCinematicVolumeBinders()
    {
        cinematicVolumeBinders = FindObjectsOfType<CinematicVolumeBinder>(true);
    }
    #endregion

    #region SETTERS
    public void SetDeadzone(float value)
    {
        if (deadzoneRepresentation != null)
            deadzoneRepresentation.localScale = new Vector3(value, value, 1);

        gameSettings.joystickDeadZone = value;
    }
    public void SetBGMVolume(float value)
    {
        BGMMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        gameSettings.BGMVolume = value;
    }
    public void SetSFXVolume(float value)
    {
        SFXMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        gameSettings.SFXVolume = value;
    }
    public void SetCinematicsVolume(float value)
    {
        gameSettings.cutscenesVolume = value;

        foreach (CinematicVolumeBinder cvb in cinematicVolumeBinders)
            cvb.SetVolumeFromSettingsManager();
    }

    public void SetRumble(bool value)
    {
        gameSettings.gamepadRumble = value;

        if (value == true)
            rumbleCoroutine = StartCoroutine(RumbleGamepad());
        else
        {
            if (rumbleCoroutine != null)
                StopCoroutine(rumbleCoroutine);

            UniversalFunctions.SetGamepadRumbleSimple(0);
        }
    }
    public void SetRumbleSilently(bool value) => gameSettings.gamepadRumble = value;

    public void SetHoldToRun(bool value) => gameSettings.holdToRun = value;

    public void SetResolutionViaAvailableResolutions(int value)
    {
        if (value == gameSettings.resolutionIndex)
            return;

        gameSettings.resolutionIndex = value;
        SetResolution(availableResolutions[value].width, availableResolutions[value].height, availableResolutions[value].refreshRate, true);
    }
    public void SetResolution(int width, int height, int refreshRate, bool doCountdown)
    {
        if (width == gameSettings.windowWidth && height == gameSettings.windowHeight && refreshRate == gameSettings.refreshRate)
            return;

        if (doCountdown)
            StartCoroutine(_resolutionCountdown(gameSettings.windowWidth, gameSettings.windowHeight, GetResolutionIndex(gameSettings.windowWidth, gameSettings.windowHeight)));

        gameSettings.windowWidth = width;
        gameSettings.windowHeight = height;
        gameSettings.refreshRate = refreshRate;

        Screen.SetResolution(width, height, gameSettings.displayMode, refreshRate);
    }
    public void ApplyResolution() => StopCoroutine(resolutionCountdownCoroutine);
    public void SetDisplayMode(int value)
    {
        FullScreenMode chosenMode = value == 1 ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;

        gameSettings.displayMode = chosenMode;

        if (Screen.fullScreenMode != chosenMode)
            Screen.fullScreenMode = chosenMode;
    }
    public void SetRenderScale(float value)
    {
        gameSettings.renderScale = value;
        pipelineAsset.renderScale = value;
    }

    public void SetPreferredGamepadLayout(int value) => gameSettings.preferredGamepadLayout = (GamepadLayout)value;

    public void DeleteMainBinding(string actionName) => DeleteBinding(actionName, false);
    public void DeleteSecondaryBinding(string actionName) => DeleteBinding(actionName, true);

    public void RebindMainKey(string actionName) => waitForRebindCoroutine = StartCoroutine(_waitForRebind(actionName, false));
    public void RebindSecondaryKey(string actionName) => waitForRebindCoroutine = StartCoroutine(_waitForRebind(actionName, true));

    void DeleteBinding(string actionName, bool secondary)
    {
        foreach (BMInputAction ia in gameSettings.actions)
            if (ia.actionName == actionName)
            {
                if (!secondary)
                    ia.keyBinding = KeyCode.None;
                else
                    ia.secondaryKeyBinding = KeyCode.None;
            }

        onInputSourceChange?.Invoke(currentInputSource, currentGamepadType);
    }
    public void DefaultBindings() { gameSettings.CloneActions(baseBindings); onInputSourceChange?.Invoke(currentInputSource, currentGamepadType); }
    public void CloseBinding()
    {
        if (waitForRebindCoroutine == null)
            return;

        StopCoroutine(waitForRebindCoroutine);
        waitForRebindCoroutine = null;

        eventSystem.SetActive(true);
        waitingForKeyWarn.SetActive(false);
    }

    public void SetLanguage(int value)
    {
        currentLanguage = (Language)value;
        gameSettings.language = value;

        if (!collectLanguageInterfacesOnAwake)
        {
            var changers = UniversalFunctions.FindInterface<ILanguageChange>(true);

            foreach (ILanguageChange c in changers)
                c.OnLanguageChange();
        }
        else
        {
            foreach (ILanguageChange c in languageChangers)
                try { c.OnLanguageChange(); } catch { }
        }
    }

    public void SetInputActionsDialogueTokens(InputSource currentSource)
    {

    }
    #endregion

    #region GETTERS
    public static UniversalRenderPipelineAsset URPAsset()
    {
        if (current)
            return current.pipelineAsset;
        else
        {
            ChampisConsole.LogError("There is no Settings Manager present in the scene");
            return null;
        }
    }
    public static UniversalRendererData URPData()
    {
        if (current)
            return current.rendererData;
        else
        {
            ChampisConsole.LogError("There is no Settings Manager present in the scene");
            return null;
        }
    }
    public static Renderer2DData URP2DData()
    {
        if (current)
            return current.renderer2DData;
        else
        {
            ChampisConsole.LogError("There is no Settings Manager present in the scene");
            return null;
        }
    }

    public void RetrieveAvailableResolutions()
    {
        Resolution[] res = Screen.resolutions;
        List<TMP_Dropdown.OptionData> dropData = new List<TMP_Dropdown.OptionData>();

        foreach (Resolution r in res)
            if (r.refreshRate >= res[res.Length - 1].refreshRate && r.width / r.height == 16 / 9 && ResolutionIsUnique(r.width, r.height))
                availableResolutions.Add(r);

        foreach (Resolution r in availableResolutions)
            dropData.Add(new TMP_Dropdown.OptionData($"{r.width}<size=35>x</size>{r.height}"));

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(dropData);
    }
    bool ResolutionIsUnique(float width, float height)
    {
        foreach (Resolution r in availableResolutions)
            if (r.width == width && r.height == height)
                return false;

        return true;
    }
    int GetResolutionIndex(float width, float height)
    {
        for (int i = 0; i < availableResolutions.Count; i++)
            if (availableResolutions[i].width == width && availableResolutions[i].height == height)
                return i;

        return -1;
    }

    public static BMInputAction GetInputAction(string actionName)
    {
        if (actionName == null || actionName == string.Empty)
        {
            ChampisConsole.LogWarning($"Specified Action Name was {(actionName == null ? "null" : "empty")}");
            return null;
        }

        foreach (BMInputAction ic in gameSettings.actions)
            if (ic.actionName == actionName)
                return ic;

        ChampisConsole.LogWarning($"Input Action '{actionName}' does not exist");
        return null;
    }

    public static bool SetMouseButtonSprite(Image targetDisplay, KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Mouse0: targetDisplay.sprite = current.leftClickIcon; return true;
            case KeyCode.Mouse1: targetDisplay.sprite = current.rightClickIcon; return true;
            case KeyCode.Mouse2: targetDisplay.sprite = current.middleClickIcon; return true;

            default: targetDisplay.sprite = current.mouseIcon; return false;
        }
    }

    public static Sprite GetKeyIcon(BMInputAction action, bool secondaryBinding = false)
    {
        KeyCode toUse = secondaryBinding ? action.secondaryKeyBinding : action.keyBinding;

        switch (toUse)
        {
            case KeyCode.UpArrow: return current.upArrowIcon;
            case KeyCode.DownArrow: return current.downArrowIcon;
            case KeyCode.LeftArrow: return current.leftArrowIcon;
            case KeyCode.RightArrow: return current.rightArrowIcon;

            case KeyCode.Mouse0: return current.leftClickIcon;
            case KeyCode.Mouse1: return current.rightClickIcon;
            case KeyCode.Mouse2: return current.middleClickIcon;
        }

        if (toUse.ToString().Contains("Mouse"))
            return current.mouseIcon;
        else
            return current.keyboardKey;
    }
    public static bool KeyIconIsATemplate(BMInputAction action, bool secondary)
    {
        switch (secondary ? action.secondaryKeyBinding : action.keyBinding)
        {
            case KeyCode.UpArrow: return false;
            case KeyCode.DownArrow: return false;
            case KeyCode.LeftArrow: return false;
            case KeyCode.RightArrow: return false;

            case KeyCode.Mouse0: return false;
            case KeyCode.Mouse1: return false;
            case KeyCode.Mouse2: return false;
        }

        return true;
    }

    public static Sprite GetGamepadButtonIcon(GamepadInputSystemButtons button, GamepadLayout layout = GamepadLayout.Auto)
    {
        if (layout == GamepadLayout.Auto)
            layout = currentGamepadType;

        switch (button)
        {
            case GamepadInputSystemButtons.yButton:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.triangleButtonIcon;
                    case GamepadLayout.Switch: return current.switchYButtonIcon;
                    default: return current.xboxYButtonIcon;
                }
            case GamepadInputSystemButtons.aButton:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.crossButtonIcon;
                    case GamepadLayout.Switch: return current.switchAButtonIcon;
                    default: return current.xboxAButtonIcon;
                }
            case GamepadInputSystemButtons.bButton:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.circleButtonIcon;
                    case GamepadLayout.Switch: return current.switchBButtonIcon;
                    default: return current.xboxBButtonIcon;
                }
            case GamepadInputSystemButtons.xButton:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.squareButtonIcon;
                    case GamepadLayout.Switch: return current.switchXButtonIcon;
                    default: return current.xboxXButtonIcon;
                }

            case GamepadInputSystemButtons.leftBumper:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.L1ButtonIcon;
                    case GamepadLayout.Switch: return current.LButtonIcon;
                    default: return current.LBButtonIcon;
                }
            case GamepadInputSystemButtons.rightBumper:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.R1ButtonIcon;
                    case GamepadLayout.Switch: return current.RButtonIcon;
                    default: return current.RBButtonIcon;
                }
            case GamepadInputSystemButtons.leftTrigger:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.L2ButtonIcon;
                    case GamepadLayout.Switch: return current.ZLButtonIcon;
                    default: return current.LTButtonIcon;
                }
            case GamepadInputSystemButtons.rightTrigger:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.R2ButtonIcon;
                    case GamepadLayout.Switch: return current.ZRButtonIcon;
                    default: return current.RTButtonIcon;
                }

            case GamepadInputSystemButtons.leftStickButton:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.L3ButtonIcon;
                    case GamepadLayout.Switch: return current.switchLSButtonIcon;
                    default: return current.xboxLSButtonIcon;
                }
            case GamepadInputSystemButtons.rightStickButton:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.R3ButtonIcon;
                    case GamepadLayout.Switch: return current.switchRSButtonIcon;
                    default: return current.xboxRSButtonIcon;
                }

            case GamepadInputSystemButtons.select:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.shareButtonIcon;
                    case GamepadLayout.Switch: return current.minusButtonIcon;
                    default: return current.viewButtonIcon;
                }
            case GamepadInputSystemButtons.start:
                switch (layout)
                {
                    case GamepadLayout.PlayStation: return current.optionsButtonIcon;
                    case GamepadLayout.Switch: return current.plusButtonIcon;
                    default: return current.menuButtonIcon;
                }

            default: return null;
        }
    }

    public static Vector2 GetLeftJoystickSquare(bool includeDpad = true)
    {
        if (Gamepad.current == null)
            return Vector2.zero;

        Vector2 axis = Gamepad.current.leftStick.ReadValue();

        if (includeDpad)
            axis += Gamepad.current.dpad.ReadValue();

        axis = new Vector2(Mathf.Clamp(Mathf.Abs(axis.x) < gameSettings.joystickDeadZone ? 0 : axis.x, -1, 1), Mathf.Clamp(Mathf.Abs(axis.y) < gameSettings.joystickDeadZone ? 0 : axis.y, -1, 1));

        return axis;
    }
    public static Vector2 GetRightJoystickSquare()
    {
        if (Gamepad.current == null)
            return Vector2.zero;

        Vector2 axis = Gamepad.current.rightStick.ReadValue();
        axis = new Vector2(Mathf.Clamp(Mathf.Abs(axis.x) < gameSettings.joystickDeadZone ? 0 : axis.x, -1, 1), Mathf.Clamp(Mathf.Abs(axis.y) < gameSettings.joystickDeadZone ? 0 : axis.y, -1, 1));

        return axis;
    }

    public static Vector2 GetLeftJoystick(bool includeDpad = true)
    {
        if (Gamepad.current == null)
            return Vector2.zero;

        Vector2 axis = Gamepad.current.leftStick.ReadValue();

        if (includeDpad)
            axis += Gamepad.current.dpad.ReadValue();

        if (Mathf.Clamp(axis.magnitude, 0, 1) < gameSettings.joystickDeadZone)
            axis = Vector2.zero;

        axis = new Vector2(Mathf.Clamp(axis.x, -1, 1), Mathf.Clamp(axis.y, -1, 1));
        return axis;
    }
    public static Vector2 GetRightJoystick()
    {
        if (Gamepad.current == null)
            return Vector2.zero;

        Vector2 axis = Gamepad.current.rightStick.ReadValue();

        if (Mathf.Clamp(axis.magnitude, 0, 1) < gameSettings.joystickDeadZone)
            axis = Vector2.zero;

        return axis;
    }

    public static Vector2 GetDpad()
    {
        if (Gamepad.current == null)
            return Vector2.zero;

        Vector2 axis = Gamepad.current.dpad.ReadValue();

        return axis;
    }

    public static Vector2 GetBakedAxis(string horizontalAxisName, string verticalAxisName)
    {
        Vector2 axis = new Vector2(Input.GetAxisRaw(horizontalAxisName), Input.GetAxisRaw(verticalAxisName));

        if (Mathf.Clamp(axis.magnitude, 0, 1) < gameSettings.joystickDeadZone)
            axis = Vector2.zero;

        return axis;
    }
    #endregion

    #region JOYSTICK TESTING
    public void ToggleJoystickTesting()
    {
        joystickDemo = !joystickDemo;
        joystick.localPosition = Vector3.zero;

        trackball.gameObject.SetActive(joystickDemo);

        EventSystem.current.SetSelectedGameObject(null);
    }
    public void MoveJoystickImage()
    {
        if (Gamepad.current == null)
            return;

        joystick.localPosition = GetLeftJoystickSquare(false) * joystickRadius;
        trackball.localPosition = Gamepad.current.leftStick.ReadValue() * joystickRadius;

        float dist = Vector3.Distance(joystick.position, joystickBounds.position);

        if (dist > joystickRadius)
        {
            Vector3 fromOrigintoObject = joystick.position - joystickBounds.position;
            fromOrigintoObject *= joystickRadius / dist;
            joystick.position = joystickBounds.position + fromOrigintoObject;
            transform.position = joystick.position;
        }
    }
    #endregion

    #region SAVE/LOAD
    void CreateNewSettings()
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            current = this;
        #endif

        gameSettings = new GameSettings();

        gameSettings.joystickDeadZone = 0.3f;
        gameSettings.BGMVolume = 1f;
        gameSettings.SFXVolume = 1f;
        gameSettings.cutscenesVolume = 1f;
        gameSettings.gamepadRumble = true;

        gameSettings.windowWidth = Screen.currentResolution.width;
        gameSettings.windowHeight = Screen.currentResolution.height;
        gameSettings.refreshRate = Screen.currentResolution.refreshRate;
        gameSettings.displayMode = FullScreenMode.FullScreenWindow;
        gameSettings.resolutionIndex = -1;
        gameSettings.renderScale = 1f;

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Spanish:
                gameSettings.language = 1;
                break;

            default:
                gameSettings.language = 0;
                break;
        }

        gameSettings.preferredGamepadLayout = GamepadLayout.Auto;
        gameSettings.CloneActions(baseBindings);

        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            current = null;
        #endif
    }

    void UpdateDialogueTokens(InputSource source, GamepadLayout layout)
    {
        if (DialogueManager.current != null)
            foreach (BMInputAction ia in gameSettings.actions)
                DialogueManager.current.SetToken(ia.actionName, $"[ {(currentInputSource == InputSource.Gamepad ? ia.GetGamepadButtonName(layout) : ia.GetKeyCodeName())} ]");
    }

    public void SaveSettingsToJson()
    {
        string path = Application.persistentDataPath + "/GameSettings.json";
        File.WriteAllText(path, JsonUtility.ToJson(gameSettings, true));
    }
    void LoadSettingsFromJson() => StartCoroutine(_loadSettingsFromJson());

    public void SetLoadedSettings()
    {
        #region UGUI
        if (DeadzoneSlider)
            DeadzoneSlider.value = gameSettings.joystickDeadZone;

        if (BGMSlider)
            BGMSlider.value = gameSettings.BGMVolume;

        if (SFXSlider)
            SFXSlider.value = gameSettings.SFXVolume;

        if (cinematicsSlider)
            cinematicsSlider.value = gameSettings.cutscenesVolume;

        if (renderScaleSlider)
            renderScaleSlider.value = gameSettings.renderScale;

        if (rumbleToggle)
            rumbleToggle.isOn = gameSettings.gamepadRumble;

        if (runToggle)
            runToggle.isOn = gameSettings.holdToRun;

        if (languageDropdown)
            languageDropdown.value = gameSettings.language;

        if (displayModeDropdown)
            displayModeDropdown.value = gameSettings.displayMode == FullScreenMode.Windowed ? 1 : 0;

        if (resolutionDropdown)
        {
            if (gameSettings.resolutionIndex == -1)
                resolutionDropdown.value = availableResolutions.Count - 1;
            else
                resolutionDropdown.value = gameSettings.resolutionIndex;
        }
        #endregion

        SetDeadzone(gameSettings.joystickDeadZone);
        SetBGMVolume(gameSettings.BGMVolume);
        SetSFXVolume(gameSettings.SFXVolume);
        SetCinematicsVolume(gameSettings.cutscenesVolume);
        SetRumbleSilently(gameSettings.gamepadRumble);
        SetLanguage(gameSettings.language);
        SetResolution(gameSettings.windowWidth, gameSettings.windowHeight, gameSettings.refreshRate, false);
        SetDisplayMode(gameSettings.displayMode == FullScreenMode.Windowed ? 1 : 0);
        SetRenderScale(gameSettings.renderScale);

        #region UGUI
        if (languageDropdown)
            languageDropdown.RefreshShownValue();

        if (resolutionDropdown)
            resolutionDropdown.RefreshShownValue();

        if (displayModeDropdown)
            displayModeDropdown.RefreshShownValue();
        #endregion
    }
    #endregion

    #region COROUTINES
    IEnumerator RumbleGamepad()
    {
        UniversalFunctions.SetGamepadRumbleSimple(0.5f);
        yield return new WaitForSeconds(0.3f);
        UniversalFunctions.SetGamepadRumbleSimple(0);

        rumbleCoroutine = null;
        yield return new WaitForEndOfFrame();
    }
    IEnumerator _loadSettingsFromJson()
    {
        string path = Application.persistentDataPath + "/GameSettings.json";

        if (File.Exists(path))
            gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(path));
        else
            CreateNewSettings();

        try { SetLoadedSettings(); } catch (System.Exception e) { ChampisConsole.LogError("Failed at 'SetLoadedSettings()': " + e.ToString()); }

        yield return new WaitForEndOfFrame();

        UpdateDialogueTokens(currentInputSource, currentGamepadType);
    }

    IEnumerator _waitForRebind(string actionName, bool secondary)
    {
        yield return new WaitForEndOfFrame();

        float t = bindingCountdown;

        bool duplicateConflict = false;
        bool ignoreKey = false;

        KeyCode pressedKey = KeyCode.None;
        eventSystem = EventSystem.current.gameObject;

        eventSystem.SetActive(false);
        waitingForKeyWarn.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        while (pressedKey == KeyCode.None && t > 0)
        {
            t -= Time.deltaTime;
            countdownDisplay.text = Mathf.RoundToInt(t).ToString();

            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
                if (Input.GetKeyDown(kcode))
                    if (!kcode.ToString().Contains("Joystick"))
                        pressedKey = kcode;

            yield return null;
        }

        if (pressedKey != KeyCode.None)
        {
            foreach (BMInputAction ia in gameSettings.actions)
            {
                if ((ia.keyBinding == pressedKey || ia.secondaryKeyBinding == pressedKey) && ia.map == GetInputAction(actionName).map)
                {
                    if (ia.actionName != actionName)
                        duplicateConflict = true;
                    else
                        ignoreKey = true;
                }
            }

            if (!duplicateConflict && !ignoreKey)
            {
                foreach (BMInputAction ia in gameSettings.actions)
                    if (ia.actionName == actionName)
                    {
                        if (!secondary)
                            ia.keyBinding = pressedKey;
                        else
                            ia.secondaryKeyBinding = pressedKey;
                    }

                onInputSourceChange?.Invoke(currentInputSource, currentGamepadType);
            }
        }

        if (duplicateConflict)
            StartCoroutine(_duplicateWarn());

        yield return new WaitForEndOfFrame();

        CloseBinding();
    }
    IEnumerator _duplicateWarn()
    {
        yield return new WaitForEndOfFrame();

        duplicateKeyWarn.SetActive(true);

        while (!GetInputAction("Jump").WasPressedThisFrame()) { eventSystem.SetActive(false); yield return null; }
        yield return new WaitForEndOfFrame();

        eventSystem.SetActive(true);
        duplicateKeyWarn.SetActive(false);
    }

    IEnumerator _resolutionCountdown(int ogWidth, int ogHeight, int dropdownValue)
    {
        yield return new WaitForEndOfFrame();

        float t = resolutionRevertCountdownTime;
        resolutionRevertWarn.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        while (t > 0)
        {
            t -= Time.deltaTime;

            eventSystem.SetActive(false);
            resolutionRevertCountDisplay.text = Mathf.RoundToInt(t).ToString();

            if (GetInputAction("Jump").WasPressedThisFrame())
            {
                eventSystem.SetActive(true);
                resolutionRevertWarn.SetActive(false);

                yield break;
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        eventSystem.SetActive(true);
        resolutionRevertWarn.SetActive(false);

        gameSettings.resolutionIndex = dropdownValue;
        resolutionDropdown.SetValueWithoutNotify(dropdownValue);
        resolutionDropdown.RefreshShownValue();

        SetResolution(ogWidth, ogHeight, 0, false);
    }
    #endregion
}
