using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public enum MathSymbol { Add, Subtract, Multiply, Divide }

public class Calculator : MonoBehaviour
{
    public class CalculatorResult
    {
        public bool success;
        public string result;
        public double resultAsDouble;

        public CalculatorResult(bool success, string result, double resultAsDouble)
        {
            this.success = success;
            this.result = result;
            this.resultAsDouble = resultAsDouble;
        }
    }

    public struct Fraction
    {
        public int Upper { get; private set; }
        public int Lower { get; private set; }

        public Fraction(int n, int d)
        {
            Upper = n;
            Lower = d;
        }

        public override string ToString() => $"{Upper}/{Lower}";
    }

    public class SimilarityResult
    {
        public bool success;

        public string originalSideA;
        public string originalSideB;
        public string originalSideC;

        public string copySideA;
        public string copySideB;
        public string copySideC;

        public string originalFactor;
        public string copyFactor;

        public SimilarityResult(bool success, string a, string b, string c, string aC, string bC, string cC, string ogFactor, string copFactor)
        {
            this.success = success;

            this.originalSideA = a;
            this.originalSideB = b;
            this.originalSideC = c;

            this.copySideA = aC;
            this.copySideB = bC;
            this.copySideC = cC;

            this.originalFactor = ogFactor;
            this.copyFactor = copFactor;
        }
    }

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
    [Space]
    [SerializeField] GameObject equalsIconObject;
    [SerializeField] GameObject nextIconObject;
    [SerializeField] GameObject approximateIconObject;

    [Foldout("Settings")]
    public string errorMessage = "Error de Sintaxis";
    public string squareRootSymbol;
    public string PISymbol;
    public string ApproximateSymbol;
    [SerializeField, Min(float.Epsilon)] float resultLerpSpeed = 1f;
    [SerializeField] float decimalToFractionAccuracy = 0.01f;

    [Foldout("Debug", readOnly = true)]
    [SerializeField] bool historyOpen;
    [SerializeField] bool advancedOpen;
    [SerializeField] bool usingDegrees = true;
    [Space]
    [SerializeField] string latestResult;
    [SerializeField] bool displayingSimilarity;

    public static string SquareRootSymbol { get { if (current) return current.squareRootSymbol; else return "?"; } }
    public static System.Globalization.CultureInfo DefaultCulture;

    public static Calculator instance;
    public static Calculator current { get { if (instance == null) instance = GameObject.FindObjectOfType<Calculator>(); return instance; } }

    Coroutine lerpResultCoroutine;

    private void Awake()
    {
        instance = this;

        //Limitamos a la aplicacion para que solamente genere 60 fotogramas por segundo
        //Esto lo hacemos para que el dispositivo no se fuerce demasiado para alcanzar la mayor cantidad de fotogramas posible.
        //Dejar que el sistema llegue al limite significa que producira mas calor y gastara mas bateria.
        //Usamos 60 como limite porque asi la aplicacion funciona con gran fluidez. Disminuir este numero hara que las animaciones se vean mas lentas y cortadas.
        Application.targetFrameRate = 60;

        //En algunas culturas se utiliza la coma para los decimales, pero nuestra calculadora solo tiene soporte para punto.
        //Dependiendo de la ubicacion geografica del usuario, Unity ajusta varias reglas de formato para que coincidan con las reglas de la cultura local.
        //Estas lineas fuerzan al sistema para que siempre utilice el punto decimal, sin importar la cultura del usuario.
        DefaultCulture = new System.Globalization.CultureInfo("en-US");
        DefaultCulture.NumberFormat.NumberDecimalSeparator = ".";

        Thread.CurrentThread.CurrentCulture = DefaultCulture;
    }

