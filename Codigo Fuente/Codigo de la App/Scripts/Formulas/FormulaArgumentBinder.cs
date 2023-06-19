using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FormulaArgumentBinder : MonoBehaviour
{
    [SerializeField] string argument;
    [Space]
    [SerializeField] TextMeshProUGUI argumentNameDisplay;
    [SerializeField] TextMeshProUGUI argumentDisplay;
    [SerializeField] GameObject selectionHighlight;

    public bool IsSelected { get; private set; }
    public string Argument { get { return argument; } set { argument = value; } }
    public string ArgumentName { get; private set; }

    public void SelectBinder()
    {
        FormulasUtilities.current.CurrentSelectedArgumentBinder = this;

        IsSelected = true;
        selectionHighlight.SetActive(true);
    }
    public void DeselectBinder()
    {
        IsSelected = false;
        selectionHighlight.SetActive(false);
    }

    public void SetArgumentName(string argName)
    {
        ArgumentName = argName;
        argumentNameDisplay.text = $"<color=#FFD966>{argName}</color> <color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>=</color>";
    }
    public void WriteCharacter(string toWrite)
    {
        switch (toWrite)
        {
            case "/":
                argument = argument + "÷";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> ÷ </color>";
                break;
            case "*":
                argument = argument + "*";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> × </color>";
                break;
            case "+":
                argument = argument + "+";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> + </color>";
                break;
            case "-":
                argument = argument + "-";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> - </color>";
                break;
            case "(":
                argument = argument + "(";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>(</color>";
                break;
            case ")":
                argument = argument + ")";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>)</color>";
                break;
            case "Sin(":
                argument = argument + ":";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin(</color>";
                break;
            case "Cos(":
                argument = argument + "'";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos(</color>";
                break;
            case "Tan(":
                argument = argument + ";";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Tan(</color>";
                break;
            case "Asin(":
                argument = argument + "\"";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin<sup>-1</sup>(</color>";
                break;
            case "Acos(":
                argument = argument + "$";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos<sup>-1</sup>(</color>";
                break;
            case "Atan(":
                argument = argument + "%";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Tan<sup>-1</sup>(</color>";
                break;
            case "Sqrt(":
                argument = argument + "\\";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>\\(</color>";
                break;
            case "Pi":
                argument = argument + Calculator.current.PISymbol;
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{Calculator.current.PISymbol}</color>";
                break;
            case "^":
                argument = argument + "**";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>^</color>";
                break;
            case "Log(":
                argument = argument + "ñ";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Log(</color>";
                break;
            case "Ln(":
                argument = argument + "[";
                argumentDisplay.text = argumentDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Ln(</color>";
                break;

            default:
                argument = argument + toWrite;
                argumentDisplay.text = argumentDisplay.text + toWrite;
                break;
        }
    }
    public void Backspace()
    {
        if (argumentDisplay.text.Contains(Calculator.current.errorMessage))
        {
            argument = string.Empty;
            argumentDisplay.text = string.Empty;

            return;
        }

        if (argument.Length <= 0)
            return;

        argument = argument.Remove(argument.Length - 1, 1);
        argumentDisplay.text = GetInputFieldContentFormatted();
    }
    public void ClearArgument()
    {
        argument = string.Empty;
        argumentDisplay.text = string.Empty;
    }

    string GetInputFieldContentFormatted()
    {
        return argument.Replace("+", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> + </color>")
            .Replace("-", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> - </color>")
            .Replace("*", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> × </color>")
            .Replace("÷", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> ÷ </color>")
            .Replace("~", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> < </color>")
            .Replace("(", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>(</color>")
            .Replace(")", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>)</color>")
            .Replace("\\", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{Calculator.SquareRootSymbol}(</color>")
            .Replace(":", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin(</color>")
            .Replace("'", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos(</color>")
            .Replace(";", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Tan(</color>")
            .Replace("\"", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin<sup>-1</sup>(</color>")
            .Replace("$", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos<sup>-1</sup>(</color>")
            .Replace("%", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Tan<sup>-1</sup>(</color>")
            .Replace(Calculator.current.PISymbol, $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{Calculator.current.PISymbol}</color>")
            .Replace("^", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>^</color>")
            .Replace("ñ", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Log(</color>")
            .Replace("[", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Ln(</color>")
            .Replace(Calculator.current.errorMessage, $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.ErrorColor)}>{Calculator.current.errorMessage}</color>");
    }
}
