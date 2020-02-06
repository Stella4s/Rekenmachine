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
        - Make helper stringbuilder method to convert math buttons input into readable code to put into the compiler.
        - Euro, Procent en Decimal buttons.
            
            */
        //Declaring Variables.
        string strCalculation, strDisplayTop, strDisplayBtm;
        bool boolAns;

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
            strCalculation += sendButton.Content.ToString();
            strDisplayTop += sendButton.Content.ToString();
            DisplayTop.Content = strDisplayTop;
        }
        private void Button_ClickMath(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            if (boolAns)
            {
                strCalculation = strDisplayBtm;
                strDisplayTop = strDisplayBtm;
                boolAns = false;
            }
            strCalculation += sendButton.Content.ToString();
            strDisplayTop += sendButton.Content.ToString();
            DisplayTop.Content = strDisplayTop;
        }
        private void Button_ClickResult(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            strDisplayBtm = Compiler(strCalculation);
            DisplayBtm.Content = strDisplayBtm;
            boolAns = true;
        }

        private void Button_ClickAns(object sender, RoutedEventArgs e)
        {
            if (boolAns)
            {
                strCalculation = strDisplayBtm;
                strDisplayTop = strDisplayBtm;
                boolAns = false;
                DisplayTop.Content = strDisplayTop;
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

        public string stringToCode(string str)
        {
            string strBase =
                @"
            namespace Base
            {
                public class MathConverter
                {
                    public int ReturnCalculation()
                    {
                        int intTemp = Placeholder;
                        return intTemp;
                    }
                }
            }
                        ";
            //Put actual calculation string in place of Placeholder.
            return strBase.Replace("Placeholder", str);
        }
    }
}
