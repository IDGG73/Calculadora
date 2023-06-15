using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField = "";
    public bool HideInInspector = false;
    public bool Invert = false;

    public ConditionalHideAttribute(string conditionalSourceField)
    {
        ConditionalSourceField = conditionalSourceField;
        HideInInspector = false;
        Invert = false;
    }

    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
    {
        ConditionalSourceField = conditionalSourceField;
        HideInInspector = hideInInspector;
        Invert = false;
    }

    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector, bool invert)
    {
        ConditionalSourceField = conditionalSourceField;
        HideInInspector = hideInInspector;
        Invert = invert;
    }
}