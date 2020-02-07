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
        - Check if string is empty before attempting to compile.
        - Make clear buttons + delete button functional.
        - Make helper stringbuilder method to convert math buttons input into readable code to put into the compiler. [In progress]
        - Euro, Procent en Decimal buttons. [Done]
        - Clean-up code. Limit amount of different methods, try streamline process.
            
            */
        //Declaring Variables.
        string strCalculation, strDisplayTop, strDisplayBtm;
        bool boolAns, boolEuro = false;

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
                strCalculation = "";
                strDisplayTop = "";
                boolAns = false;
            }
            addToStrAndDisplay(sendButton.Content.ToString());
        }
        private void Button_ClickMath(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            addToStrAndDisplay(sendButton.Content.ToString());
        }
   
        private void Button_ClickAns(object sender, RoutedEventArgs e)
        {
            checkAns();
        }
        private void Button_ClickMulti(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            addToStrAndDisplay("*", sendButton.Content.ToString());
        }
        private void Button_ClickDiv(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
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
            Button sendButton = e.Source as Button;
            if (boolAns)
            {
                strCalculation = "";
                strDisplayTop = "";
                boolAns = false;
            }
            boolEuro = true;
            calculationVarType(equationTypes.Double);
            addToStrAndDisplay("", sendButton.Content.ToString());
        }
        private void Button_ClickDot(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            checkAns();
            addToStrAndDisplay(".", sendButton.Content.ToString());
        }
        private void Button_ClickResult(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            needsDouble();
            strDisplayBtm = Compiler(strCalculation);
            if (boolEuro)
                strDisplayBtm = String.Format("{0:C2}", Convert.ToDouble(strDisplayBtm));
            DisplayBtm.Content = strDisplayBtm;
            boolAns = true;
        }
        



        //Button support methods.
        private void checkAns()
        {
            if (boolAns)
            {
                strCalculation = strDisplayBtm;
                strDisplayTop = strDisplayBtm;
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
        string strVarX = "int", strVarY ="", strSymbols = "+-*/";

       

        public string equationBuildPercent()
        {
            char[] chrSymbols = strSymbols.ToCharArray();
            int intTemp = (strCalculation.LastIndexOfAny(chrSymbols) + 1);
            string strTemp = strCalculation.Substring(intTemp);
            strCalculation = strCalculation.Remove(intTemp);
            return string.Format("((Y){0} / (Y)100)", strTemp);
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
                @"
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
            if (str.Contains(","))
                str = str.Replace(",", ".");
            if (str.Contains("€"))
                str = str.Replace("€", "");
            strBase = strBase.Replace("Placeholder", str);
            strBase = strBase.Replace("VarX", strVarX);
            strBase = strBase.Replace("(Y)", strVarY);

            return strBase;
        }
    }
}
