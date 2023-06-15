using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class TextStyleBinder : MonoBehaviour
{
    enum TextType { ButtonText, DisplayText, OperatorText, FunctionText, ExpressionText }

    [SerializeField] TextType type;

    TextMeshProUGUI text;

    private void Update() => RefreshTextStyle();

    void GetTextComponent() => text = GetComponent<TextMeshProUGUI>();
    public void RefreshTextStyle()
    {
        if (!text)
            GetTextComponent();

        if (SkinManager.current == null)
            return;

        switch (type)
        {
            case TextType.ButtonText:
                #region Font
                if (text.font != SkinManager.current.ButtonsFontAsset)
                    text.font = SkinManager.current.ButtonsFontAsset;
                #endregion
                #region Font Color
                if (text.color != SkinManager.current.ButtonsTextColor)
                    text.color = SkinManager.current.ButtonsTextColor;
                #endregion
                #region Font Size (Min/Max)
                if (text.fontSizeMin != SkinManager.current.ButtonsMinimumTextSize)
                    text.fontSizeMin = SkinManager.current.ButtonsMinimumTextSize;

                if (text.fontSizeMax != SkinManager.current.ButtonsMaximumTextSize)
                    text.fontSizeMax = SkinManager.current.ButtonsMaximumTextSize;
                #endregion
                break;

            case TextType.DisplayText:
                #region Font
                if (text.font != SkinManager.current.DisplayFontAsset)
                    text.font = SkinManager.current.DisplayFontAsset;
                #endregion
                #region Font Color
                if (text.color != SkinManager.current.DisplayTextColor)
                    text.color = SkinManager.current.DisplayTextColor;
                #endregion
                #region Font Size (Min/Max)
                if (text.fontSizeMin != SkinManager.current.DisplayMinimumTextSize)
                    text.fontSizeMin = SkinManager.current.DisplayMinimumTextSize;

                if (text.fontSizeMax != SkinManager.current.DisplayMaximumTextSize)
                    text.fontSizeMax = SkinManager.current.DisplayMaximumTextSize;
                #endregion
                break;

            case TextType.OperatorText:
                #region Font
                if (text.font != SkinManager.current.ButtonsFontAsset)
                    text.font = SkinManager.current.ButtonsFontAsset;
                #endregion
                #region Font Color
                if (text.color != SkinManager.current.OperatorsColor)
                    text.color = SkinManager.current.OperatorsColor;
                #endregion
                #region Font Size (Min/Max)
                if (text.fontSizeMin != SkinManager.current.ButtonsMinimumTextSize)
                    text.fontSizeMin = SkinManager.current.ButtonsMinimumTextSize;

                if (text.fontSizeMax != SkinManager.current.ButtonsMaximumTextSize)
                    text.fontSizeMax = SkinManager.current.ButtonsMaximumTextSize;
                #endregion
                break;

            case TextType.FunctionText:
                #region Font
                if (text.font != SkinManager.current.ButtonsFontAsset)
                    text.font = SkinManager.current.ButtonsFontAsset;
                #endregion
                #region Font Color
                if (text.color != SkinManager.current.FunctionsColor)
                    text.color = SkinManager.current.FunctionsColor;
                #endregion
                #region Font Size (Min/Max)
                if (text.fontSizeMin != SkinManager.current.ButtonsMinimumTextSize)
                    text.fontSizeMin = SkinManager.current.ButtonsMinimumTextSize;

                if (text.fontSizeMax != SkinManager.current.ButtonsMaximumTextSize)
                    text.fontSizeMax = SkinManager.current.ButtonsMaximumTextSize;
                #endregion
                break;

            case TextType.ExpressionText:
                #region Font
                if (text.font != SkinManager.current.DisplayFontAsset)
                    text.font = SkinManager.current.DisplayFontAsset;
                #endregion
                #region Font Color
                if (text.color != SkinManager.current.DisplayTextColor)
                    text.color = SkinManager.current.DisplayTextColor;
                #endregion
                #region Font Size (Min/Max)
                if (text.fontSizeMin != SkinManager.current.ExpressionMinimumTextSize)
                    text.fontSizeMin = SkinManager.current.ExpressionMinimumTextSize;

                if (text.fontSizeMax != SkinManager.current.ExpressionMaximumTextSize)
                    text.fontSizeMax = SkinManager.current.ExpressionMaximumTextSize;
                #endregion
                break;
        }
    }
}
