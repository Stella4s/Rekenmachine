using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace ItViteaRekenmachine01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*Todo:
        - Fix Ans. [Done] 
        - Fix undo button. [Done]
        - Investigate and fix issue. Using root seems to mess up how ANS works. [Done] (Forgot to add ans to StrCalc which only send R instead of RAns)
        - Fix Brackets with RPM. [Done] RPM builder now probably works with brackets and brackets within brackets.


        - Have try-catch methods for faulty input by user. (e.g. Dividing by zero, Straight up faulty syntax, etc.)

        - More complex changes.
            - Clean-up code. Limit amount of different methods + make more efficient.
                - Declared the compiler outside of compiler method, so it will only need to be called once.

            - Find a way to condense and simplify the amount of button methods. [In progress]
                    - Put all the simple Operator buttons under the same method. (*+-/)
                    - Added the , operator under numbers method + made it so if pressed whilst screen is clear it'll automatically add 0,.
                    - Put ALL operators included RPM under one method.
        */
        //Declaring Variables.
        string strCalculation, strDisplayTop, strDisplayBtm, strAns;
        bool boolAns = false, boolResult = false, boolClear = true, boolEuro = false;

        public MainWindow()
        {
            InitializeComponent();
        }
        //Buttton methods
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Numbers(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            string strButtonContent = sendButton.Content.ToString(),
                   strForCalc = strButtonContent;
            if (boolResult)
            {
                Clear();
                boolResult = false;
                if (strButtonContent == ",")
                {
                    strForCalc = "0.";
                    strButtonContent = "0,";
                }
            }
            else if (strButtonContent == ",")
                strForCalc = ".";

            AddToStrAndDisplay(strForCalc, strButtonContent);
        }
        private void Button_Operators(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            string strButtonName = sendButton.Name.ToString(),
                   strForDisplay = sendButton.Content.ToString(),
                   strForCalc = "";

            if (strButtonName == "ButtonRoot")
            {
                strForCalc = "R";
                if (boolResult)
                {
                    Clear();
                    boolResult = false;
                    strForCalc += strAns;
                    strForDisplay += strAns;
                }
            }
            else
            {
                if ((strForDisplay == "+") || (strForDisplay == "-"))
                    strForCalc = strForDisplay;
                else if (strButtonName == "ButtonMulti")
                    strForCalc = "*";
                else if (strButtonName == "ButtonDivide")
                    strForCalc = "/";
                else if (strButtonName == "ButtonPerc")
                {
                    strForCalc = "P";
                    strForDisplay = "%";
                }
                else if (strButtonName == "ButtonPow")
                {
                    strForCalc = "M";
                    strForDisplay = strForDisplay.Substring(1);
                }
                ClearAndAddAns();
            }
            AddToStrAndDisplay(strForCalc, strForDisplay);
        }
        private void Button_Ans(object sender, RoutedEventArgs e)
        {
            if (boolAns)
            {
                if (boolResult == false)
                    AddToStrAndDisplay(strAns);
                else
                    ClearAndAddAns();
            }
        }
        private void Button_Euro(object sender, RoutedEventArgs e)
        {
            if (boolEuro)
            {
                DisplayEur.Opacity = 0;
                boolEuro = false;
            }
            else
            {
                DisplayEur.Opacity = 0.9;
                boolEuro = true;
            }
        }
        private void Button_Negative(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(strAns)) { }
            else
            {
                double dblTempAns = (Convert.ToDouble(strAns) * -1);
                strAns = dblTempAns.ToString();
                
                if (boolEuro)
                    strDisplayBtm = String.Format("{0:C2}", Convert.ToDouble(strAns));
                else
                    strDisplayBtm = strAns;
                DisplayBtm.Content = strDisplayBtm;
                boolAns = true;
            }
        }
        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            Clear();
        }
        private void Button_ClearAll(object sender, RoutedEventArgs e)
        {
            Clear();
            strDisplayBtm = "";
            strAns = "";
            boolAns = false;
            DisplayBtm.Content = strDisplayBtm;
        }
        private void Button_Undo(object sender, RoutedEventArgs e)
        {
            if ((String.IsNullOrEmpty(strCalculation)) == false)
            {
                boolResult = false;
                int intTemp = (strCalculation.Length) - 1;
                strCalculation = strCalculation.Remove(intTemp);
                intTemp = (strDisplayTop.Length) - 1;
                strDisplayTop  = strDisplayTop.Remove(intTemp);
                DisplayTop.Content = strDisplayTop;
            }
        }
        private void Button_ClickResult(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(strCalculation)) { }
            else
            {
                strAns = ResultCompiler(strCalculation);
                if (boolEuro)
                    strDisplayBtm = String.Format("{0:C2}", Convert.ToDouble(strAns));
                else
                    strDisplayBtm = strAns;
                DisplayBtm.Content = strDisplayBtm;
                boolAns = true;
                boolResult = true;
            }
        }
        

        //Button support methods.
        private void ClearAndAddAns()
        {
            if ((boolClear && boolAns) || (boolResult && boolAns))
            {
                Clear();
                AddToStrAndDisplay(strAns);
            }
            boolResult = false;
        }
        private void AddToStrAndDisplay(string strCalc)
        {
            boolClear = false;
            strCalculation += strCalc;
            strDisplayTop += strCalc;
            DisplayTop.Content = strDisplayTop;
        }
        private void AddToStrAndDisplay(string strCalc, string strDisplay)
        {
            boolClear = false;
            strCalculation += strCalc;
            strDisplayTop += strDisplay;
            DisplayTop.Content = strDisplayTop;
        }
        private void Clear()
        {
            boolClear = true;
            strCalculation = "";
            strDisplayTop = "";
            DisplayTop.Content = strDisplayTop;
        }

        //Declare compiler. (This way it only needs to be declared once, not over and over each time the Compiler method is called.)
        CSharpCodeProvider codeProvider = new CSharpCodeProvider();

        CompilerParameters compParams = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false
        };
        //Compiler Methods
        public string ResultCompiler(string str)
        {
            string strReturn;
            CompilerResults results = codeProvider.CompileAssemblyFromSource(compParams, StringToCode(str));

            if (results.Errors.Count != 0)
            {
                // Compilation produces errors. Print out each error.
                string strErrorMessage = "Listing errors from compilation:";
                for (int i = 0; i < results.Errors.Count; i++)
                    strErrorMessage += ("\n" + results.Errors[i].ToString());

                throw new Exception("Compiling failed!" + strErrorMessage);
            }

            object objA = results.CompiledAssembly.CreateInstance("Base.MathConverter");
            MethodInfo metInfo = objA.GetType().GetMethod("ReturnCalculation");
            object objB = metInfo.Invoke(objA, null);
            strReturn= objB.ToString();
            return strReturn;
        }
        //String building methods. Which aid in building/changing the string to be compiled.
        public string StringToCode(string str)
        {
            //For when ans includes a "," which needs to be an "." Instead of constantly checking Ans seperately the entire strCalc is checked once before conversion.
            if (str.Contains(","))
            str = str.Replace(",", ".");
            EquationTypeCheck(str);
            str = BuildRPM(str);
            string strBase =
                @"  using System;
            namespace Base
            {
                public class MathConverter
                {
                    public VarX ReturnCalculation()
                    {
                        VarX varTemp = (Y)Placeholder;
                        return varTemp;
                    }
                }
            }
                        ";
            //Put actual calculation string in place of Placeholder + alter str to proper syntax
            strBase = strBase.Replace("Placeholder", str);
            strBase = strBase.Replace("VarX", strVarX);
            strBase = strBase.Replace("(Y)", strVarY);
            return strBase;
        }
        //Variables for string building.
        //Root = R Percent = P Power/Macht = M
        string strVarX = "int", strVarY = "";
        char[] chrSymbols = { '+', '-', '*', '/' , ')', '('};
        char[] chrSubstitude = {'R', 'P', 'M'};

        //String builder for support stringToCode. Which replaces R, P and M with the appropriate equations.
        public string BuildRPM(string str)
        {
            int intStart, intIndex, intEnd;
            intEnd = str.Length;
            intStart = 0; intIndex = 0;

            while ((intStart <= intEnd) && (intIndex > -1))
            {
                string strInsert, strTemp, strLetter;
                int intAddLength;
                intIndex = str.IndexOfAny(chrSubstitude, intStart);
                if (intIndex == -1) break;            //If non of the substitude letters are found break out of the while loop.
                strLetter = str.Substring(intIndex, 1);  //Find which substitude letter is present.

                if (strLetter == "R")
                {
                    /* If brackets are involved counts from the ( bracket until there is an even amount of closing and opening brackets.
                  Then takes everything between the outest () as strTemp.
                  */
                    if (str.Substring(intIndex + 1, 1) == "(")
                    {
                        int start = intIndex + 2,
                            at = 0,
                            intBracketR = 0,
                            intBracketL = 1;
                        char[] chrBrackets = { '(', ')' };

                        while ((start > -1) && (at > -1))
                        {
                            at = str.IndexOfAny(chrBrackets, start);
                            if (at > -1)
                            {
                                if (str.Substring(at, 1) == ")")
                                    intBracketR++;
                                else
                                    intBracketL++;
                                if (intBracketR == intBracketL)
                                    break;
                                start = at + 1;
                            }
                        }
                        strTemp = str.Substring(intIndex + 1, at - intIndex);
                    }
                    /*Starting at strIndex, find first chrSymbols. Substring starts one after strIndex and length is lenght till chrSymbols -1.
                        e.g. R25+5  counts from R, to + which is 3. Substring starts at 0+1=1 and is 3-1=2 long.
                    If no chrSymbols are found then likely the equation is something like R25 in which the substring goes from 2 to the end of the string.
                    */
                    else
                    {
                        int intTemp = str.IndexOfAny(chrSymbols, intIndex);
                        if (intTemp == -1)
                            strTemp = str.Substring(intIndex + 1);
                        else
                            strTemp = str.Substring((intIndex + 1), (intTemp - 1));
                    }
                    strInsert = "Math.Sqrt((Y){0})";
                    str = str.Replace((strLetter + strTemp), strInsert);
                }
                else
                {
                    /* If brackets are involved counts from the ) bracket until there is an even amount of closing and opening brackets.
                    Then takes everything between the outest () as strTemp.
                    */
                    if (str.Substring(intIndex - 1, 1) == ")")
                    {
                        int start = intIndex - 2,
                            at = 0,
                            intBracketR = 1,
                            intBracketL = 0;
                        char[] chrBrackets = { '(', ')' };

                        while ((start > -1) && (at > -1))
                        {
                            at = str.LastIndexOfAny(chrBrackets, start);
                            if (at > -1)
                            {
                                if (str.Substring(at, 1) == ")")
                                    intBracketR++;
                                else
                                    intBracketL++;
                                if (intBracketR == intBracketL)
                                    break;
                                start = at - 1;
                            }
                        }
                        strTemp = str.Substring(at, intIndex - at);
                    }
                    /*Starting at strIndex, go backwards till chrSymbol is found.
                   Substring starts from chrSymbol + 1 and is strIndex - intTemp long.

                   e.g. 5+50P; P = strIndex = 4; intTemp+1 = 1+1 = 2;  Substring starts at 5 = index 2; And is 4-2 = 2 char long.

                   Depending on strLetter a different strInsert is used.
                   e.g. strTemp + strLetter = 50P; 50P is replaced by ((Y){0}/ (Y)100); Formatting to ((Y)50 / (Y)100);
                   */
                    else
                    {
                        int intTemp = str.LastIndexOfAny(chrSymbols, intIndex) + 1;
                        strTemp = str.Substring(intTemp, (intIndex - intTemp));
                    }
                    if (strLetter == "P")
                        strInsert = "((Y){0} / (Y)100)";
                    else
                        strInsert = "((Y){0}*(Y){0})";
                    str = str.Replace((strTemp + strLetter), strInsert);
                }
                intAddLength = (string.Format(strInsert, strTemp)).Length - strTemp.Length;
                str = string.Format(str, strTemp);
                intStart = intIndex + intAddLength;
                intEnd = str.Length;
            }
            return str;
        }
        //Variable to use with enum CalculationType switch.
        EquationTypes equationType;
        char[] chrNeedsDoubleSymbols = { '.', '/', 'R', 'P', 'M' };

        //To check the equation type and use the switch in stringToCode before any str actually reaches the compiler. Instead of each button having to set the equationType seperately.
        public void EquationTypeCheck(string str)
        {
            if (boolEuro)
                equationType = EquationTypes.Euro;
            else if (str.IndexOfAny(chrNeedsDoubleSymbols) != -1)
                equationType = EquationTypes.Double;
            else
                equationType = EquationTypes.Int;

            EquationTypeSwitch(equationType);
        }
        public void EquationTypeSwitch(EquationTypes x)
        {
            switch (x)
            {
                case EquationTypes.Double:
                    strVarX = "double"; strVarY = "(double)";
                    break;
                case EquationTypes.Euro:
                    strVarX = "double"; strVarY = "(double)";
                    break;
                case EquationTypes.Int:
                    strVarX = "int"; strVarY = "(int)";
                    break;
                default:
                    strVarX = "int"; strVarY = "";
                    break;
            }
        }
        public enum EquationTypes
        {
            Double,
            Euro,
            Int
        }
    }
}
