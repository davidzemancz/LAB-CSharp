using System;

namespace Calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IInputReader inputReader = new ConsoleInputReader();
            //inputReader = new FileInputReader(@"C:\Users\david.zeman\Downloads\data\04.in");
            IOutputWriter outputWriter = new ConsoleOutputWriter();
            Int32OperatorHandler int32OperatorHandler = new Int32OperatorHandler();
            DoubleOperatorHandler doubleOperatorHandler = new DoubleOperatorHandler();
            Expression expression = null;
            ExpressionEvaluationStatus status;
            int resultInt;
            double resultDouble;

            inputReader.Open();
            outputWriter.Open();

            while (true)
            {
                string command = inputReader.ReadLine();
                if (command == null || command == "end") break;
                else if (command.Length > 0)
                {
                    if (command[0] == 'i' && command.Length == 1)
                    {
                        if (expression == null) outputWriter.WriteLine("Expression Missing");
                        else
                        {
                            resultInt = expression.Evaluate(int32OperatorHandler, out status);
                            if (status.State == ExpressionEvaluationStatus.StateEnum.Ok) outputWriter.WriteLine(resultInt.ToString());
                            else outputWriter.WriteLine(status.ToString());
                        }
                    }
                    else if (command[0] == 'd' && command.Length == 1)
                    {
                        if (expression == null) outputWriter.WriteLine("Expression Missing");
                        else
                        {
                            resultDouble = expression.Evaluate(doubleOperatorHandler, out status);
                            if (status.State == ExpressionEvaluationStatus.StateEnum.Ok) outputWriter.WriteLine(resultDouble.ToString("f05"));
                            else outputWriter.WriteLine(status.ToString());
                        }
                    }
                    else if (command[0] == '=' && command.Length > 2)
                    {
                        expression = new Expression(command.Substring(2));
                        expression.Read(int32OperatorHandler, out status);
                        if (status.State == ExpressionEvaluationStatus.StateEnum.FormatError)
                        {
                            expression = null;
                            outputWriter.WriteLine(status.ToString());
                        }
                        else
                        {
                            expression.Read(doubleOperatorHandler, out status);
                            if (status.State == ExpressionEvaluationStatus.StateEnum.FormatError)
                            {
                                expression = null;
                                outputWriter.WriteLine(status.ToString());
                            }
                        }
                    }
                    else
                    {
                        expression = null;
                        outputWriter.WriteLine("Format Error");
                    }
                }
            }

            inputReader.Dispose();
            outputWriter.Dispose();
        }
    }
}
