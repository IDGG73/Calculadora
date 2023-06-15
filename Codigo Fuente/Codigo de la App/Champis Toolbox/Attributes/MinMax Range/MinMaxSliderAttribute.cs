using UnityEngine;

public class MinMaxSliderAttribute : PropertyAttribute
{

    public float min;
    public float max;
    public bool showTooltip;

    public MinMaxSliderAttribute(float min, float max, bool showTooltip = true)
    {
        this.min = min;
        this.max = max;
        this.showTooltip = showTooltip;
    }
}