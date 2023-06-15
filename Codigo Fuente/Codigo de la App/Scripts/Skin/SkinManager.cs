using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkinManager : MonoBehaviour
{
    [Foldout("App General Style")]
    [SerializeField] Gradient backgroundGradient;
    [SerializeField] Gradient keyboardSlabGradient;
    [Space]
    [SerializeField] Color operatorsColor = Color.magenta;
    [SerializeField] Color functionsColor = Color.green;
    [SerializeField] Color errorColor = Color.red;
    [SerializeField] Gradient advancedGradientColor;

    [Foldout("Display Text Style")]
    [SerializeField] TMP_FontAsset displayFontAsset;
    [SerializeField] Color displayTextColor = Color.white;
    [Space]
    [SerializeField] float displayMinimumTextSize = 10f;
    [SerializeField] float displayMaximumTextSize = 50f;
    [Space]
    [SerializeField] float expressionMinimumTextSize = 10f;
    [SerializeField] float expressionMaximumTextSize = 50f;

    [Foldout("Calculator Keyboard UI")]
    [Header("Buttons Style")]
    [SerializeField] Gradient buttonGradientColor;
    [SerializeField] float middleSizedbuttonRoundness = 1f;
    [SerializeField] float smallSizedButtonRoundness = 1.3f;
    [Header("Text Style")]
    [SerializeField] TMP_FontAsset buttonsFontAsset;
    [SerializeField] Color buttonsTextColor = Color.white;
    [Space]
    [SerializeField] float buttonsMinimumTextSize = 10f;
    [SerializeField] float buttonsMaximumTextSize = 35f;

    #region Wrappers
    public Gradient BackgroundGradient { get { return backgroundGradient; } }
    public Gradient KeyboardSlabGradient { get { return keyboardSlabGradient; } }
    public Gradient AdvancedGradientColor { get { return advancedGradientColor; } }

    public Color DisplayTextColor { get { return displayTextColor; } }

    public Color OperatorsColor { get { return operatorsColor; } }
    public Color FunctionsColor { get { return functionsColor; } }
    public Color ErrorColor { get { return errorColor; } }

    public float ButtonsMinimumTextSize { get { return buttonsMinimumTextSize; } }
    public float ButtonsMaximumTextSize { get { return buttonsMaximumTextSize; } }

    public float MiddleSizedButtonRoundness { get { return middleSizedbuttonRoundness; } }
    public float SmallSizedButtonRoundness { get { return smallSizedButtonRoundness; } }

    public Color ButtonsTextColor { get { return buttonsTextColor; } }

    public float DisplayMinimumTextSize { get { return displayMinimumTextSize; } }
    public float DisplayMaximumTextSize { get { return displayMaximumTextSize; } }

    public float ExpressionMinimumTextSize { get { return expressionMinimumTextSize; } }
    public float ExpressionMaximumTextSize { get { return expressionMaximumTextSize; } }

    public Gradient ButtonGradientColor { get { return buttonGradientColor; } }

    public TMP_FontAsset DisplayFontAsset { get { return displayFontAsset; } }
    public TMP_FontAsset ButtonsFontAsset { get { return buttonsFontAsset; } }
    #endregion

    static SkinManager instance;
    public static SkinManager current { get { if (instance == null) instance = GameObject.FindObjectOfType<SkinManager>(); return instance; } }
}
