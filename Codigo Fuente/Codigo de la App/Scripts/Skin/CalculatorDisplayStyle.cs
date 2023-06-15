using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class CalculatorDisplayStyle : MonoBehaviour
{
    string latestContent;
    TextMeshProUGUI text;

    void GetTextComponent() => text = GetComponent<TextMeshProUGUI>();
    private void Update() => RefreshTextStyle();

    void RefreshTextStyle()
    {
        if (!text)
            GetTextComponent();

        if(text.text != latestContent)
        {
            text.text = text.text.Replace("_div", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> ÷ </color>");
            text.text = text.text.Replace("_plus", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> + </color>");
            text.text = text.text.Replace("_minus", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> - </color>");
            text.text = text.text.Replace("_mult", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> × </color>");
            text.text = text.text.Replace("_lower", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> < </color>");
            text.text = text.text.Replace("_great", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> > </color>");

            text.text = text.text.Replace("\\", Calculator.SquareRootSymbol);

            /*
            text.text = text.text.Replace("(", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>(</color>");
            text.text = text.text.Replace(")", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>)</color>");*/

            text.text = text.text.Replace("True", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}> Verdadero </color>");
            text.text = text.text.Replace("False", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}> Falso </color>");

            text.text = text.text.Replace("_equals", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}> = </color>");

            latestContent = text.text;
        }
    }
}
