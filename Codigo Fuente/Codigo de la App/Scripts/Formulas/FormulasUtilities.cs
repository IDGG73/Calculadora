using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Formulas
{
    None = 0,
    Quadratic = 1,
    OppositeLength = 2,
    AdjacentLength = 3,
    HypotenuseLength = 4,
    CosAfterOpposite = 5,
    SinAfterAdjacent = 6,
    TanAfterOppositeAndAdjacent = 7,
    Similarity = 8,
    PythagorasHypotenuse = 9,
    PythagorasSide = 10
}

[System.Serializable]
public class FormulaRules
{
    public Formulas formula;
    public GameObject formulaPreviewObject;
    public string[] arguments;
}

public class FormulasUtilities : MonoBehaviour
{
    [SerializeField] Animator formulasMenuAnimator;

    [Foldout("Usages")]
    [SerializeField] Formulas selectedFormula;
    [Space]
    [SerializeField] FormulaArgumentBinder[] arguments;
    [SerializeField] FormulaRules[] formulasRules;

    public bool ArgumentsMenuIsOpen { get; private set; }
    FormulaArgumentBinder currentSelectedArgumentBinder;
    public FormulaArgumentBinder CurrentSelectedArgumentBinder { get { return currentSelectedArgumentBinder; } set { if (CurrentSelectedArgumentBinder != null && currentSelectedArgumentBinder != value) currentSelectedArgumentBinder.DeselectBinder(); currentSelectedArgumentBinder = value; } }
    public FormulaRules[] FormulasRules { get { return formulasRules; } }

    public static FormulasUtilities current;

    private void Awake() => current = this;

    #region Data
    public void SelectFormula(int formulaIndex)
    {
        selectedFormula = (Formulas)formulaIndex;
        ClearArgs();

        OpenArgumentsMenu();
    }

    public void ClearArgs()
    {
        for (int i = 0; i < arguments.Length; i++)
            arguments[i].ClearArgument();
    }
    public string GetArgumentNameByIndex(Formulas formula, int argIndex)
    {
        for (int i = 0; i < formulasRules.Length; i++)
        {
            if (formulasRules[i].formula == formula)
                if (formulasRules[i].arguments.IsInsideArrayBounds(argIndex))
                    return formulasRules[i].arguments[argIndex];
                else
                    break;
        }

        return string.Empty;
    }
    public int GetFormulaArgumentsCount(Formulas formula)
    {
        for (int i = 0; i < formulasRules.Length; i++)
        {
            if (formulasRules[i].formula == formula)
                return FormulasRules[i].arguments.Length;
        }

        return 0;
    }

    public void WriteCharacter(string toWrite) => CurrentSelectedArgumentBinder.WriteCharacter(toWrite);
    public void Backspace() => CurrentSelectedArgumentBinder.Backspace();

    public void Calculate()
    {
        int formArgCount = GetFormulaArgumentsCount(selectedFormula);
        List<FormulaArgumentBinder> neededArgs = new List<FormulaArgumentBinder>();

        for(int i = 0; i < formArgCount; i++)
            neededArgs.Add(arguments[i]);

        try { Calculator.current.CalculateFormula(selectedFormula, neededArgs); }
        catch (System.Exception e) { throw e; }

        ArgumentsMenuIsOpen = false;
        formulasMenuAnimator.Play("Formulas Evaluate");
    }
    #endregion

    public void OpenArgumentsMenu()
    {
        ArgumentsMenuIsOpen = true;
        formulasMenuAnimator.Play("Formulas To Args");

        arguments[0].SelectBinder();

        foreach (FormulaRules fr in formulasRules)
            fr.formulaPreviewObject.SetActive(false);

        foreach (FormulaArgumentBinder fab in arguments)
            fab.gameObject.SetActive(false);

        for (int i = 0; i < formulasRules.Length; i++)
        {
            if (formulasRules[i].formula == selectedFormula)
                FormulasRules[i].formulaPreviewObject.SetActive(true);
        }

        for (int i = 0; i < GetFormulaArgumentsCount(selectedFormula); i++)
        {
            arguments[i].gameObject.SetActive(true);
            arguments[i].SetArgumentName(GetArgumentNameByIndex(selectedFormula, i));
        }
    }
    public void CloseArgumentsMenu()
    {
        ArgumentsMenuIsOpen = false;
        formulasMenuAnimator.Play("Args To Formulas");
    }
}
