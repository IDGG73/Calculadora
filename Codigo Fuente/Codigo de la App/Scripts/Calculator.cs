using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public enum MathSymbol { Add, Subtract, Multiply, Divide }

public class Calculator : MonoBehaviour
{
    [Foldout("Calculator")]
    [SerializeField] string latestExpression;
    [SerializeField] string inputFieldContent;
    [Space]
    [SerializeField] List<string> history = new List<string>();

    [Foldout("UI")]
    [SerializeField] TextMeshProUGUI latestExpressionDisplay;
    [SerializeField] TextMeshProUGUI inputFieldDisplay;
    [SerializeField] Animator advancedKeyboardSlabAnimator;
    [Space]
    [SerializeField] TextMeshProUGUI historyDisplay;
    [SerializeField] Animator historySlabAnimator;
    [Space]
    [SerializeField] Animator angleUnitToggle;
    [SerializeField] GameObject muteIconObject;

    [Foldout("Settings")]
    [SerializeField] string errorMessage = "Error de Sintaxis";
    [SerializeField] string squareRootSymbol;
    [SerializeField] string PISymbol;
    [SerializeField, Min(float.Epsilon)] float resultLerpSpeed = 1f;

    [Foldout("Debug", readOnly = true)]
    [SerializeField] bool historyOpen;
    [SerializeField] bool advancedOpen;
    [SerializeField] bool usingDegrees = true;
    [Space]
    [SerializeField] string latestResult;

    public static string SquareRootSymbol { get { if (current) return current.squareRootSymbol; else return "?"; } }
    public static System.Globalization.CultureInfo DefaultCulture;
    public static Calculator current;

    Coroutine lerpResultCoroutine;

    private void Awake()
    {
        current = this;

        DefaultCulture = new System.Globalization.CultureInfo("en-US");
        DefaultCulture.NumberFormat.NumberDecimalSeparator = ".";

        Thread.CurrentThread.CurrentCulture = DefaultCulture;
    }

