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
            calculationVarType(equationTypes.Double);
            addToStrAndDisplay("/", sendButton.Content.ToString());
        }
        private void Button_ClickPerc(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            calculationVarType(equationTypes.Double);
            addToStrAndDisplay(equationBuildPercent(), sendButton.Content.ToString());
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
                calculationVarType(equationTypes.Double);
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
        private void Button_Pow(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            sqrtActive();
            calculationVarType(equationTypes.Double);
            addToStrAndDisplay(equationPowerBuild(), sendButton.Content.ToString().Substring(1));
        }
        private void Button_Root(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            sqrtActive();
            calculationVarType(equationTypes.Double);
            addToStrAndDisplay("Math.Sqrt((Y)", sendButton.Content.ToString());
            boolSqrtActive = true;
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
                calculationVarType(equationTypes.Double);
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
 
        






        //String formatting/building methods.
        //Variables for string building.
        string strVarX = "int", strVarY ="", strSymbols = "+-*/()";

       

        public string equationBuildPercent()
        {
            char[] chrSymbols = strSymbols.ToCharArray();
            int intTemp = (strCalculation.LastIndexOfAny(chrSymbols) + 1);
            string strTemp = strCalculation.Substring(intTemp);
            strCalculation = strCalculation.Remove(intTemp);
            return string.Format("((Y){0} / (Y)100)", strTemp);
        }
        private string equationPowerBuild()
        {
            char[] chrSymbols = strSymbols.ToCharArray();
            int intTemp = (strCalculation.LastIndexOfAny(chrSymbols) + 1);
            string strTemp = strCalculation.Substring(intTemp);
            strCalculation = strCalculation.Remove(intTemp);
            return string.Format("((Y){0}*(Y){0})", strTemp);
        }

        public void calculationVarType(equationTypes x)
        {
            switch (x)
            {
                case equationTypes.Double:
                    strVarX = "double"; strVarY = "(double)";
                    break;
                case equationTypes.Macht:
                    break;
                case equationTypes.Root:
                    break;
                //case equationTypes.Euro:
                   // break;
                case equationTypes.Reset:
                    strVarX = "int"; strVarY = "";
                    break;
                default:
                    strVarX = "int"; strVarY ="";
                    break;
            }
        }
        public enum equationTypes
        {
            Double,
            Macht,
            Root,
            //Euro,
            Reset
        }

        public string stringToCode(string str)
        {
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
    }
}
