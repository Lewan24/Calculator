﻿using System;
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
    private bool isSpaceNeeded = true;
    private string[]? memory;

    public MainWindow()
    {
        InitializeComponent();
        BackspaceButton.Content = "<-";
    }
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
            isSpaceNeeded = false;

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

            isSpaceNeeded = false;

            operationOutput.Text += operation;
            lastOperation = operation;
        }
        else
        {
            isSpaceNeeded = true;

            operationOutput.Text += $" {operation}";
            lastOperation = operation;
        }
    }

    private void AddOperationButton_Click(object sender, RoutedEventArgs e) => AddOperationToOutput((sender as Button).Content.ToString());
    private void AddOperationToOutput(string operation)
    {
        if (lastOperation == "+" || lastOperation == "-" || lastOperation == "*" || lastOperation == "/")
        {
            string[] splittedText = operationOutput.Text.Split(" ");

            splittedText[splittedText.Length - 1] = operation;

            operationOutput.Text = string.Join(" ", splittedText);
        }
        else
            operationOutput.Text += $" {operation}";

        lastOperation = operation;
    }

    private void MainFunctionsButton_Click(object sender, RoutedEventArgs e)
    {
        ExecuteMainFunction((sender as Button).Content.ToString());
    }
    private void ExecuteMainFunction(string operation)
    {
        switch (operation)
        {
            case "MEMORY":
                if (memory.Length >= 1)
                {
                    if (lastOperation.All(Char.IsDigit) || lastOperation == "+/-" || lastOperation == ".")
                        operationOutput.Text += string.Join(" ", memory);
                    else
                        operationOutput.Text += $" {string.Join(" ", memory)}";

                    lastOperation = memory.Last();
                }
                break;
            case "MEMORY+":
                memory = operationOutput.Text.Split(" ");
                break;
            case "CLEAR":
                operationOutput.Text = "0";
                break;
        }
    }

    private void BackspaceButton_Click(object sender, RoutedEventArgs e) => RemoveLastItem();
    private void RemoveLastItem()
    {
        string[] splittedText = operationOutput.Text.Split(" ");
        splittedText = splittedText.SkipLast(1).ToArray();

        operationOutput.Text = string.Join(" ", splittedText);

        lastOperation = "-";
    }

    private void EnterButton_Click(object sender, RoutedEventArgs e)
    {
        string[] splittedText = operationOutput.Text.Split(" ");

        int i = 0;

        while (splittedText.Contains("*") || splittedText.Contains("/"))
        {
            if (splittedText[i] == "*" && i > 0)
            {
                splittedText[i - 1] = $"{double.Parse(splittedText[i - 1]) * double.Parse(splittedText[i + 1])}";
                splittedText[i] = ""; splittedText[i + 1] = "";
                i = 0;
            }
            else if (splittedText[i] == "/" && i > 0)
                if (double.Parse(splittedText[i + 1]) != 0)
                {
                    splittedText[i - 1] = $"{double.Parse(splittedText[i - 1]) / double.Parse(splittedText[i + 1])}";
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
                splittedText[i - 1] = $"{double.Parse(splittedText[i - 1]) + double.Parse(splittedText[i + 1])}";
                splittedText[i] = ""; splittedText[i + 1] = "";
                i = 0;
            }
            else if (splittedText[i] == "-" && i > 0)
            {
                splittedText[i - 1] = $"{double.Parse(splittedText[i - 1]) - double.Parse(splittedText[i + 1])}";
                splittedText[i] = ""; splittedText[i + 1] = "";
                i = 0;
            }
            else i++;

            splittedText = splittedText.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        operationOutput.Text = string.Concat(splittedText);
    }
}