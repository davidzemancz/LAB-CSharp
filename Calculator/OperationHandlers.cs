using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class Int32OperatorHandler : ExpressionOperatorHandler<int>
    {
        public int Add(int x, int y, bool isChecked)
        {
            return isChecked ? checked(x + y) : x + y;
        }

        public int Divide(int x, int y, bool isChecked)
        {
            return isChecked ? checked(x / y) : x / y;
        }

        public bool IsZeroDivisionCheck(int x)
        {
            return x == 0;
        }

        public int Minus(int x, bool isChecked)
        {
            return isChecked ? checked(-x) : -x;
        }

        public int Multiply(int x, int y, bool isChecked)
        {
            return isChecked ? checked(x * y) : x * y;
        }

        public bool TryParse(string value, out int num)
        {
            return int.TryParse(value, out num);
        }

        public int Subtract(int x, int y, bool isChecked)
        {
            return isChecked ? checked(x - y) : x - y;
        }

        public int Parse(string value)
        {
            return int.Parse(value);
        }
    }

    public class DoubleOperatorHandler : ExpressionOperatorHandler<double>
    {
        public double Add(double x, double y, bool isChecked)
        {
            return isChecked ? checked(x + y) : x + y;
        }

        public double Divide(double x, double y, bool isChecked)
        {
            return isChecked ? checked(x / y) : x / y;
        }

        public bool IsZeroDivisionCheck(double x)
        {
            return false;
        }

        public double Minus(double x, bool isChecked)
        {
            return isChecked ? checked(-x) : -x;
        }

        public double Multiply(double x, double y, bool isChecked)
        {
            return isChecked ? checked(x * y) : x * y;
        }

        public bool TryParse(string value, out double num)
        {
            return double.TryParse(value, out num);
        }

        public double Subtract(double x, double y, bool isChecked)
        {
            return isChecked ? checked(x - y) : x - y;
        }

        public double Parse(string value)
        {
            return double.Parse(value);
        }
    }
}
