using System;
using System.Collections.Generic;

namespace ExprEval
{
    internal class Program
    {
        private enum OperatorArityEnum
        {
            None = 0,
            Unary = 1,
            Binary = 2,
        }

        private static bool CharIsOperator(char c, out OperatorArityEnum arity)
        {
            arity = OperatorArityEnum.Unary;
            bool isUnary = c == '~';
            bool isBinary = c == '+' || c == '-' || c == '*' || c == '/';

            if (isUnary) arity = OperatorArityEnum.Unary;
            else if (isBinary) arity = OperatorArityEnum.Binary;

            return isUnary || isBinary;
        }

        private static void Main(string[] args)
        {
            try
            {
                string expr = Console.ReadLine();
                Stack<int> stack = new Stack<int>();

                for (int j = expr.Length - 1; j >= 0; j--)
                {
                    char c = expr[j];

                    if (CharIsOperator(c, out OperatorArityEnum arity))
                    {
                        if (arity == OperatorArityEnum.Unary)
                        {
                            int num1 = stack.Pop();
                            switch (c)
                            {
                                case '~':
                                    stack.Push(checked(-num1));
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
                                    stack.Push(checked(num1 + num2));
                                    break;
                                case '-':
                                    stack.Push(checked(num1 - num2));
                                    break;
                                case '*':
                                    stack.Push(checked(num1 * num2));
                                    break;
                                case '/':
                                    stack.Push(checked(num1 / num2));
                                    break;
                            }
                        }
                    }
                    else
                    {
                        string buffer = "";
                        while (j >= 0 && expr[j] != ' ')
                        {
                            buffer = expr[j] + buffer;
                            j--;
                        }
                        if (buffer != "")
                        {
                            try
                            {
                                stack.Push(int.Parse(buffer));
                            }
                            catch (OverflowException ex)
                            {
                                throw new FormatException("Invalid expression format", ex);
                            }
                        }
                    }
                }

                if (stack.Count == 1)
                {
                    Console.WriteLine(stack.Peek());
                }
                else throw new FormatException("Invalid expression format");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Overflow Error");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Divide Error");
            }
            catch
            {
                Console.WriteLine("Format Error");
            }
        }
    }
}
