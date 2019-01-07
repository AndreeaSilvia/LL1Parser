using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LL1_remake
{
    public partial class MainWindow : Window
    {
        Grammar myGrammar;
        ParsingTable myTable;
        CompileCode generateCode;
        bool allOK = false;
        bool grammarFlag = false;
        bool symbolsFlag = false;
        bool parsingFlag = false;
        bool executableFlag = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FilePath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            myGrammar = new Grammar();
            allOK = false;
            grammarFlag = false;
            parsingFlag = false;
            executableFlag = false;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            string filename = dlg.FileName;
            FilePath.Text = filename;
            if (FilePath.Text.Count() == 0) return;

            StreamReader grammar = new StreamReader(FilePath.Text);
            char[] delimiterChars = { ' ', '\t' };
            myGrammar.StartSymbol = grammar.ReadLine();
            myGrammar.Nonterminals.AddRange(grammar.ReadLine().Split(delimiterChars));
            myGrammar.Terminals.AddRange(grammar.ReadLine().Split(delimiterChars));
            myGrammar.RulesNumber = Int32.Parse(grammar.ReadLine());
            for (int i = 0; i < myGrammar.RulesNumber; i++)
                myGrammar.CreateRules(grammar.ReadLine());
            myGrammar.Terminals.Add("$");
            grammar.Close();
        }

        private void TestGrammarButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilePath.Text.Count() == 0)
            {
                Error myError = new Error();
                myError.Message.Text = "There's no grammar loaded.";
                myError.Show();
                return;
            }
            else grammarFlag = true;
            if (myGrammar.test_grammar == false)
            {
                myGrammar.test_grammar = true;
                bool status1, status2;
                status1 = myGrammar.CommonPrefixes();
                status2 = myGrammar.LeftRecursion();
                if (status1 == true || status2 == true)
                {
                    Information actiondone = new Information();
                    actiondone.Message.Text = "Your grammar didn't meet the predictive analysis' criteria. \nBellow it's the fixed grammar for you. \n";
                    myGrammar.PrintInformation(ref actiondone);
                    actiondone.Show();
                }
                else if (status1 == false && status2 == false)
                {
                    Information actiondone = new Information();
                    actiondone.Message.Text = "Your grammar met the predictive analysis' criteria. \nBellow it's the input grammar. \n";
                    myGrammar.PrintInformation(ref actiondone);
                    actiondone.Show();
                }
            }
            else
            {
                Information actiondone = new Information();
                actiondone.Message.Text = "You've already tested this grammar. \nHere she is. \n";
                myGrammar.PrintInformation(ref actiondone);
                actiondone.Show();
            }
        }

        private void TestLL1Button_Click(object sender, RoutedEventArgs e)
        {
            if (grammarFlag == false) 
            {
                Error myError = new Error();
                myError.Message.Text = "There's no grammar loaded.";
                myError.Show();
                return;
            }
            else {
                myTable = new ParsingTable();
                myTable.GrammarToBoChecked = myGrammar;
                myTable.CalculateFirst();
                myTable.CalculateFollow();
                allOK = myTable.CalculateIntersections();
                if (allOK == true)
                {
                    Information actiondone = new Information();
                    actiondone.Message.Text = "Your grammar follows the LL(1)'s conditions. \nBellow are the FIRST and FOLLOW. \n";
                    myTable.PrintInformation(ref actiondone);
                    actiondone.Show();
                }
                if (allOK == false)
                {
                    Information actiondone = new Information();
                    actiondone.Message.Text = "Your grammar does't follow the LL(1)'s conditions. \nBellow are the FIRST and FOLLOW. \n";
                    myTable.PrintInformation(ref actiondone);
                    actiondone.Show();
                }
                parsingFlag = false;
                symbolsFlag = true;
            }
        }

        private void CreateTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (symbolsFlag == false)
            {
                Error myError = new Error();
                myError.Message.Text = "FIRST+ and FOLLOW+ are nonexistent.";
                myError.Show();
                return;
            }
            myTable.CalculateParsingTable();
            parsingFlag = true;
        }

        private void SeeTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (parsingFlag == false)
            {
                Error myError = new Error();
                myError.Message.Text = "There was no table created. Nothing to show.";
                myError.Show();
                return;
            }
            Parse result = new Parse();
            if (allOK == true) myTable.PrintParseTable(ref result);
            else result.Message.Text = "Because your grammar wasn't LL(1) the table couldn't be done";
            result.Show();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (parsingFlag == false)
            {
                Error myError = new Error();
                myError.Message.Text = "There was no table created. Nothing to generate.";
                myError.Show();
                return;
            }
            generateCode = new CompileCode();
            generateCode.lookahead = myTable;
            generateCode.rulesconstruct = myGrammar;
            generateCode.GenerateCode();
            generateCode.CompileGeneratedCode();
            executableFlag = true;
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (executableFlag== false)
            {
                Error myError = new Error();
                myError.Message.Text = "There was no executable created. Nothing to run.";
                myError.Show();
                return;
            }
            string temp = string.Empty;
            temp = Input_Expresion.Text;
            System.Diagnostics.ProcessStartInfo param = new System.Diagnostics.ProcessStartInfo();
            param.FileName = "SentenceChecker.exe";
            param.Arguments = temp;
            param.UseShellExecute = false;
            param.RedirectStandardOutput = true;
            param.CreateNoWindow = true;
            System.Diagnostics.Process execute = System.Diagnostics.Process.Start(param);
            string output = execute.StandardOutput.ReadToEnd();
            Result.Text = output;
            execute.WaitForExit();
        }
    }
}
