using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public interface IKeyboardShift
{
    public void OnKeyboardShift(bool shiftEnabled);
}

public class OnScreenKeyboard : MonoBehaviour
{
    [Tooltip("This keyboard's animator component.")]
    [SerializeField] Animator animator;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] GameObject firstSelectedKey;
    [Space]
    [SerializeField] OnScreenKeyboardKey[] keys;

    bool shifting;
    bool waitingDiaeresis;
    bool waitingAccent;
    bool rememberText;

    //IKeyboardShift[] shifters;

    //Static stuff
    public static UnityEvent onClose = new UnityEvent();
    static OnScreenKeyboard instance;

    [ContextMenu("Get Keys")]
    public void _getK() => keys = FindObjectsOfType<OnScreenKeyboardKey>(true);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
            CloseAndKeepWriting();
    }

    #region WRITING FUNCTIONS
    public void WriteLetter(string character)
    {
        if (waitingDiaeresis)
        {
            switch (character)
            {
                case "a":
                    inputField.text += "�";
                    break;
                case "e":
                    inputField.text += "�";
                    break;
                case "i":
                    inputField.text += "�";
                    break;
                case "o":
                    inputField.text += "�";
                    break;
                case "u":
                    inputField.text += "�";
                    break;
                case "A":
                    inputField.text += "�";
                    break;
                case "E":
                    inputField.text += "�";
                    break;
                case "I":
                    inputField.text += "�";
                    break;
                case "O":
                    inputField.text += "�";
                    break;
                case "U":
                    inputField.text += "�";
                    break;

                default:
                    inputField.text += "�" + character;
                    break;
            }
        }
        else if (waitingAccent)
        {
            switch (character)
            {
                case "a":
                    inputField.text += "�";
                    break;
                case "e":
                    inputField.text += "�";
                    break;
                case "i":
                    inputField.text += "�";
                    break;
                case "o":
                    inputField.text += "�";
                    break;
                case "u":
                    inputField.text += "�";
                    break;
                case "A":
                    inputField.text += "�";
                    break;
                case "E":
                    inputField.text += "�";
                    break;
                case "I":
                    inputField.text += "�";
                    break;
                case "O":
                    inputField.text += "�";
                    break;
                case "U":
                    inputField.text += "�";
                    break;

                default:
                    inputField.text += "�" + character;
                    break;
            }
        }

        if (!waitingDiaeresis && !waitingAccent)
            inputField.text += character;

        waitingDiaeresis = false;
        waitingAccent = false;
    }
    public void EraseLetter()
    {
        waitingDiaeresis = false;
        waitingAccent = false;

        if (inputField.text.Length > 0)
            inputField.text = inputField.text.Remove(inputField.text.Length - 1);
    }
    public void EraseAll()
    {
        inputField.text = string.Empty;
    }

    public void WaitForDiaeresis()
    {
        if (waitingAccent)
        {
            waitingAccent = false;

            WriteLetter("��");
            return;
        }

        if (waitingDiaeresis)
        {
            waitingDiaeresis = false;

            WriteLetter("��");
            return;
        }

        waitingDiaeresis = true;
    }

    public void WaitForAccent()
    {
        if (waitingAccent)
        {
            waitingAccent = false;

            WriteLetter("��");
            return;
        }

        if (waitingDiaeresis)
        {
            waitingDiaeresis = false;

            WriteLetter("��");
            return;
        }

        waitingAccent = true;
    }
    #endregion

    void OpenKeyboard()
    {
        if (gameObject.activeInHierarchy)
            return;

        gameObject.SetActive(true);
        StartCoroutine(_selectFirstKey());
    }
    public void CloseKeyboard()
    {
        animator.Play("Keyboard Popout");
        onClose.Invoke();

        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }
    public void CloseAndKeepWriting()
    {
        CloseKeyboard();
        StartCoroutine(_enableEditionWithoutSelectingText());
    }
    IEnumerator _enableEditionWithoutSelectingText()
    {
        yield return new WaitForEndOfFrame();

        bool defaultFocusSelectAll = inputField.onFocusSelectAll;
        inputField.onFocusSelectAll = false;

        yield return new WaitForEndOfFrame();

        inputField.ActivateInputField();
        inputField.caretPosition = inputField.text.Length;

        yield return new WaitForEndOfFrame();

        inputField.onFocusSelectAll = defaultFocusSelectAll;
    }

    IEnumerator _selectFirstKey()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstSelectedKey);
        ExecuteEvents.Execute(inputField.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.selectHandler);
    }

    void DisableObject() => gameObject.SetActive(false);

    public void ToggleShift()
    {
        shifting = !shifting;
        CallShifters();
    }

    void CallShifters()
    {
        foreach (OnScreenKeyboardKey s in keys)
            s.OnKeyboardShift(shifting);
    }

    public bool IsShifting() => shifting;

    #region STATIC FUNCTIONS
    public static void Show() => instance.OpenKeyboard();
    public static bool IsShowing()
    {
        if (instance == null)
            return false;

        return instance.gameObject.activeInHierarchy;
    }
    public static void SetTargetInputField(TMP_InputField newTarget) => instance.inputField = newTarget;
    #endregion
}