    public void WriteCharacter(string toWrite)
    {
        if (FormulasUtilities.current.ArgumentsMenuIsOpen)
        {
            FormulasUtilities.current.WriteCharacter(toWrite);
            return;
        }

        if (inputFieldDisplay.text.Contains(errorMessage) || displayingSimilarity)
        {
            inputFieldContent = string.Empty;
            inputFieldDisplay.text = string.Empty;

            if (displayingSimilarity)
            {
                latestExpression = string.Empty;
                latestExpressionDisplay.text = string.Empty;

                displayingSimilarity = false;
            }
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
                inputFieldContent = inputFieldContent + "NOT IMPLEMENTED";
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
                inputFieldContent = inputFieldContent + ":";
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin(</color>";
                break;
            case "Cos(":
                inputFieldContent = inputFieldContent + "ö";
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
                inputFieldDisplay.text = inputFieldDisplay.text + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{SquareRootSymbol}(</color>";
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
        if (FormulasUtilities.current.ArgumentsMenuIsOpen)
        {
            FormulasUtilities.current.Backspace();
            return;
        }

        if (inputFieldDisplay.text.Contains(errorMessage) || displayingSimilarity)
        {
            inputFieldContent = string.Empty;
            inputFieldDisplay.text = string.Empty;

            if (displayingSimilarity)
            {
                latestExpression = string.Empty;
                latestExpressionDisplay.text = string.Empty;

                displayingSimilarity = false;
            }

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
        if (FormulasUtilities.current.ArgumentsMenuIsOpen)
        {
            FormulasUtilities.current.Calculate();
            return;
        }

        if (string.IsNullOrEmpty(inputFieldContent))
            return;

        CalculatorResult cR = Evaluate(inputFieldContent);

        if (!cR.success)
        {
            Debug.LogError("[NCalc Error] " + cR.result);

            latestExpression = inputFieldContent;
            latestExpressionDisplay.text = inputFieldDisplay.text;
            history.Add(inputFieldDisplay.text + "_equals" + $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.ErrorColor)}>{errorMessage}</color>");

            inputFieldContent = errorMessage;
            inputFieldDisplay.text = GetInputFieldContentFormatted();

            return;
        }

        latestExpression = inputFieldContent;
        latestExpressionDisplay.text = inputFieldDisplay.text;
        history.Add(inputFieldDisplay.text + "_equals" + cR.result);

        inputFieldContent = cR.result;
        inputFieldDisplay.text = GetInputFieldContentFormatted();

        latestResult = cR.result;
    }
    public void CalculateFormula(Formulas formula, List<FormulaArgumentBinder> args)
    {
        if (args == null || args.Count <= 0)
        {
            Champis.UI.ToastNotification.Show("No se han dado parámetros.");
            throw new System.Exception("No Arguments declared for formula. Aborting...");
        }

        List<CalculatorResult> argsResults = new List<CalculatorResult>();

        #region Empty Arguments
        if (formula != Formulas.Similarity)
            foreach (FormulaArgumentBinder arg in args)
                if (string.IsNullOrEmpty(arg.Argument))
                {
                    Champis.UI.ToastNotification.Show("Faltan parámetros.");
                    throw new System.Exception("One of the requiered arguments was null or empty. Aborting...");
                }
        #endregion

        #region Arguments Count
        for (int i = 0; i < FormulasUtilities.current.FormulasRules.Length; i++)
        {
            if (formula == FormulasUtilities.current.FormulasRules[i].formula)
            {
                if (args.Count < FormulasUtilities.current.FormulasRules[i].arguments.Length)
                {
                    Champis.UI.ToastNotification.Show("Faltan parámetros.");
                    throw new System.Exception("Missing Arguments for formula. Aborting...");
                }
                else
                    break;
            }
        }
        #endregion

        #region Arguments Evaluation
        for (int i = 0; i < args.Count; i++)
        {
            if (!string.IsNullOrEmpty(args[i].Argument))
            {
                CalculatorResult cR = Evaluate(args[i].Argument);

                if (!cR.success)
                {
                    Champis.UI.ToastNotification.Show($"Error de Sintaxis en el parámetro {args[i].ArgumentName}.");
                    throw new System.Exception($"'Argument {i}' evaluation failed");
                }
                else
                    argsResults.Add(cR);
            }
            else
            {
                if (formula == Formulas.Similarity)
                    argsResults.Add(new CalculatorResult(true, string.Empty, 0));
            }
        }
        #endregion

        #region Evaluate Formula
        string result = string.Empty;
        string formattedExpression = string.Empty;

        switch (formula)
        {
            case Formulas.Quadratic:
                result = QuadraticFormula(argsResults[0].resultAsDouble, argsResults[1].resultAsDouble, argsResults[2].resultAsDouble);
                formattedExpression = $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>Fórmula General(</color><color=#FFD966>a</color>_equals{argsResults[0].result}, </color><color=#FFD966>b</color>_equals{argsResults[1].result}, </color><color=#FFD966>c</color>_equals{argsResults[2].result}<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>)</color>";
                history.Add(formattedExpression + "_equals" + "\n" + result);
                break;

            case Formulas.OppositeLength:
                result = Evaluate($"{argsResults[0].resultAsDouble}*Tan({argsResults[1].resultAsDouble})").result;
                formattedExpression = $"<color=#FFD966>{argsResults[0].result}</color><color=#E67373> · Tan(</color><color=#FFD966>{argsResults[1].result}</color><color=#E67373>)</color><color=#4CFFC4> {ApproximateSymbol} </color>";
                history.Add(formattedExpression + result);
                break;

            case Formulas.AdjacentLength:
                result = Evaluate($"{argsResults[0].resultAsDouble}*Cos({argsResults[1].resultAsDouble})").result;
                formattedExpression = $"<color=#FFD966>{argsResults[0].result}</color><color=#E67373> · Cos(</color><color=#FFD966>{argsResults[1].result}</color><color=#E67373>)</color><color=#4CFFC4> {ApproximateSymbol} </color>";
                history.Add(formattedExpression + result);
                break;

            case Formulas.HypotenuseLength:
                result = Evaluate($"{argsResults[0].resultAsDouble}/Sin({argsResults[1].resultAsDouble})").result;
                formattedExpression = $"<color=#FFD966>{argsResults[0].result}</color><color=#E67373> ÷ Sin(</color><color=#FFD966>{argsResults[1].result}</color><color=#E67373>)</color><color=#4CFFC4> {ApproximateSymbol} </color>";
                history.Add(formattedExpression + result);
                break;

            case Formulas.CosAfterOpposite:
                result = Evaluate($"{argsResults[0].resultAsDouble}/{argsResults[1].resultAsDouble}").result;
                formattedExpression = $"<color=#FFD966>{argsResults[0].result}</color><color=#E67373> ÷ </color><color=#FFD966>{argsResults[1].result}</color><color=#4CFFC4> = </color>";
                history.Add(formattedExpression + result);
                break;

            case Formulas.SinAfterAdjacent:
                result = Evaluate($"{argsResults[0].resultAsDouble}/{argsResults[1].resultAsDouble}").result;
                formattedExpression = $"<color=#FFD966>{argsResults[0].result}</color><color=#E67373> ÷ </color><color=#FFD966>{argsResults[1].result}</color><color=#4CFFC4> = </color>";
                history.Add(formattedExpression + result);
                break;

            case Formulas.TanAfterOppositeAndAdjacent:
                result = Evaluate($"{argsResults[0].resultAsDouble}/{argsResults[1].resultAsDouble}").result;
                formattedExpression = $"<color=#FFD966>{argsResults[0].result}</color><color=#E67373> ÷ </color><color=#FFD966>{argsResults[1].result}</color><color=#4CFFC4> = </color>";
                history.Add(formattedExpression + result);
                break;

            case Formulas.Similarity:
                SimilarityResult sR = SimilarityFormula(argsResults[0].result, argsResults[1].result, argsResults[2].result, argsResults[3].result, argsResults[4].result, argsResults[5].result);
                result = FormatSimilarityResult(sR);

                formattedExpression = "Semejanza";
                history.Add($"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>Semejanza:</color> {result}");

                displayingSimilarity = true;
                break;

            case Formulas.PythagorasHypotenuse:
                result = Evaluate($"Sqrt({argsResults[0].resultAsDouble}^2+{argsResults[1].resultAsDouble}^2)").result;
                formattedExpression = $"<color=#E67373>{SquareRootSymbol}(</color><color=#FFD966>{argsResults[0].result}</color><sup>2</sup><color=#E67373> + </color><color=#FFD966>{argsResults[1].result}</color><sup>2</sup><color=#E67373>)</color><color=#4CFFC4> = </color>";
                history.Add(formattedExpression + result);
                break;

            case Formulas.PythagorasSide:
                result = Evaluate($"Sqrt({argsResults[0].resultAsDouble}^2-{argsResults[1].resultAsDouble}^2)").result;
                formattedExpression = $"<color=#E67373>{SquareRootSymbol}(</color><color=#FFD966>{argsResults[0].result}</color><sup>2</sup><color=#E67373> + </color><color=#FFD966>{argsResults[1].result}</color><sup>2</sup><color=#E67373>)</color><color=#4CFFC4> = </color>";
                history.Add(formattedExpression + result);
                break;
        }

        latestExpression = formattedExpression;
        latestExpressionDisplay.text = formattedExpression;

        inputFieldContent = result;
        inputFieldDisplay.text = GetInputFieldContentFormatted();

        if (!displayingSimilarity)
            latestResult = result;
        #endregion

        #region Embedded Voids
        string QuadraticFormula(double a, double b, double c)
        {
            double sqrtpart = b * b - 4 * a * c;
            double x, x1, x2, img;

            if (sqrtpart > 0)
            {
                x1 = (-b + System.Math.Sqrt(sqrtpart)) / (2 * a);
                x2 = (-b - System.Math.Sqrt(sqrtpart)) / (2 * a);

                //return string.Format("{0,8:f4} ó {1,8:f4}", x1, x2);
                return $"x<sub>1</sub> <color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>=</color> {x1}" +
                    $"\nx<sub>2</sub> <color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>=</color> {x2}";
            }
            else if (sqrtpart < 0)
            {
                sqrtpart = -sqrtpart;
                x = -b / (2 * a);
                img = System.Math.Sqrt(sqrtpart) / (2 * a);

                //return string.Format(string.Format("{0,8:f4} + {1,8:f4} i | {2,8:f4} + {3,8:f4} i", x, img, x, img));

                /*
                return $"x <color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>=</color> {x}_plus{img} i" +
                    $"\nx <color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>=</color> {x}_plus{img} i";*/

                return $"x <color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>=</color> {x}_pluMin{img}<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>i</color>";
            }
            else
            {
                x = (-b + System.Math.Sqrt(sqrtpart)) / (2 * a);
                //return string.Format(string.Format("One Real Solution: {0,8:f4}", x));
                return $"x <color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>=</color> {x}";
            }
        }
        SimilarityResult SimilarityFormula(string a, string b, string c, string aC, string bC, string cC)
        {
            SimilarityResult tmpResult = new SimilarityResult(false, a, b, c, aC, bC, cC, string.Empty, string.Empty);
            Fraction fraction = new Fraction();

            CalculatorResult aR = new CalculatorResult(true, string.Empty, 0);
            CalculatorResult bR = new CalculatorResult(true, string.Empty, 0);
            CalculatorResult cR = new CalculatorResult(true, string.Empty, 0);
            CalculatorResult aCR = new CalculatorResult(true, string.Empty, 0);
            CalculatorResult bCR = new CalculatorResult(true, string.Empty, 0);
            CalculatorResult cCR = new CalculatorResult(true, string.Empty, 0);

            if (!string.IsNullOrEmpty(a))
                aR = Evaluate(a);
            if (!string.IsNullOrEmpty(b))
                bR = Evaluate(b);
            if (!string.IsNullOrEmpty(c))
                cR = Evaluate(c);
            if (!string.IsNullOrEmpty(aC))
                aCR = Evaluate(aC);
            if (!string.IsNullOrEmpty(bC))
                bCR = Evaluate(bC);
            if (!string.IsNullOrEmpty(cC))
                cCR = Evaluate(cC);

            #region Get Scale Factor As Fraction
            if (aR.resultAsDouble > 0 && aCR.resultAsDouble > 0)
                fraction = DecimalToFraction(aR.resultAsDouble / aCR.resultAsDouble, decimalToFractionAccuracy);
            else if (bR.resultAsDouble > 0 && bCR.resultAsDouble > 0)
                fraction = DecimalToFraction(bR.resultAsDouble / bCR.resultAsDouble, decimalToFractionAccuracy);
            else if (cR.resultAsDouble > 0 && cCR.resultAsDouble > 0)
                fraction = DecimalToFraction(cR.resultAsDouble / cCR.resultAsDouble, decimalToFractionAccuracy);
            else
            {
                Champis.UI.ToastNotification.Show("Faltan parámetros.\nIntroduce un lado en sus formas Original y Copia'", 5f);
                throw new System.Exception("[SimilarityFormula] Unable to get Scale Factor: Missing arguments.");
            }
            #endregion

            tmpResult.originalFactor = $"{fraction.Upper}/{fraction.Lower}";
            tmpResult.copyFactor = $"{fraction.Lower}/{fraction.Upper}";

            #region Auto-Fill Missing Arguments
            //Auto-Fill A
            try
            {
                if (aR.resultAsDouble <= 0 && aCR.resultAsDouble > 0)
                {
                    Fraction frac = DecimalToFraction(Evaluate($"{tmpResult.originalFactor}*{aCR.resultAsDouble}").resultAsDouble, decimalToFractionAccuracy);
                    tmpResult.originalSideA = frac.ToString();
                }
            }
            catch { tmpResult.originalSideA = string.Empty; }

            //Auto-Fill B
            try
            {
                if (bR.resultAsDouble <= 0 && bCR.resultAsDouble > 0)
                {
                    Fraction frac = DecimalToFraction(Evaluate($"{tmpResult.originalFactor}*{bCR.resultAsDouble}").resultAsDouble, decimalToFractionAccuracy);
                    tmpResult.originalSideB = frac.ToString();
                }
            }
            catch { tmpResult.originalSideB = string.Empty; }

            //Auto-Fill C
            try
            {
                if (cR.resultAsDouble <= 0 && cCR.resultAsDouble > 0)
                {
                    Fraction frac = DecimalToFraction(Evaluate($"{tmpResult.originalFactor}*{cCR.resultAsDouble}").resultAsDouble, decimalToFractionAccuracy);
                    tmpResult.originalSideC = frac.ToString();
                }
            }
            catch { tmpResult.originalSideC = string.Empty; }

            //Auto-Fill A'
            try
            {
                if (aCR.resultAsDouble <= 0 && aR.resultAsDouble > 0)
                {
                    Fraction frac = DecimalToFraction(Evaluate($"{tmpResult.copyFactor}*{aR.resultAsDouble}").resultAsDouble, decimalToFractionAccuracy);
                    tmpResult.copySideA = frac.ToString();
                }
            }
            catch { tmpResult.copySideA = string.Empty; }

            //Auto-Fill B'
            try
            {
                if (bCR.resultAsDouble <= 0 && bR.resultAsDouble > 0)
                {
                    Fraction frac = DecimalToFraction(Evaluate($"{tmpResult.copyFactor}*{bR.resultAsDouble}").resultAsDouble, decimalToFractionAccuracy);
                    tmpResult.copySideB = frac.ToString();
                }
            }
            catch { tmpResult.copySideB = string.Empty; }

            //Auto-Fill C'
            try
            {
                if (cCR.resultAsDouble <= 0 && cR.resultAsDouble > 0)
                {
                    Fraction frac = DecimalToFraction(Evaluate($"{tmpResult.copyFactor}*{cR.resultAsDouble}").resultAsDouble, decimalToFractionAccuracy);
                    tmpResult.copySideC = frac.ToString();
                }
            }
            catch { tmpResult.copySideC = string.Empty; }
            #endregion

            return tmpResult;
        }
        string FormatSimilarityResult(SimilarityResult sR)
        {
            string bake = string.Empty;

            if (!string.IsNullOrEmpty(sR.originalSideA) && sR.originalSideA != "0")
                bake += $"<color=#FFD966>A</color>_equals{sR.originalSideA} ";

            if (!string.IsNullOrEmpty(sR.originalSideB) && sR.originalSideB != "0")
                bake += $"<color=#FFD966>B</color>_equals{sR.originalSideB} ";

            if (!string.IsNullOrEmpty(sR.originalSideC) && sR.originalSideC != "0")
                bake += $"<color=#FFD966>C</color>_equals{sR.originalSideC}";

            bake += "\n";

            if (!string.IsNullOrEmpty(sR.copySideA) && sR.copySideA != "0")
                bake += $"<color=#FFD966>A'</color>_equals{sR.copySideA} ";

            if (!string.IsNullOrEmpty(sR.copySideB) && sR.copySideB != "0")
                bake += $"<color=#FFD966>B'</color>_equals{sR.copySideB} ";

            if (!string.IsNullOrEmpty(sR.copySideC) && sR.copySideC != "0")
                bake += $"<color=#FFD966>C'</color>_equals{sR.copySideC}";

            bake += $"\n\n<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>Factor Escala</color>_equals{sR.originalFactor}";
            bake += $"\n<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.FunctionsColor)}>Factor Escala'</color>_equals{sR.copyFactor}";

            return bake;
        }
        #endregion
    }
    public CalculatorResult Evaluate(string toEvaluate)
    {
        NCalc.Expression e = new NCalc.Expression(toEvaluate
            .Replace("÷", "/")
            .Replace("~", "<")
            .Replace("\\", "Sqrt(")
            .Replace(":", "Sin(")
            .Replace("ö", "Cos(")
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
        catch (System.Exception exception) { return new CalculatorResult(false, exception.ToString(), 0); }

        double tryParse = 0;
        double.TryParse(eR.ToString(), out tryParse);

        return new CalculatorResult(true, eR.ToString(), tryParse);
    }
    private static uint GreatestCommonDivisor(uint a, uint b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a | b;
    }
    public Fraction DecimalToFraction(double value, double accuracy)
    {
        if (accuracy <= 0.0 || accuracy >= 1.0)
            throw new System.ArgumentOutOfRangeException("accuracy", "Must be > 0 and < 1.");

        int sign = System.Math.Sign(value);

        if (sign == -1)
            value = System.Math.Abs(value);

        double maxError = sign == 0 ? accuracy : value * accuracy;

        int n = (int)System.Math.Floor(value);
        value -= n;

        if (value < maxError)
            return new Fraction(sign * n, 1);

        if (1 - maxError < value)
            return new Fraction(sign * (n + 1), 1);

        int upper_n = 1;
        int upper_d = 1;

        int lower_n = 0;
        int lower_d = 1;

        while (true)
        {
            int middle_n = lower_n + upper_n;
            int middle_d = lower_d + upper_d;

            if (middle_d * (value + maxError) < middle_n)
                Seek(ref upper_n, ref upper_d, lower_n, lower_d, (un, ud) => (lower_d + ud) * (value + maxError) < (lower_n + un));
            else if (middle_n < (value - maxError) * middle_d)
                Seek(ref lower_n, ref lower_d, upper_n, upper_d, (ln, ld) => (ln + upper_n) < (value - maxError) * (ld + upper_d));
            else
                return new Fraction((n * middle_d + middle_n) * sign, middle_d);
        }

        void Seek(ref int a, ref int b, int ainc, int binc, System.Func<int, int, bool> f)
        {
            a += ainc;
            b += binc;

            if (f(a, b))
            {
                int weight = 1;

                while (f(a, b))
                {
                    weight *= 2;
                    a += ainc * weight;
                    b += binc * weight;
                }

                while (weight > 1)
                {
                    weight /= 2;

                    int adec = ainc * weight;
                    int bdec = binc * weight;

                    if (!f(a - adec, b - bdec))
                    {
                        a -= adec;
                        b -= bdec;
                    }
                }
            }
        }
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
        if (FormulasUtilities.current.ArgumentsMenuIsOpen)
        {
            FormulasUtilities.current.CurrentSelectedArgumentBinder.ClearArgument();
            return;
        }

        latestExpression = string.Empty;
        inputFieldContent = string.Empty;

        latestExpressionDisplay.text = string.Empty;
        inputFieldDisplay.text = string.Empty;

        displayingSimilarity = false;
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

    string FormatExpression(string toFormat)
    {
        return toFormat.Replace("+", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> + </color>")
            .Replace("-", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> - </color>")
            .Replace("*", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> × </color>")
            .Replace("÷", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> ÷ </color>")
            .Replace("~", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> < </color>")
            .Replace("(", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>(</color>")
            .Replace(")", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>)</color>")
            .Replace("\\", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{SquareRootSymbol}(</color>")
            .Replace(":", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin(</color>")
            .Replace("ö", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos(</color>")
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
    string GetInputFieldContentFormatted()
    {
        return inputFieldContent.Replace("+", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> + </color>")
            .Replace("-", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> - </color>")
            .Replace("*", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> × </color>")
            .Replace("÷", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> ÷ </color>")
            .Replace("~", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}> < </color>")
            .Replace("(", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>(</color>")
            .Replace(")", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>)</color>")
            .Replace("\\", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>{SquareRootSymbol}(</color>")
            .Replace(":", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Sin(</color>")
            .Replace("ö", $"<color=#{ColorUtility.ToHtmlStringRGB(SkinManager.current.OperatorsColor)}>Cos(</color>")
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
