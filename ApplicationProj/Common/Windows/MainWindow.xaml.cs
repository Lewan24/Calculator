using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ApplicationProj;

/// <summary>
/// Main window of application
/// </summary>
public partial class MainWindow : Window
{
    private string lastOperation = "0";
    private string? memory;

    public MainWindow() => InitializeComponent();
    private void MouseDown_Drag(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }
    private void CloseProgramButton_Click(object sender, RoutedEventArgs e) => Close();
    private void MinimizeProgramButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

    private void AddOrChangeNumberButton_Click(object sender, RoutedEventArgs e) => AddToOutput((sender as Button).Content.ToString());
    private void AddToOutput(string operation)
    {
        if (operation == ".")
        {
            if (lastOperation.All(Char.IsDigit))
            {
                string[] splittedText = operationOutput.Text.Split(' ');

                string[] lastItem = splittedText[splittedText.Length-1].Split('.');

                if (lastItem.Length == 1)
                {
                    operationOutput.Text += operation;
                    lastOperation = operation;
                }
            }
        }
        else if (operation == "+/-")
        {
            string[] splittedText = operationOutput.Text.Split(' ');

            if (lastOperation != ".")
            {
                splittedText[splittedText.Length - 1] = $"{(double.Parse(splittedText.Last()) * -1)}";

                operationOutput.Text = string.Join(" ",splittedText);
            }

            lastOperation = operation;
        }
        else if (lastOperation.All(Char.IsDigit) && operation.All(Char.IsDigit) || 
                lastOperation == "." && operation.All(Char.IsDigit) ||
                lastOperation == "+/-" && operation.All(Char.IsDigit))
        {
            if (operationOutput.Text == "0")
                operationOutput.Text = "";
            operationOutput.Text += operation;

            lastOperation = operation;
        }
        else
        {
            string[] splittedText = operationOutput.Text.Split(" ");
            if(splittedText.Last().All(Char.IsDigit))
                operationOutput.Text += operation;
            else operationOutput.Text += $" {operation}";

            lastOperation = operation;
        }

        ExecuteResult();
    }

    private void AddOperationButton_Click(object sender, RoutedEventArgs e) => AddOperationToOutput((sender as Button).Content.ToString());
    private void AddOperationToOutput(string operation)
    {
        string[] splittedText = operationOutput.Text.Split(" ");

        if (lastOperation == "+" || lastOperation == "-" || lastOperation == "*" || lastOperation == "/" ||
            splittedText.Last() == "+" || splittedText.Last() == "-" || splittedText.Last() == "*" || splittedText.Last() == "/")
        {
            splittedText[splittedText.Length - 1] = operation;
            operationOutput.Text = string.Join(" ", splittedText);
        }
        else operationOutput.Text += $" {operation}";
            
        lastOperation = operation;
    }

    private void MainFunctionsButton_Click(object sender, RoutedEventArgs e) => ExecuteMainFunction((sender as Button).Content.ToString());
    private void ExecuteMainFunction(string operation)
    {
        switch (operation)
        {
            case "MEMORY":
                if (memory is not null && memory != "0")
                {
                    if (lastOperation.All(Char.IsDigit) || lastOperation == "+/-" || lastOperation == ".")
                        operationOutput.Text += memory;
                    else
                        operationOutput.Text += $" {memory}";

                    lastOperation = memory;

                    ExecuteResult();
                }
                break;
            case "MEMORY+":
                memory = result.Text;
                break;
            case "CLEAR":
                operationOutput.Text = "0";
                result.Text = "0";
                lastOperation = "0";
                break;
        }
    }

    private void BackspaceButton_Click(object sender, RoutedEventArgs e) => RemoveLastItem();
    private void RemoveLastItem()
    {
        if (!String.IsNullOrEmpty(operationOutput.Text) && operationOutput.Text != "0")
        {
            string[] splittedText = operationOutput.Text.Split(" ");
            
            if(splittedText.Length > 1)
            {
                splittedText = splittedText.SkipLast(1).ToArray();

                operationOutput.Text = string.Join(" ", splittedText);

                lastOperation = "<-";
            }
        }

        ExecuteResult();
    }

