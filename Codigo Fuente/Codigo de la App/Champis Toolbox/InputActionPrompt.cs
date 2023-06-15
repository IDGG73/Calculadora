using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputActionPrompt : MonoBehaviour
{
    [SerializeField] string inputActionBinding;
    [SerializeField] bool useSecondaryBinding;
    [Space]
    [SerializeField] Image display;
    [SerializeField] TextMeshProUGUI keyboardLabel;

    private void Start()
    {
        SettingsManager.current.onInputSourceChange += RefreshInputSource;
        RefreshInputSource(SettingsManager.currentInputSource, SettingsManager.currentGamepadType);
    }
    private void OnDestroy() => SettingsManager.current.onInputSourceChange -= RefreshInputSource;

    public void RefreshInputSource(InputSource source, GamepadLayout layout)
    {
        if (display == null || keyboardLabel == null)
            return;

        BMInputAction tmpAction = SettingsManager.GetInputAction(inputActionBinding);

        switch (source)
        {
            case InputSource.Gamepad:
                display.sprite = SettingsManager.GetGamepadButtonIcon(tmpAction.gamepadBinding, layout);
                keyboardLabel.gameObject.SetActive(true);
                keyboardLabel.text = string.Empty;
                break;

            default:
                display.sprite = SettingsManager.GetKeyIcon(tmpAction, useSecondaryBinding);
                keyboardLabel.text = tmpAction.GetKeyCodeName();

                keyboardLabel.gameObject.SetActive(SettingsManager.KeyIconIsATemplate(tmpAction, useSecondaryBinding));
                break;
        }
    }
}
