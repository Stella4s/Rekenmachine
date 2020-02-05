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

namespace ItViteaRekenmachine01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Variables to hold numbers.
        Int32 intA, intB, intResult;
        string strInputA, strInputB, strEquation;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_ClickMath(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            toNumber(strInputA);
            Display.Content += " " + sendButton.Content.ToString() + " ";
            strEquation = sendButton.Content.ToString();
            strInputA = "";
        }

        private void Button_ClickResult(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            toNumber(strInputA);
            doEquation(strEquation);
            Display.Content += " " + sendButton.Content.ToString() + " " + intResult;
        }

        private void Button_ClickNumber(object sender, RoutedEventArgs e)
        {
            Button sendButton = e.Source as Button;
            strInputA += sendButton.Content.ToString();
            Display.Content += sendButton.Content.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Button sendButton = e.Source as Button;
            //strInput += addNumber(sendButton.Content.ToString());
            
        }

       /* public string addNumber(string num)
        {
            return num;
        }*/
        public void toNumber(string strNum)
        {
            if (String.IsNullOrEmpty(strNum))
            {

            }
            else
            {
                try
                {
                    intA = Int32.Parse(strNum);
                }
                catch
                { }
            }
        }

        public void doEquation(string strInput)
        {
            if (strInput == "+")
            {
                intResult = intA + intB;
            }
            else if (strInput == "-")
            {
                intResult = intA - intB;
            }
            else if (strInput == "&#xd7;")
            {
                intResult = intA * intB;
            }
            else if (strInput == "&#xf7;")
            {
                intResult = intA / intB;
            }
        }
    }
}
