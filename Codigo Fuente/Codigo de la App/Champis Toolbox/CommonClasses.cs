using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public enum ParticlesDensity
{
    None,
    Low,
    Medium,
    High,
    VeryHigh,
    Excessive
}
public enum _resolution
{
    _800x600,
    _1024x768,
    _1280x720,
    _1600x900,
    _1920x1080,
    _2560x1440,
    _3840x2160
}
public enum DisplayModes
{
    Fullscreen, Borderless, Window
}
public enum FPSLimit
{
    _30,
    _60,
    _120,
    _240,
    _480,
    _unlimited
}
public enum ColorblindessType
{
    None,
    Protanopia,
    Deuteranopia,
    Tritanopia
}

public enum Languages
{
    English,
    Spanish
}

public enum UpdateType { Update, LateUpdate, FixedUpdate }

[System.Serializable]
public struct LocalizedString
{
    [TextArea(5, 20)]
    public string englishText;
    [TextArea(5, 20)]
    public string spanishText;
}

public enum OnscreenKeyboardKeyType
{
    normalValue, diaeresis, accent
}

public enum ModalWindowButtons
{
    Ok, YesNo, OkCancel, YesNoCancel
}

[Serializable]
public enum UIElementType
{
    button, dropdown, slider, keyboardKey, toggle
}

[Serializable]
public enum InputSystemButtons
{
    none, yButton, aButton, bButton, xButton, leftShoulder, rightShoulder, leftTrigger, rightTrigger, leftStickButton, rightStickButton, start, select,
    escapeKey, escapeAndBButton
}

public enum PerformanceImpact { Null, Low, Medium, High }

[Serializable]
public class Menu
{
    public string menuName;
    [Tooltip("The gameobject that holds the respective menu.")]
    public GameObject holder;
    [Tooltip("The gameobject of the button that should be selected first, when using a gamepad.")]
    public GameObject firstSelectedButton;
    [Tooltip("The name of the menu to go when 'Back' is pressed.")]
    public string previousMenu;
    public bool disableSelectionTracking;
    [Space]
    public float openEventDelay;
    public UnityEvent onOpen;
    public UnityEvent onClose;

    [HideInInspector] public bool trackLastSelection = true;
    [HideInInspector] public GameObject latestSelectedButton;

    public void ClearLatestSelectedButton()
    {
        latestSelectedButton = null;
    }
}

[Serializable]
public struct TabSection
{
    public string sectionName;
    public Color color;
    [Space]
    [Tooltip("The button that lets you open this tab.")]
    public Button tabButton;
    [Tooltip("The gameobject that holds the respective section.")]
    public GameObject holder;
    [Tooltip("The gameobject of the button that should be selected first, when using a gamepad.")]
    public GameObject firstSelectedButton;
    [Tooltip("The name of the section to go when 'Left Bumper' is pressed.")]
    public string previousSection;
    [Tooltip("The name of the menu to go when 'Right Bumper' is pressed.")]
    public string nextSection;
    [Space]
    public UnityEvent onOpen;
}

public interface IGamepadChange
{
    public void OnGamepadChange(InputDevice device, InputDeviceChange change);
}

public class DeviceSpaceHelper
{
    public static long GetTotalFreeSpace(string path)
    {
        FileInfo f = new FileInfo(path);
        string driveIdentifier = Path.GetPathRoot(f.FullName);

        DriveInfo drive = new DriveInfo(driveIdentifier);

        if (drive.IsReady)
            return drive.AvailableFreeSpace;
        else
            return -1;
    }
}

public static class StringExtensions
{
    public static bool Contains(this string source, string toCheck, bool caseSensitive)
    {
        if (caseSensitive)
            return source?.IndexOf(toCheck, System.StringComparison.Ordinal) >= 0;
        else
            return source?.IndexOf(toCheck, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}

public class BadWordsFilter
{
    public bool ContainsBadWord(string toCheck)
    {
        string[] badWords = new string[]
        {
            //English
            "ARSE",
            "ASS",
            "BITCH",
            "BOLLOCKS",
            "COCK",
            "CRAP",
            "CUNT",
            "DICK",
            "EFFIN",
            "FRIGGER",
            "FUCK",
            "NIGG",
            "PORN",
            "PUSSY",
            "SHIT",
            "SLUT",
            "MISSIONARY",
            "DOGGYSTYLE",
            "DOGGY STYLE",
            "DOGGY-STYLE",
            "69",
            "PAIZURI",
            "AHEGAO",
            "COWGIRL",
            "WEED",
            "TIT",
            "BOOBJOB",
            "BREASTJOB",
            "BUTTJOB",
            "THIGHJOB",
            "THROATJOB",
            "DEEPTHROAT",
            "DEEP THROAT",
            "RAPE",
            "RAPI",
            "MUTILATION",
            "GORE",
            "VORE",
            "FETISH",
            "MURDER",

            //Spanish
            "PENDEJ",
            "CABRON",
            "CABRÓN",
            "PUTO",
            "PUTA",
            "PERRA",
            "TETA",
            "TETOTA",
            "TETONA",
            "TETUDA",
            "ABUSO SEX",
            "ABUSÓ SEX",
            "ABUSAR SEX",
            "COGET",
            "CÓGET",
            "COGERT",
            "CÓGERT",
            "MARICA",
            "MARICON",
            "MARICÓN",
            "MAMADA",
            "PROSTI",
            "MUTILACIÓN",
            "MUTILACION",
            "FETICH",
            "ASESIN"
        };

        foreach (string s in badWords)
            if (toCheck.ToUpper().Contains(s))
                return true;

        return false;
    }
}
