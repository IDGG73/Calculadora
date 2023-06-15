using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class KeyboardEnabler : MonoBehaviour, ISubmitHandler
{
    public UnityEvent onKeyboardOpen;
    public UnityEvent onKeyboardClose;

    TMP_InputField champisField;

    private void Start() => champisField = GetComponent<TMP_InputField>();
    public void OnSubmit(BaseEventData data) => OpenKeyboard();

    public void OpenKeyboard()
    {
        if (OnScreenKeyboard.IsShowing() || SettingsManager.currentInputSource != InputSource.Keyboard)
            return;

        OnScreenKeyboard.Show();
        OnScreenKeyboard.SetTargetInputField(champisField);

        OnScreenKeyboard.onClose = onKeyboardClose;
        onKeyboardOpen.Invoke();
    }

    void SelectSelf() => EventSystem.current.SetSelectedGameObject(gameObject);
}