    private void EnterButton_Click(object sender, RoutedEventArgs e) => ExecuteResult();
    private void ExecuteResult()
    {
        string[] splittedText = operationOutput.Text.Split(" ");

        if (splittedText.Last() == "+" || splittedText.Last() == "-" ||
                splittedText.Last() == "*" || splittedText.Last() == "/")
        {
            splittedText[splittedText.Length - 1] = "";
            splittedText = splittedText.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        if (splittedText.Length >= 3)
        {
            int i = 0;

            while (splittedText.Contains("*") || splittedText.Contains("/"))
            {
                if (splittedText[i] == "*" && i > 0)
                {
                    splittedText[i - 1] = (double.Parse(splittedText[i - 1], new CultureInfo("en-GB")) * double.Parse(splittedText[i + 1], new CultureInfo("en-GB"))).ToString(new CultureInfo("en-GB"));
                    splittedText[i] = ""; splittedText[i + 1] = "";
                    i = 0;
                }
                else if (splittedText[i] == "/" && i > 0)
                    if (double.Parse(splittedText[i + 1], new CultureInfo("en-GB")) != 0)
                    {
                        splittedText[i - 1] = (double.Parse(splittedText[i - 1], new CultureInfo("en-GB")) / double.Parse(splittedText[i + 1], new CultureInfo("en-GB"))).ToString(new CultureInfo("en-GB"));
                        splittedText[i] = ""; splittedText[i + 1] = "";
                        i = 0;
                    }
                    else { splittedText[i - 1] = "0"; splittedText[i] = ""; splittedText[i + 1] = ""; }
                else i++;

                splittedText = splittedText.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }

            i = 0;

            while (splittedText.Contains("+") || splittedText.Contains("-"))
            {
                if (splittedText[i] == "+" && i > 0)
                {
                    splittedText[i - 1] = (double.Parse(splittedText[i - 1], new CultureInfo("en-GB")) + double.Parse(splittedText[i + 1], new CultureInfo("en-GB"))).ToString(new CultureInfo("en-GB"));
                    splittedText[i] = ""; splittedText[i + 1] = "";
                    i = 0;
                }
                else if (splittedText[i] == "-" && i > 0)
                {
                    splittedText[i - 1] = (double.Parse(splittedText[i - 1], new CultureInfo("en-GB")) - double.Parse(splittedText[i + 1], new CultureInfo("en-GB"))).ToString(new CultureInfo("en-GB"));
                    splittedText[i] = ""; splittedText[i + 1] = "";
                    i = 0;
                }
                else i++;

                splittedText = splittedText.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }

            result.Text = string.Concat(splittedText);
        }
    }

    private void ExtraFunctionButton_Click(object sender, RoutedEventArgs e) => ExecuteExtraFunction((sender as Button).Name);
    private void ExecuteExtraFunction(string name)
    {
        string[] splittedText = operationOutput.Text.Split(" ");

        double number;

        if (Double.TryParse(splittedText.Last(), NumberStyles.Float, new CultureInfo("en-GB"), out number))
        {
            switch (name)
            {
                case "oneSlashx":
                    splittedText[splittedText.Length - 1] = (1 / number).ToString(new CultureInfo("en-GB"));
                    break;
                case "xToPowerOf2":
                    splittedText[splittedText.Length - 1] = (Math.Pow(number, 2)).ToString(new CultureInfo("en-GB"));
                    break;
                case "squareRootOfx":
                    splittedText[splittedText.Length - 1] = (Math.Sqrt(number)).ToString(new CultureInfo("en-GB"));
                    break;
            }

            operationOutput.Text = String.Join(" ", splittedText);
            ExecuteResult();
            lastOperation = "1";
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                ExecuteResult();
                break;
            case Key.Back:
                RemoveLastItem();
                break;

            case Key.D0: case Key.NumPad0:
                AddToOutput("0");
                break;
            case Key.D1: case Key.NumPad1:
                AddToOutput("1");
                break;
            case Key.D2: case Key.NumPad2:
                AddToOutput("2");
                break;
            case Key.D3: case Key.NumPad3:
                AddToOutput("3");
                break;
            case Key.D4: case Key.NumPad4:
                AddToOutput("4");
                break;
            case Key.D5: case Key.NumPad5:
                AddToOutput("5");
                break;
            case Key.D6: case Key.NumPad6:
                AddToOutput("6");
                break;
            case Key.D7: case Key.NumPad7:
                AddToOutput("7");
                break;
            case Key.D8: case Key.NumPad8: 
            AddToOutput("8");
                break;
            case Key.D9: case Key.NumPad9:
                AddToOutput("9");
                break;

            case Key.Multiply:
            AddOperationToOutput("*");
                break;
            case Key.Add: case Key.OemPlus:
                AddOperationToOutput("+");
                break;
            case Key.Subtract: case Key.OemMinus:
                AddOperationToOutput("-");
                break;
            case Key.Divide: case Key.OemQuestion:
                AddOperationToOutput("/");
                break;
        }
    }
}