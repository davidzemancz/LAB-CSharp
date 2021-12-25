using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public interface ExpressionOperatorHandler
    {
    }

    public interface ExpressionOperatorHandler<T> : ExpressionOperatorHandler
    {
        public T Minus(T x, bool isChecked);

        public bool IsZeroDivisionCheck(T x);

        public bool TryParse(string value, out T num);

        public T Add(T x, T y, bool isChecked);

        public T Subtract(T x, T y, bool isChecked);

        public T Multiply(T x, T y, bool isChecked);

        public T Divide(T x, T y, bool isChecked);
    }

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

        public Dictionary<ExpressionOperatorHandler, List<object>> Parts { get; set; }

        #region CONSTRUCTORS

        public Expression(string text)
        {
            this.Text = text;
            this.Parts = new Dictionary<ExpressionOperatorHandler, List<object>>();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Evaluate integer expression with safty checks for division by zero, formatting etc...If error occurrs, returns null and status.
        /// </summary>
        /// <param name="status">Status</param>
        /// <returns>Expression result or null, if error occurrs.</returns>
        public T Evaluate<T>(ExpressionOperatorHandler<T> operatonHandler, out ExpressionEvaluationStatus status)
        {
            status = new ExpressionEvaluationStatus(ExpressionEvaluationStatus.StateEnum.Ok);
            Stack<T> stack = new Stack<T>();
            try
            {
                foreach (object part in this.Parts[operatonHandler])
                {
                    if (part is char c && CharIsOperator(c, out OperatorArityEnum arity))
                    {
                        T result = default(T);
                        if (arity == OperatorArityEnum.Unary)
                        {
                            T num1 = stack.Pop();
                            switch (c)
                            {
                                case '~':
                                    result = operatonHandler.Minus(num1, true);
                                    break;
                            }
                        }
                        else if (arity == OperatorArityEnum.Binary)
                        {
                            T num1 = stack.Pop();
                            T num2 = stack.Pop();
                            switch (c)
                            {
                                case '+':
                                    result = operatonHandler.Add(num1, num2, true);
                                    break;
                                case '-':
                                    result = operatonHandler.Subtract(num1, num2, true);
                                    break;
                                case '*':
                                    result = operatonHandler.Multiply(num1, num2, true);
                                    break;
                                case '/':
                                    if (operatonHandler.IsZeroDivisionCheck(num2))
                                    {
                                        status.State = ExpressionEvaluationStatus.StateEnum.DivideByZero;
                                        return default(T);
                                    }
                                    result = operatonHandler.Divide(num1, num2, true);
                                    break;
                            }
                        }
                        stack.Push(result);
                    }
                    else
                    {
                        stack.Push((T)part);
                    }
                }
            }
            catch (OverflowException)
            {
                status.State = ExpressionEvaluationStatus.StateEnum.OverflowError;
                return default(T);
            }
            catch (Exception ex)
            {
                status.State = ExpressionEvaluationStatus.StateEnum.FormatError;
                return default(T);
            }

            if (stack.Count == 1)
            {
                return stack.Peek();
            }
            else
            {
                status.State = ExpressionEvaluationStatus.StateEnum.FormatError;
                return default(T);
            }
        }

        /// <summary>
        /// Evaluate integer expression with safty checks for division by zero, formatting etc...If error occurrs, returns null and status.
        /// </summary>
        /// <param name="status">Status</param>
        /// <returns>Expression result or null, if error occurrs.</returns>
        public void Read<T>(ExpressionOperatorHandler<T> operatonHandler, out ExpressionEvaluationStatus status)
        {
            this.Parts[operatonHandler] = new List<object>();

            status = new ExpressionEvaluationStatus(ExpressionEvaluationStatus.StateEnum.Ok);
            Stack<T> stack = new Stack<T>();
            try
            {
                for (int i = Text.Length - 1; i >= 0; i--)
                {
                    char c = Text[i];

                    if (CharIsOperator(c, out OperatorArityEnum arity))
                    {
                        T result = default(T);
                        if (arity == OperatorArityEnum.Unary)
                        {
                            T num1 = stack.Pop();
                        }
                        else if (arity == OperatorArityEnum.Binary)
                        {
                            T num1 = stack.Pop();
                            T num2 = stack.Pop();
                        }
                        stack.Push(result);
                        this.Parts[operatonHandler].Add(c);
                    }
                    else
                    {
                        string numStr = ReadWord(ref i);
                        if (numStr != "")
                        {
                            if (operatonHandler.TryParse(numStr, out T val))
                            {
                                stack.Push(val);
                                this.Parts[operatonHandler].Add(val);
                            }
                            else
                            {
                                status.State = ExpressionEvaluationStatus.StateEnum.FormatError;
                                return;
                            }
                        }
                    }
                }
            }
            catch
            {
                status.State = ExpressionEvaluationStatus.StateEnum.FormatError;
                return;
            }

            if (stack.Count == 1)
            {
                return;
            }
            else
            {
                status.State = ExpressionEvaluationStatus.StateEnum.FormatError;
                return;
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
        /// Reads word from Text starting at position i
        /// </summary>
        /// <param name="i">Position in Text</param>
        /// <returns>Read word</returns>
        private string ReadWord(ref int i)
        {
            string buffer = "";
            while (i >= 0 && Text[i] != ' ')
            {
                buffer = Text[i] + buffer;
                i--;
            }
            return buffer;
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

}
