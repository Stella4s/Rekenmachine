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
        - Check if string is empty before attempting to compile. [Done]
        - Make clear buttons + delete button functional. [Done]
        - Make Sqrt and Pow work. [Done]
        - Euro, Procent en Decimal buttons. [Done]
        - Make +/- button work. [Done]
        - Made Root, Pow and Perc buttons work through 1 button and add a letter that will later be converted to the appropriate math when the strCalculation is send to the converter. [Done]

        - Fix undo button. (It messes up when attempting to undo one char whilst Percent, Pow and Sqrt add longer strings.)
        - Have try-catch methods for faulty input by user. (e.g. Dividing by zero, Straight up faulty syntax, etc.)
        - Clean-up code. Limit amount of different methods, try streamline process.

        - Suggested changes.
            - Make Sqrt, Pow and Perc strings be included AFTER = is pressed. Using singular 1 char symbols in base string. (So Undo will work.)
            And replacing said symbols before putting the string into the compiler.
            Would also make the boolSqrtActive way of solving the bracket issue with using math.sqrt obsolete, cleaning up code in the process.

            - Find a way to condense and simplify the amount of button methods?
            Make multiple buttons hook up to one button method. And have that method send the buttonname? To a switch method who changes the variables put in depending on the button name.
            Reducing the amount of repeat code. (Button sendbutton, addToStrAndDisplay, checkAns, sqrtActive, Checkdouble.)

            - Streamline enums and equation types. 
            Either remove enums altogether and work with a single bool to switch between int and double.
            Or use the enum equation types and the switch, to switch between variable types depending on the size of the calculation.
            The enums don't seem to be needed, nor the switch. (As only two options are used.) So likely remove this altogether.
            
            
            
            */
        //Declaring Variables.
        string strCalculation, strDisplayTop, strDisplayBtm, strAns;
        bool boolAns, boolClear, boolEuro = false, boolSqrtActive;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Buttton methods
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_ClickNumber(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            if (boolAns)
            {
                clear();
                boolAns = false;
            }
            addToStrAndDisplay(sendButton.Content.ToString());
        }
        private void Button_ClickMath(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            sqrtActive();
            addToStrAndDisplay(sendButton.Content.ToString());
        }
   
        private void Button_ClickAns(object sender, RoutedEventArgs e)
        {
            addToStrAndDisplay(strAns);
        }
        private void Button_ClickMulti(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            sqrtActive();
            addToStrAndDisplay("*", sendButton.Content.ToString());
        }
        private void Button_ClickDiv(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            sqrtActive();
            calculationVarType(EquationTypes.Double);
            addToStrAndDisplay("/", sendButton.Content.ToString());
        }
        private void Button_ClickEuro(object sender, RoutedEventArgs e)
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
                calculationVarType(EquationTypes.Double);
            }
            //addToStrAndDisplay("", sendButton.Content.ToString());
        }
        private void Button_Neg(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(strAns)) { }
            else
            {
                needsDouble();
                strAns += "*-1";
                strAns = Compiler(strAns);
                if (boolEuro)
                    strDisplayBtm = String.Format("{0:C2}", Convert.ToDouble(strAns));
                else
                    strDisplayBtm = strAns;
                DisplayBtm.Content = strDisplayBtm;
                boolAns = true;
            }
        }
        private void Button_RPM(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            string strForCalc ="", strForDisplay =""; 
            string strButtonName = sendButton.Name.ToString();
            if (strButtonName == "ButtonPow")
            {
                strForCalc = "M";
                strForDisplay = sendButton.Content.ToString().Substring(1);
            }
            else if (strButtonName == "ButtonPerc")
            {
                calculationVarType(EquationTypes.Double);
                strForCalc = "P";
                strForDisplay = "%";
            }
            else if (strButtonName == "ButtonRoot")
            {
                calculationVarType(EquationTypes.Double);
                strForCalc = "R";
                strForDisplay = sendButton.Content.ToString();
            }
            checkAns();
            addToStrAndDisplay(strForCalc, strForDisplay);
        }
        private void Button_ClickDot(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            addToStrAndDisplay(".", sendButton.Content.ToString());
        }
        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            clear();
        }
        private void Button_ClearAll(object sender, RoutedEventArgs e)
        {
            clear();
            strDisplayBtm = "";
            strAns = "";
            boolAns = false;
            DisplayBtm.Content = strDisplayBtm;
        }
        private void Button_Undo(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(strCalculation)) { }
            else
            { 
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
                needsDouble();
                sqrtActive();
                strAns = Compiler(strCalculation);
                if (boolEuro)
                    strDisplayBtm = String.Format("{0:C2}", Convert.ToDouble(strAns));
                else
                    strDisplayBtm = strAns;
                DisplayBtm.Content = strDisplayBtm;
                boolAns = true;
            }
        }
        

        //Button support methods.
        private void checkAns()
        {
            if (boolAns)
            {
                strCalculation = strAns;
                strDisplayTop = strAns;
                boolAns = false;
                DisplayTop.Content = strDisplayTop;
            }
        }
        private void addToStrAndDisplay(string strCalc)
        {
            strCalculation += strCalc;
            strDisplayTop += strCalc;
            DisplayTop.Content = strDisplayTop;
        }
        private void addToStrAndDisplay(string strCalc, string strDisplay)
        {
            strCalculation += strCalc;
            strDisplayTop += strDisplay;
            DisplayTop.Content = strDisplayTop;
        }
        public void needsDouble()
        {
            if (strCalculation.Contains("."))
            {
                calculationVarType(EquationTypes.Double);
            }
        }
        private void sqrtActive()
        {
            if (boolSqrtActive)
            {
                strCalculation += ")";
                boolSqrtActive = false;
            }
        }
        private void clear()
        {
            strCalculation = "";
            strDisplayTop = "";
            boolClear = true;
            DisplayTop.Content = strDisplayTop;
        }

        //Compiler Methods
        public string Compiler(string str)
        {
            string strReturn;
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            CompilerParameters compParams = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            CompilerResults results = codeProvider.CompileAssemblyFromSource(compParams, stringToCode(str));

            if (results.Errors.Count != 0)
                throw new Exception("Compiling failed!");

            object objA = results.CompiledAssembly.CreateInstance("Base.MathConverter");
            MethodInfo metInfo = objA.GetType().GetMethod("ReturnCalculation");
            object objB = metInfo.Invoke(objA, null);
            strReturn= objB.ToString();
            return strReturn;
        }
        //String building methods.
        public string stringToCode(string str)
        {
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
            //Now Ans is separate from display , to . and removing € should not be necessary?
            //if (str.Contains(","))
            //str = str.Replace(",", ".");
            //if (str.Contains("€"))
            //  str = str.Replace("€", "");

            strBase = strBase.Replace("Placeholder", str);
            strBase = strBase.Replace("VarX", strVarX);
            strBase = strBase.Replace("(Y)", strVarY);
            return strBase;
        }
        //Variables for string building.
        //Root = R Percent = P Power/Macht = M
        string strVarX = "int", strVarY ="", strSymbols = "+-*/", strSubstitudeLetters = "RPM";


        //String builder for when support stringToCode.
        public string BuildRPM(string str)
        {
            char[] chrSymbols = strSymbols.ToCharArray();
            char[] chrSubstitude = strSubstitudeLetters.ToCharArray();

            int strStart, strIndex, strEnd;
            strEnd = str.Length;
            strStart = 0; strIndex = 0;

            while ((strStart <= strEnd) && (strIndex > -1))
            {
                string strInsert, strTemp, strLetter;
                strIndex = str.IndexOfAny(chrSubstitude, strStart);
                if (strIndex == -1) break;            //If non of the substitude letters are found break out of the while loop.
                strLetter = str.Substring(strIndex);  //Find which substitude letter is present.

                if (strLetter == "R")
                {
                    /*Starting at strIndex, find first chrSymbols. Substring starts one after strIndex and length is lenght till chrSymbols -1.
                        e.g. R25+5  counts from R, to + which is 3. Substring starts at 0+1=1 and is 3-1=2 long.
                    */
                    int intTemp = str.IndexOfAny(chrSymbols, strIndex) - 1;
                    strTemp = str.Substring((strIndex + 1), intTemp);
                    strInsert = "Math.Sqrt((Y){0})";
                }
                else
                {
                    /*Starting at strIndex, go backwards till chrSymbol is found.
                    Substring starts from chrSymbol + 1 and is strIndex - intTemp long.
                        
                    e.g. 5+50P; P = strIndex = 4; intTemp+1 = 1+1 = 2;  Substring starts at 5 = index 2; And is 4-2 = 2 char long.

                    Depending on strLetter a different strInsert is used.
                    e.g. strTemp + strLetter = 50P; 50P is replaced by ((Y){0}/ (Y)100); Formatting to ((Y)50 / (Y)100);
                    */
                    int intTemp = str.LastIndexOfAny(chrSymbols, strIndex) + 1;
                    strTemp = str.Substring(intTemp, (strIndex - intTemp));
                    if (strLetter == "P")
                        strInsert = "((Y){0} / (Y)100)";
                    else
                        strInsert = "((Y){0}*(Y){0})";
                }
                str = str.Replace((strTemp + strLetter), strInsert);
                str = string.Format(str, strTemp);
                strStart = strIndex + 1;
            }
            return str;
        }

        public void calculationVarType(EquationTypes x)
        {
            switch (x)
            {
                case EquationTypes.Double:
                    strVarX = "double"; strVarY = "(double)";
                    break;
                case EquationTypes.Macht:
                    break;
                case EquationTypes.Root:
                    break;
                //case equationTypes.Euro:
                   // break;
                case EquationTypes.Reset:
                    strVarX = "int"; strVarY = "";
                    break;
                default:
                    strVarX = "int"; strVarY ="";
                    break;
            }
        }
        public enum EquationTypes
        {
            Double,
            Macht,
            Root,
            //Euro,
            Reset
        }
    }
}
