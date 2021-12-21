using System;
using System.Collections.Generic;

namespace ExprEval
{
    public class ExpressionEvaluationStatus
    {
        public StateEnum State { get; set; }

        public ExpressionEvaluationStatus(StateEnum state)
        {
            State = state;
        }

        public enum StateEnum
        {
            Ok = 0,
            FormatError = 1,
            DivideByZero = 2,
            OverflowError = 3,
        }

        public override string ToString()
        {
            switch (State)
            {
                case StateEnum.Ok: return "";
                case StateEnum.FormatError: return "Format Error";
                case StateEnum.DivideByZero: return "Divide Error";
                case StateEnum.OverflowError: return "Overflow Error";
                default: return "";
            }
        }
    }

    /// <summary>
    /// Class for working with numerical expressions
    /// </summary>
    public class Expression
    {
        /// <summary>
        /// Expression raw text
        /// </summary>
        public string Text { get; protected set; }

        #region CONSTRUCTORS

        public Expression(string text)
        {
            this.Text = text;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Evaluate integer expression with safty checks for division by zero, formatting etc...If error occurrs, returns null and status.
        /// </summary>
        /// <param name="status">Status</param>
        /// <returns>Expression result or null, if error occurrs.</returns>
        public int? EvaluateIntSafe(out ExpressionEvaluationStatus status)
        {
            status = new ExpressionEvaluationStatus(ExpressionEvaluationStatus.StateEnum.Ok);
            Stack<int> stack = new Stack<int>();
            try
            {
                for (int i = Text.Length - 1; i >= 0; i--)
                {
                    char c = Text[i];

                    if (CharIsOperator(c, out OperatorArityEnum arity))
                    {
                        int result = 0;
                        if (arity == OperatorArityEnum.Unary)
                        {
                            int num1 = stack.Pop();
                            switch (c)
                            {
                                case '~':
                                    result = checked(-num1);
                                    break;
                            }
                        }
                        else if (arity == OperatorArityEnum.Binary)
                        {
                            int num1 = stack.Pop();
                            int num2 = stack.Pop();
                            switch (c)
                            {
                                case '+':
                                    result = checked(num1 + num2);
                                    break;
                                case '-':
                                    result = checked(num1 - num2);
                                    break;
                                case '*':
                                    result = checked(num1 * num2);
                                    break;
                                case '/':
                                    if (num2 == 0)
                                    {
                                        status.State = ExpressionEvaluationStatus.StateEnum.DivideByZero;
                                        return null;
                                    }
                                    result = checked(num1 / num2);
                                    break;
                            }
                        }
                        stack.Push(result);
                    }
                    else
                    {
                        int? num = ReadInt(ref i, ref status);
                        if (status.State != ExpressionEvaluationStatus.StateEnum.Ok)
                        {
                            return null;
                        }
                        else if (num != null)
                        {
                            stack.Push(num.Value);
                        }
                    }
                }
            }
            catch (OverflowException)
            {
                status.State = ExpressionEvaluationStatus.StateEnum.OverflowError;
                return null;
            }
            catch
            {
                throw;
            }

            if (stack.Count == 1)
            {
                return stack.Peek();
            }
            else 
            {
                status.State = ExpressionEvaluationStatus.StateEnum.FormatError;
                return null;
            }
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Check if char is operator and return its arity
        /// </summary>
        /// <param name="c">Character</param>
        /// <param name="arity">Operator arity</param>
        /// <returns>true if valid operator; otherwise false</returns>
        private bool CharIsOperator(char c, out OperatorArityEnum arity)
        {
            arity = OperatorArityEnum.Unary;
            bool isUnary = c == '~';
            bool isBinary = c == '+' || c == '-' || c == '*' || c == '/';

            if (isUnary) arity = OperatorArityEnum.Unary;
            else if (isBinary) arity = OperatorArityEnum.Binary;

            return isUnary || isBinary;
        }

        /// <summary>
        /// Reads int from Text starting at position i
        /// </summary>
        /// <param name="i">Position in Text</param>
        /// <returns>Read integer</returns>
        /// <exception cref="FormatException">If value in Text is to large for Int32</exception>
        private int? ReadInt(ref int i, ref ExpressionEvaluationStatus status)
        {
            string buffer = "";
            while (i >= 0 && Text[i] != ' ')
            {
                buffer = Text[i] + buffer;
                i--;
            }
            if (buffer != "")
            {
                try
                {
                    return int.Parse(buffer);
                }
                catch (OverflowException)
                {
                    status.State = ExpressionEvaluationStatus.StateEnum.FormatError;
                }
            }
            return null;
        }

        #endregion

        #region ENUMS

        /// <summary>
        /// Operator arity
        /// </summary>
        private enum OperatorArityEnum
        {
            None = 0,
            Unary = 1,
            Binary = 2,
        }

        #endregion
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                string expression = Console.ReadLine();
                int? result = new Expression(expression).EvaluateIntSafe(out ExpressionEvaluationStatus status);
                if (status.State == ExpressionEvaluationStatus.StateEnum.Ok) Console.WriteLine(result);
                else Console.WriteLine(status.ToString());
            }
            catch
            {
                Console.WriteLine("Format Error");
            }
        }
    }
}
