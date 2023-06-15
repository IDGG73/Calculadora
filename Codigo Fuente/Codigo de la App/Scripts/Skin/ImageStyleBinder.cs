using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class ImageStyleBinder : MonoBehaviour
{
    enum TextType { Background, KeyboardSlab, MiddleSizedButton, SmallSizedButton, Advanced }

    [SerializeField] TextType type;
    [Space]
    [SerializeField] bool overrideGradient;
    [SerializeField, ConditionalHide(nameof(overrideGradient), true)] Gradient color;
    [Space]
    [SerializeField] bool overrideRoundness;
    [SerializeField, ConditionalHide(nameof(overrideRoundness), true)] float roundness = 1f;

    Image target;
    UIGradient gradient;

    private void Update() => RefreshImageStyle();

    void GetImageComponent() => target = GetComponent<Image>();
    void GetGradientComponent() => gradient = GetComponent<UIGradient>();

    public void RefreshImageStyle()
    {
        if (!target)
            GetImageComponent();

        if (!gradient)
            GetGradientComponent();

        if (SkinManager.current == null)
            return;

        switch (type)
        {
            case TextType.Background:
                SetRoundness();
                #region Gradient
                if (gradient && !overrideGradient)
                {
                    if (gradient.m_color1 != SkinManager.current.BackgroundGradient.Evaluate(0f))
                    {
                        gradient.m_color1 = SkinManager.current.BackgroundGradient.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != SkinManager.current.BackgroundGradient.Evaluate(1f))
                    {
                        gradient.m_color2 = SkinManager.current.BackgroundGradient.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                else if(gradient && overrideGradient)
                {
                    if (gradient.m_color1 != color.Evaluate(0f))
                    {
                        gradient.m_color1 = color.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != color.Evaluate(1f))
                    {
                        gradient.m_color2 = color.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                #endregion
                break;

            case TextType.KeyboardSlab:
                SetRoundness();
                #region Gradient
                if (gradient && !overrideGradient)
                {
                    if (gradient.m_color1 != SkinManager.current.KeyboardSlabGradient.Evaluate(0f))
                    {
                        gradient.m_color1 = SkinManager.current.KeyboardSlabGradient.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != SkinManager.current.KeyboardSlabGradient.Evaluate(1f))
                    {
                        gradient.m_color2 = SkinManager.current.KeyboardSlabGradient.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                else if (gradient && overrideGradient)
                {
                    if (gradient.m_color1 != color.Evaluate(0f))
                    {
                        gradient.m_color1 = color.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != color.Evaluate(1f))
                    {
                        gradient.m_color2 = color.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                #endregion
                break;

            case TextType.MiddleSizedButton:
                SetRoundness();
                #region Gradient
                if (gradient && !overrideGradient)
                {
                    if (gradient.m_color1 != SkinManager.current.ButtonGradientColor.Evaluate(0f))
                    {
                        gradient.m_color1 = SkinManager.current.ButtonGradientColor.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != SkinManager.current.ButtonGradientColor.Evaluate(1f))
                    {
                        gradient.m_color2 = SkinManager.current.ButtonGradientColor.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                else if (gradient && overrideGradient)
                {
                    if (gradient.m_color1 != color.Evaluate(0f))
                    {
                        gradient.m_color1 = color.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != color.Evaluate(1f))
                    {
                        gradient.m_color2 = color.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                #endregion
                break;

            case TextType.SmallSizedButton:
                SetRoundness();
                #region Gradient
                if (gradient && !overrideGradient)
                {
                    if (gradient.m_color1 != SkinManager.current.ButtonGradientColor.Evaluate(0f))
                    {
                        gradient.m_color1 = SkinManager.current.ButtonGradientColor.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != SkinManager.current.ButtonGradientColor.Evaluate(1f))
                    {
                        gradient.m_color2 = SkinManager.current.ButtonGradientColor.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                else if (gradient && overrideGradient)
                {
                    if (gradient.m_color1 != color.Evaluate(0f))
                    {
                        gradient.m_color1 = color.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != color.Evaluate(1f))
                    {
                        gradient.m_color2 = color.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                #endregion
                break;

            case TextType.Advanced:
                SetRoundness();
                #region Gradient
                if (gradient && !overrideGradient)
                {
                    if (gradient.m_color1 != SkinManager.current.AdvancedGradientColor.Evaluate(0f))
                    {
                        gradient.m_color1 = SkinManager.current.AdvancedGradientColor.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != SkinManager.current.AdvancedGradientColor.Evaluate(1f))
                    {
                        gradient.m_color2 = SkinManager.current.AdvancedGradientColor.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                else if (gradient && overrideGradient)
                {
                    if (gradient.m_color1 != color.Evaluate(0f))
                    {
                        gradient.m_color1 = color.Evaluate(0f);
                        gradient.RequestRefresh();
                    }

                    if (gradient.m_color2 != color.Evaluate(1f))
                    {
                        gradient.m_color2 = color.Evaluate(1f);
                        gradient.RequestRefresh();
                    }
                }
                #endregion
                break;
        }

        void SetRoundness()
        {
            switch (type)
            {
                case TextType.SmallSizedButton:
                    if (!overrideRoundness)
                    {
                        if (target.pixelsPerUnitMultiplier != SkinManager.current.SmallSizedButtonRoundness)
                            target.pixelsPerUnitMultiplier = SkinManager.current.SmallSizedButtonRoundness;
                    }
                    else
                        target.pixelsPerUnitMultiplier = roundness;
                    break;

                case TextType.Advanced:
                    if (!overrideRoundness)
                    {
                        if (target.pixelsPerUnitMultiplier != SkinManager.current.SmallSizedButtonRoundness)
                            target.pixelsPerUnitMultiplier = SkinManager.current.SmallSizedButtonRoundness;
                    }
                    else
                        target.pixelsPerUnitMultiplier = roundness;
                    break;

                default:
                    if (!overrideRoundness)
                    {
                        if (target.pixelsPerUnitMultiplier != SkinManager.current.MiddleSizedButtonRoundness)
                            target.pixelsPerUnitMultiplier = SkinManager.current.MiddleSizedButtonRoundness;
                    }
                    else
                        target.pixelsPerUnitMultiplier = roundness;
                    break;
            }
        }
    }
}
