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
        public Cell GetCell(Adress adress)
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
        protected Dictionary<ulong, Cell> Cells { get; }

        /// <summary>
        /// Get or set cell by adress
        /// </summary>
        /// <param name="adress">Adress</param>
        /// <returns></returns>
        public Cell this[Adress adress]
        {
            get => Cells[adress.GetId()];
            set => Cells[adress.GetId()] = value;
        }

        public CellCollection()
        {
            this.Cells = new Dictionary<ulong, Cell>();
        }

        /// <summary>
        /// Gets first cell in sheet
        /// </summary>
        /// <returns>First cell in sheet or null if not exists</returns>
        public Cell First()
        {
            return Cells.FirstOrDefault().Value;
        }

        /// <summary>
        /// Adds new cell to collection.
        /// </summary>
        /// <param name="cell">Cell</param>
        public void Add(Cell cell)
        {
            Cells.Add(cell.Adress.GetId(), cell);
        }

        /// <summary>
        /// Add new cell to collection or updates it, if it already exists.
        /// </summary>
        /// <param name="cell">Cell</param>
        public void AddOrUpdate(Cell cell)
        {
            Cells[cell.Adress.GetId()] = cell;
        }

        /// <summary>
        /// Remove cell from collection if exists.
        /// </summary>
        /// <param name="cell">Cell</param>
        public void Remove(Cell cell)
        {
            Cells.Remove(cell.Adress.GetId());
        }

        /// <summary>
        /// Checks if cell exists in collection.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>True if cell exists in collection, otherwise false.</returns>
        public bool Exists(Adress adress)
        {
            return Cells.ContainsKey(adress.GetId());
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return Cells.Values.GetEnumerator();
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
            else if (valueStr.StartsWith("=")) // Expression
            {
                // Check operator
                if (valueStr.IndexOfAny("+-*/".ToCharArray()) == -1)
                {
                    ErrorType = ErrorTypeEnum.MissingOperator;
                }
                else try
                {
                    (Adress adress1, Adress adress2, string op) = ReadExpression(valueStr);
                    if (op != null)
                    {
                        Cell cell1 = context.GetCell(adress1);
                        Cell cell2 = context.GetCell(adress2);

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

                                    if (cell1Value.ToString() == EmptyCellValue) cell1Value = 0;
                                    if (cell2Value.ToString() == EmptyCellValue) cell2Value = 0;

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
                                            this.Value = (int)cell1Value / (int)cell2Value;
                                            break;
                                        default:
                                            ErrorType = ErrorTypeEnum.InvalidFormula;
                                            break;
                                    }
                                }
                                catch (DivideByZeroException)
                                {
                                    ErrorType = ErrorTypeEnum.DivisionByZero;
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
                        ErrorType = ErrorTypeEnum.MissingOperator;
                    }
                }
                catch
                {
                    ErrorType = ErrorTypeEnum.InvalidFormula;
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
        private (Adress adress1, Adress adress2, string op) ReadExpression(string expression)
        {
            Adress adress1, adress2;
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

            // Create adresses
            adress1 = new Adress(adressStr1);
            adress2 = new Adress(adressStr2);

            return (adress1, adress2, op);
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
    public struct Adress
    {
        public uint Row { get; }

        public uint Column { get; }


        public Adress(uint row, uint column)
        {
            this.Row = row;
            this.Column = column;
        }

        public Adress(string adress)
        {
            const int lettersInAlphabet = 26;
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
        /// Gets cells unique id.
        /// </summary>
        /// <returns>Unique id</returns>
        public ulong GetId()
        {
            return Adress.GetId(this.Row, this.Column);
        }

        /// <summary>
        /// Gets cells unique id.
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <returns>Unique id</returns>
        public static ulong GetId(uint row, uint column)
        {
            ulong l = ((ulong)row << 33) + column;
            return l;
        }

        public override string ToString()
        {
            return $"[{Row},{Column}]";
        }

    }
}
