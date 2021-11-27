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

        public readonly static char[] Operators = new char[] { '+', '-', '*', '/' };

        public readonly static Cell Empty = new Cell(0, 0) { Value = 0, IsEvaluated = true };

        #endregion

        #region PROPS

        public uint AdressRow { get; }

        public uint AdressColumn { get; }

        public string Adress { get; }

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

        public Cell(uint row, uint column)
        {
            AdressRow = row;
            AdressColumn = column;
            Adress = GetAdressString();
        }

        #endregion

        #region PUBLIC METHODS

        public void Evaluate(Context context)
        {
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

            if (Value is string && (string)Value == EmptyCellValue)
            {
                // pass
            }
            else if (Value is int)
            {
                // pass
            }
            else if (!(Value is int) && int.TryParse(Value.ToString(), out int intValue)) // Convert value to int
            {
                this.Value = intValue;
            }
            else if (Value is string && ((string)Value).Length > 0 && ((string)Value)[0] == '=') // Expression
            {
                string exprStr = ((string)Value).Substring(1);
                string[] exprArr = exprStr.Split(Operators);
                if (exprArr.Length < 2)
                {
                    ErrorType = ErrorTypeEnum.MissingOperator;
                }
                else if (exprArr.Length > 2)
                {
                    ErrorType = ErrorTypeEnum.InvalidFormula;
                }
                else
                {
                    string adress1 = exprArr[0];
                    string adress2 = exprArr[1];
                    if (adress1.Length == 0 || !char.IsLetter(adress1, 0) || !char.IsUpper(adress1, 0) || !char.IsNumber(adress1, adress1.Length - 1)
                     || adress2.Length == 0 || !char.IsLetter(adress2, 0) || !char.IsUpper(adress2, 0) || !char.IsNumber(adress2, adress2.Length - 1))
                    {
                        ErrorType = ErrorTypeEnum.InvalidFormula;
                    }
                    else
                    {
                        char op = exprStr[exprArr[0].Length];
                        Cell cell1 = context.GetCell(adress1);
                        Cell cell2 = context.GetCell(adress2);

                        if (cell1 == null) cell1 = Empty;
                        if (cell2 == null) cell2 = Empty;

                        if (!cell1.IsEvaluated) cell1.Evaluate(context);
                        if (cell1.IsError)
                        {
                            if (cell1.ErrorType == ErrorTypeEnum.Cycle) ErrorType = ErrorTypeEnum.Cycle;
                            else ErrorType = ErrorTypeEnum.Error;
                        }
                        else
                        { 
                            if (!cell2.IsEvaluated) cell2.Evaluate(context);
                            if (cell2.IsError)
                            {
                                if (cell2.ErrorType == ErrorTypeEnum.Cycle) ErrorType = ErrorTypeEnum.Cycle;
                                else ErrorType = ErrorTypeEnum.Error;
                            }
                            else
                            {
                                int cell1Int = cell1.Value is string ? 0 : (int)cell1.Value;
                                int cell2Int = cell2.Value is string ? 0 : (int)cell2.Value;

                                switch (op)
                                {
                                    case '+':
                                        Value = cell1Int + cell2Int;
                                        break;
                                    case '-':
                                        Value = cell1Int - cell2Int;
                                        break;
                                    case '*':
                                        Value = cell1Int * cell2Int;
                                        break;
                                    case '/':
                                        if (cell2Int == 0) ErrorType = ErrorTypeEnum.DivisionByZero;
                                        else Value = cell1Int / cell2Int;
                                        break;
                                    default:
                                        ErrorType = ErrorTypeEnum.InvalidFormula;
                                        break;
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                this.ErrorType = ErrorTypeEnum.InvalidValue;
            }

            _isEvaluating = false;
            IsEvaluated = true;
        }

        #endregion

        #region PRIVATE METHODS

        private string GetAdressString()
        {
            const int lettersInAlphabet = 26;

            string colStr = "", rowStr = "";

            // Column string
            uint col = AdressColumn;
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
            rowStr = (AdressRow + 1).ToString();

            return colStr + rowStr;
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
}