    public void WriteCharacter(string toWrite)
    {
        if (inputFieldDisplay.text.Contains(errorMessage))
        {
            inputFieldContent = string.Empty;
            inputFieldDisplay.text = string.Empty;
        }

        switch (toWrite)
        {
            case "/":
                inputFieldContent = inputFieldContent + "÷";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> ÷ </color>";
                break;
            case "*":
                inputFieldContent = inputFieldContent + "*";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> × </color>";
                break;
            case "+":
                inputFieldContent = inputFieldContent + "+";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> + </color>";
                break;
            case "-":
                inputFieldContent = inputFieldContent + "-";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> - </color>";
                break;
            case "<":
                inputFieldContent = inputFieldContent + "~";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> < </color>";
                break;
            case ">":
                inputFieldContent = inputFieldContent + ":";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> > </color>";
                break;
            case "(":
                inputFieldContent = inputFieldContent + "(";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>(</color>";
                break;
            case ")":
                inputFieldContent = inputFieldContent + ")";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>)</color>";
                break;
            case "Sin(":
                inputFieldContent = inputFieldContent + "_";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin(</color>";
                break;
            case "Cos(":
                inputFieldContent = inputFieldContent + "'";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos(</color>";
                break;
            case "Tan(":
                inputFieldContent = inputFieldContent + ";";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Tan(</color>";
                break;
            case "Asin(":
                inputFieldContent = inputFieldContent + "\"";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin<sup>-1</sup>(</color>";
                break;
            case "Acos(":
                inputFieldContent = inputFieldContent + "$";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos<sup>-1</sup>(</color>";
                break;
            case "Atan(":
                inputFieldContent = inputFieldContent + "%";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Tan<sup>-1</sup>(</color>";
                break;
            case "Sqrt(":
                inputFieldContent = inputFieldContent + "\\";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>\\(</color>";
                break;
            case "Pi":
                inputFieldContent = inputFieldContent + PISymbol;
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{PISymbol}</color>";
                break;
            case "^":
                inputFieldContent = inputFieldContent + "**";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>^</color>";
                break;
            case "Log(":
                inputFieldContent = inputFieldContent + "ñ";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Log(</color>";
                break;
            case "Ln(":
                inputFieldContent = inputFieldContent + "[";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Ln(</color>";
                break;

            default:
                inputFieldContent = inputFieldContent + toWrite;
                inputFieldDisplay.text = inputFieldDisplay.text + toWrite;
                break;
        }
    }
    public void Backspace()
    {
        if (inputFieldDisplay.text.Contains(errorMessage))
        {
            inputFieldContent = string.Empty;
            inputFieldDisplay.text = string.Empty;

            return;
        }

        if (inputFieldContent.Length <= 0)
            return;

        inputFieldContent = inputFieldContent.Remove(inputFieldContent.Length - 1, 1);
        inputFieldDisplay.text = GetInputFieldContentFormatted();
    }
    public void WriteLatestResult() => WriteCharacter(latestResult);

    public void Calculate()
    {
        if (string.IsNullOrEmpty(inputFieldContent))
            return;

        NCalc.Expression e = new NCalc.Expression(inputFieldContent
            .Replace("÷", "/")
            .Replace("~", "<")
            .Replace(":", ">")
            .Replace("\\", "Sqrt(")
            .Replace("_", "Sin(")
            .Replace("'", "Cos(")
            .Replace(";", "Tan(")
            .Replace("\"", "Asin(")
            .Replace("$", "Acos(")
            .Replace("%", "Atan(")
            .Replace(PISymbol, "3.1415926535897931")
            .Replace("^", "**")
            .Replace("ñ", "Log10(")
            .Replace("[", "Ln(")
            , usingDegrees ? NCalc.EvaluateOptions.UseDegrees : NCalc.EvaluateOptions.None, DefaultCulture);

        object eR = null;

        try { eR = e.Evaluate(); }
        catch(System.Exception exception)
        {
            Debug.LogError("[NCalc Error] " + exception);

            latestExpression = inputFieldContent;
            latestExpressionDisplay.text = inputFieldDisplay.text;
            history.Add(inputFieldDisplay.text + "_equals" + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.ErrorColor)}>{errorMessage}</color>");

            inputFieldContent = errorMessage;
            inputFieldDisplay.text = GetInputFieldContentFormatted();

            return;
        }

        latestExpression = inputFieldContent;
        latestExpressionDisplay.text = inputFieldDisplay.text;
        history.Add(inputFieldDisplay.text + "_equals" + eR.ToString());

        inputFieldContent = eR.ToString();
        inputFieldDisplay.text = GetInputFieldContentFormatted();

        latestResult = eR.ToString();
    }
    bool ContainsOperator(string toCheck)
    {
        return toCheck.Contains("+") || toCheck.Contains("-") || toCheck.Contains("*") || toCheck.Contains("/");
    }

    public void RefreshHistoryDisplay()
    {
        string hContent = string.Empty;

        foreach(string me in history)
            hContent += me + "\n\n";

        historyDisplay.text = hContent;
    }
    public void DeleteHistory()
    {
        history.Clear();
        historyDisplay.text = string.Empty;
    }

    public void ClearCalculator()
    {
        latestExpression = string.Empty;
        inputFieldContent = string.Empty;

        latestExpressionDisplay.text = string.Empty;
        inputFieldDisplay.text = string.Empty;
    }

    public void ToggleHistorySlab()
    {
        historySlabAnimator.SetTrigger(historyOpen ? "Close" : "Open");
        historyOpen = !historyOpen;
    }
    public void ToggleAdvancedKeyboardSlab()
    {
        advancedOpen = !advancedOpen;
        advancedKeyboardSlabAnimator.SetBool("Open", advancedOpen);
    }
    public void ToggleMute()
    {
        AudioListener.volume = AudioListener.volume > 0 ? 0f : 1f;
        muteIconObject.SetActive(AudioListener.volume <= 0);
    }

    public void ToggleDegreesAndRadians()
    {
        usingDegrees = !usingDegrees;
        angleUnitToggle.SetBool("Degrees", usingDegrees);
    }
    public void SetUsingDegrees()
    {
        usingDegrees = true;
        angleUnitToggle.SetBool("Degrees", usingDegrees);
    }
    public void SetUsingRadians()
    {
        usingDegrees = false;
        angleUnitToggle.SetBool("Degrees", usingDegrees);
    }

    public void OpenGitHub() => Application.OpenURL("https://github.com/IDGG73/Calculadora");

    string GetInputFieldContentFormatted()
    {
        return inputFieldContent.Replace("+", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> + </color>")
            .Replace("-", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> - </color>")
            .Replace("*", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> × </color>")
            .Replace("÷", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> ÷ </color>")
            .Replace("~", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> < </color>")
            .Replace(":", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> > </color>")
            .Replace("(", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>(</color>")
            .Replace(")", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>)</color>")
            .Replace("\\", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{SquareRootSymbol}(</color>")
            .Replace("_", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin(</color>")
            .Replace("'", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos(</color>")
            .Replace(";", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Tan(</color>")
            .Replace("\"", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin<sup>-1</sup>(</color>")
            .Replace("$", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos<sup>-1</sup>(</color>")
            .Replace("%", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Tan<sup>-1</sup>(</color>")
            .Replace(PISymbol, $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{PISymbol}</color>")
            .Replace("^", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>^</color>")
            .Replace("ñ", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Log(</color>")
            .Replace("[", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Ln(</color>")
            .Replace(errorMessage, $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.ErrorColor)}>{errorMessage}</color>");
    }
}
