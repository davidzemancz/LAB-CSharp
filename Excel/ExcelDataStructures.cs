using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel
{

    /// <summary>
    /// Excel application context
    /// </summary>
    public class Context
    {
        protected Dictionary<string, Sheet> _sheets;

        /// <summary>
        /// Add sheet to context
        /// </summary>
        /// <param name="sheet">Sheet</param>
        public void AddSheet(Sheet sheet)
        {
            _sheets.Add(sheet.Name, sheet);
        }

        /// <summary>
        /// Get cell from context by adress
        /// </summary>
        /// <param name="adress">Cell adress</param>
        /// <returns>Cell or null if cell does not exists</returns>
        public Cell GetCell(string adress)
        {
            Sheet sheet = _sheets.FirstOrDefault().Value;
            return sheet?.Cells?.Exists(adress) ?? false ? sheet.Cells[adress] : null;
        }

        public Context()
        {
            _sheets = new Dictionary<string, Sheet>();
        }
    }

    /// <summary>
    /// Sheet with cells
    /// </summary>
    public class Sheet
    {
        public string Name { get; set; }

        public CellCollection Cells { get; }

        public Sheet()
        {
            Cells = new CellCollection();
        }
    }

    /// <summary>
    /// Collection of cells
    /// </summary>
    public class CellCollection : IEnumerable<Cell>
    {
        protected Dictionary<string, Cell> CellsByString { get; }

        /// <summary>
        /// Get or set cell by adress
        /// </summary>
        /// <param name="adress">Adress</param>
        /// <returns></returns>
        public Cell this[string adress]
        {
            get => CellsByString[adress];
        }

        public CellCollection()
        {
            this.CellsByString = new Dictionary<string, Cell>();
        }


        /// <summary>
        /// Adds new cell to collection.
        /// </summary>
        /// <param name="cell">Cell</param>
        public void Add(Cell cell)
        {
            CellsByString[cell.Adress.ToString()] = cell;
        }

        /// <summary>
        /// Checks if cell exists in collection.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>True if cell exists in collection, otherwise false.</returns>
        public bool Exists(string adress)
        {
            return CellsByString.ContainsKey(adress);
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return CellsByString.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Represents one cell with its value and another metadata
    /// </summary>
    public class Cell
    {
        #region CONSTS

        public const string EmptyCellValue = "[]";

        #endregion

        #region PROPS

        public Adress Adress { get; }

        public object Value { get; set; }

        public ErrorTypeEnum ErrorType
        {
            get => _errorType;
            set
            {
                _errorType = value;
                if (IsError) Value = ErrorTypeEnumToString(ErrorType);
            }
        }

        public bool IsError => (int)this.ErrorType > 0;

        public bool IsEvaluated { get; protected set; }

        #endregion

        #region FIELDS

        private ErrorTypeEnum _errorType;

        private bool _isEvaluating;

        #endregion

        #region CONSTRUCTORS

        public Cell (Adress adress)
        {
            this.Adress = adress;
        }

        #endregion

        #region PUBLIC METHODS

        public void Evaluate(Context context)
        {
            // If is already evaluating -> cycle
            if (_isEvaluating)
            {
                ErrorType = ErrorTypeEnum.Cycle;
                return;
            }
            else if (IsEvaluated)
            {
                return;
            }

            _isEvaluating = true;

            string valueStr = Value.ToString();
            if (valueStr == EmptyCellValue) // Empty cell
            {
                // pass
            }
            else if (Value is int) // Valid integer cell
            {
                // pass
            }
            else if (int.TryParse(valueStr, out int valueInt)) // Cell with numeric string value
            {
                Value = valueInt;
            }
            else if (valueStr.Length > 0 && valueStr[0] == '=') // Expression
            {
                // Check operator
                if (valueStr.IndexOfAny("+-*/".ToCharArray()) == -1)
                {
                    ErrorType = ErrorTypeEnum.MissingOperator;
                }
                else
                { 
                    try
                    {
                        (string adress1, string adress2, string op) = ReadExpression(valueStr);
                        if (op != null)
                        {
                            Cell cell1 = context.GetCell(adress1);
                            Cell cell2 = context.GetCell(adress2);

                            if (cell1 != null && cell2 != null)
                            {
                                // Evaluate cell1
                                if (!cell1.IsEvaluated) cell1.Evaluate(context);
                                if (cell1.IsError)
                                {
                                    if (cell1.ErrorType == ErrorTypeEnum.Cycle)
                                    {
                                        ErrorType = ErrorTypeEnum.Cycle;
                                    }
                                    else
                                    {
                                        ErrorType = ErrorTypeEnum.Error;
                                    }
                                }
                                else // cell1 is ok
                                {
                                    // Evaluate cell2
                                    if (!cell2.IsEvaluated) cell2.Evaluate(context);
                                    if (cell2.IsError)
                                    {
                                        if (cell2.ErrorType == ErrorTypeEnum.Cycle)
                                        {
                                            ErrorType = ErrorTypeEnum.Cycle;
                                        }
                                        else
                                        {
                                            ErrorType = ErrorTypeEnum.Error;
                                        }
                                    }
                                    else // cell2 is ok
                                    {
                                        try
                                        {
                                            object cell1Value = cell1.Value;
                                            object cell2Value = cell2.Value;

                                            if (cell1Value is string && (string)cell1Value == EmptyCellValue) cell1Value = 0;
                                            if (cell2Value is string && (string)cell2Value == EmptyCellValue) cell2Value = 0;

                                            switch (op)
                                            {
                                                case "+":
                                                    this.Value = (int)cell1Value + (int)cell2Value;
                                                    break;
                                                case "-":
                                                    this.Value = (int)cell1Value - (int)cell2Value;
                                                    break;
                                                case "*":
                                                    this.Value = (int)cell1Value * (int)cell2Value;
                                                    break;
                                                case "/":
                                                    if ((int)cell2Value == 0)
                                                    {
                                                        ErrorType = ErrorTypeEnum.DivisionByZero;
                                                    }
                                                    else
                                                    {
                                                        this.Value = (int)cell1Value / (int)cell2Value;
                                                    }
                                                    break;
                                                default:
                                                    ErrorType = ErrorTypeEnum.InvalidFormula;
                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            ErrorType = ErrorTypeEnum.Error;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ErrorType = ErrorTypeEnum.InvalidFormula;
                            }
                        }
                        else
                        {
                            ErrorType = ErrorTypeEnum.MissingOperator;
                        }
                    }
                    catch
                    {
                        ErrorType = ErrorTypeEnum.InvalidFormula;
                    }
            }
            }
            else // Invalid value
            {
                ErrorType = ErrorTypeEnum.InvalidValue;
            }

            _isEvaluating = false;
            IsEvaluated = true;
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Reads expression
        /// </summary>
        /// <param name="expression">String expression</param>
        /// <returns>Expression splitted in parts</returns>
        private (string adress1, string adress2, string op) ReadExpression(string expression)
        {
            string adressStr1 = null, adressStr2 = null; 
            string op = null;

            // Read expression
            for (int i = 1; i < expression.Length; i++) // Starting at index 1, expression[0] = '='
            {
                if (expression[i] == '+' || expression[i] == '-' || expression[i] == '*' || expression[i] == '/')
                {
                    op = expression[i].ToString();
                }
                else if (op == null) 
                { 
                    adressStr1 += expression[i];
                }
                else
                {
                    adressStr2 += expression[i];
                }
            }

            return (adressStr1, adressStr2, op);
        }

        #endregion

        #region ErrorTypeEnum

        public enum ErrorTypeEnum
        {
            None = 0,
            Error = 1,
            DivisionByZero = 2,
            Cycle = 3,
            MissingOperator = 4,
            InvalidFormula = 5,
            InvalidValue = 6,
        }

        public static string ErrorTypeEnumToString(ErrorTypeEnum value)
        {
            switch (value)
            {
                case ErrorTypeEnum.None: return "";
                case ErrorTypeEnum.Error: return "#ERROR";
                case ErrorTypeEnum.DivisionByZero: return "#DIV0";
                case ErrorTypeEnum.Cycle: return "#CYCLE";
                case ErrorTypeEnum.MissingOperator: return "#MISSOP";
                case ErrorTypeEnum.InvalidFormula: return "#FORMULA";
                case ErrorTypeEnum.InvalidValue: return "#INVVAL";
                default: return "";
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents adress
    /// </summary>
    public class Adress
    {
        private const int lettersInAlphabet = 26;

        public uint Row { get; }

        public uint Column { get; }

        public Adress(uint row, uint column)
        {
            this.Row = row;
            this.Column = column;
        }

        public Adress (string adress)
        {
            string colStr = null, rowStr = null;
            uint row = 0, col = 0;

            // Read col and row adress to strings
            for (int i = 0; i < adress.Length; i++)
            {
                if (char.IsLetter(adress[i]) && rowStr == null)
                {
                    colStr += char.ToLower(adress[i]);
                }
                else if (char.IsNumber(adress[i]) && colStr != null)
                {
                    rowStr += adress[i];
                }
                else
                {
                    throw new ArgumentException("Invalid adress format");
                }
            }

            // Parse row adress
            row = uint.Parse(rowStr);

            // Parse column adress
            int power = colStr.Length - 1;
            for (int i = 0; i < colStr.Length; i++)
            {
                uint letterVal = (uint)(colStr[i] - ('a' - 1));
                col += letterVal * (uint)Math.Pow(lettersInAlphabet, power--);
            }

            // Indexing from 0
            row -= 1;
            col -= 1;

            this.Row = row;
            this.Column = col;
        }

        /// <summary>
        /// Converts adress to ulong format
        /// </summary>
        /// <returns>Ulong adress format</returns>
        public ulong ToUlong()
        {
            ulong l = ((ulong)this.Row << 33) + this.Column;
            return l;
        }

        /// <summary>
        /// Converts adress to string format
        /// </summary>
        /// <returns>String adress format</returns>
        public override string ToString()
        {
            string colStr = "", rowStr = "";

            // Column string
            uint col = this.Column;
            while (true)
            {
                uint q = col / lettersInAlphabet; 
                if (q > 0)
                {
                    colStr += char.ToUpper((char)(q + 96));
                    col -= q * lettersInAlphabet;
                }
                else
                {
                    colStr += char.ToUpper((char)(col + 97));
                    break;
                }
            }


            // Row string
            rowStr = (this.Row + 1).ToString();

            return colStr + rowStr;
        }

    }
}
