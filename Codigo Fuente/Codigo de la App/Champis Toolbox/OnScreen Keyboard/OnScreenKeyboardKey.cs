using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnScreenKeyboardKey : MonoBehaviour, ISubmitHandler, IPointerEnterHandler, IPointerClickHandler, IKeyboardShift
{
    [Tooltip("The string that must be added to the InputField. This can be more than one character.")]
    public string key;
    [Tooltip("The string that must be added to the InputField when 'Shift' is pressed. This can be more than one character.")]
    public string shiftKey;
    public OnScreenKeyboard keyboard;
    [Space]
    public OnscreenKeyboardKeyType valueType;

    Text label;
    string actualKey;

    OnscreenKeyboardKeyType defaultType;

    private void Start()
    {
        defaultType = valueType;
        actualKey = key;

        label = GetComponentInChildren<Text>();
        label.text = key;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SubmitKey();
    }
    public void OnSubmit(BaseEventData data)
    {
        SubmitKey();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnKeyboardShift(bool shifting)
    {
        if (shifting)
            actualKey = shiftKey;
        else
            actualKey = key;

        label.text = actualKey;

        if (valueType != OnscreenKeyboardKeyType.normalValue && shifting)
            valueType = OnscreenKeyboardKeyType.normalValue;

        if (valueType != defaultType && !shifting)
            valueType = defaultType;
    }

    public void SubmitKey()
    {
        switch (valueType)
        {
            case OnscreenKeyboardKeyType.normalValue:
                keyboard.WriteLetter(actualKey);
                break;
            case OnscreenKeyboardKeyType.diaeresis:
                keyboard.WaitForDiaeresis();
                break;
            case OnscreenKeyboardKeyType.accent:
                keyboard.WaitForAccent();
                break;
        }
    }

    [ContextMenu("Find Neighbours Automatically")]
    public void FindNeigh()
    {
        Button thisButton = GetComponent<Button>();

        if (thisButton.navigation.mode != Navigation.Mode.Automatic)
        {
            Debug.LogError("The navigation mode is not set to 'Automatic' in '" + thisButton.gameObject.name + "'");
            return;
        }

        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;

        newNav.selectOnUp = thisButton.FindSelectableOnUp();
        newNav.selectOnDown = thisButton.FindSelectableOnDown();
        newNav.selectOnLeft = thisButton.FindSelectableOnLeft();
        newNav.selectOnRight = thisButton.FindSelectableOnRight();

        thisButton.navigation = newNav;
    }
}
