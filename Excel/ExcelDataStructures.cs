using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Excel
{
    /// <summary>
    /// Context
    /// </summary>
    public class Context
    {
        public Dictionary<string, Sheet> Sheets { get; set; }

        public Sheet PrimarySheet { get; set; }

        public Context (Sheet primarySheet)
        {
            Sheets = new Dictionary<string, Sheet>();
            Sheets.Add(primarySheet.Name, primarySheet);
            PrimarySheet = primarySheet;
        }

        /// <summary>
        /// Get cell from primary sheet by adress
        /// </summary>
        /// <param name="adress">Cell adress</param>
        /// <returns>Cell or null if cell does not exists</returns>
        public Cell GetCell((int row, int column) adress)
        {
            if (PrimarySheet.Cells.Count > adress.row && PrimarySheet.Cells[adress.row].Length > adress.column) return PrimarySheet.Cells[adress.row][adress.column];
            else return null;
        }
    }

    /// <summary>
    /// Sheet with cells
    /// </summary>
    public class Sheet
    {
        public List<Cell[]> Cells { get; }

        public string Name { get; set; }

        public Sheet()
        {
            Cells = new List<Cell[]>();
        }


        /// <summary>
        /// Add row to sheet
        /// </summary>
        /// <param name="cell">Cell</param>
        public void AddRow(Cell[] rowCells)
        {
            Cells.Add(rowCells);
        }

    }

    /// <summary>
    /// Represents one cell with its value and another metadata
    /// </summary>
    public class Cell
    {
        #region CONSTS

        private const int LettersInAlphabet = 26;

        public const string EmptyCellValue = "[]";

        public readonly static char[] Alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public readonly static Cell Empty = new Cell(0, 0) { Value = 0, IsEvaluated = true };


        #endregion

        #region PROPS

        public int AdressRow { get; }

        public int AdressColumn { get; }

        public object Value { get; set; }

        public ErrorTypeEnum ErrorType
        {
            get => _errorType;
            set
            {
                _errorType = value;
                if (ErrorType > 0) Value = ErrorTypeEnumToString(ErrorType);
            }
        }

        public bool IsEvaluated { get; protected set; }

        public bool IsEvaluating { get; protected set; }

        #endregion

        #region FIELDS

        private ErrorTypeEnum _errorType;

        #endregion

        #region CONSTRUCTORS

        public Cell(int row, int column)
        {
            AdressRow = row;
            AdressColumn = column;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Evaluate cell in specific context (sheet, range, multiple sheets ...)
        /// </summary>
        /// <param name="context">Context object</param>
        public void Evaluate(Context context)
        {
            IsEvaluating = true;

            if (Value is string && (string)Value == EmptyCellValue)
            {
                // pass
            }
            else if (Value is int)
            {
                // pass
            }
            else if (Value is string valueStr && valueStr.Length > 0 && valueStr[0] == '=') // Expression
            {
                (char op, Cell cell1, Cell cell2) = GetFormulaCells(valueStr, context);

                if (op == ' ')
                {
                    ErrorType = ErrorTypeEnum.MissingOperator;
                }
                else if (ErrorType == ErrorTypeEnum.InvalidFormula)
                {
                    // pass
                }
                else
                {
                    if (cell1.IsEvaluating)
                    {
                        ErrorType = ErrorTypeEnum.Cycle;
                    }
                    else 
                    { 
                        if (!cell1.IsEvaluated) cell1.Evaluate(context);
                        if (cell1.ErrorType > 0)
                        {
                            ErrorType = ErrorTypeEnum.Cycle;
                        }
                        else
                        {
                            if (cell2.IsEvaluating)
                            {
                                ErrorType = ErrorTypeEnum.Cycle;
                            }
                            else
                            {
                                if (!cell2.IsEvaluated) cell2.Evaluate(context);

                                if (cell2.ErrorType > 0)
                                {
                                    ErrorType = ErrorTypeEnum.Error;
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
            }
            else if (!(Value is int) && int.TryParse(Value.ToString(), out int intValue)) // Convert value to int
            {
                if (intValue < 0)
                {
                    ErrorType = ErrorTypeEnum.InvalidValue;
                }
                else
                {
                    this.Value = intValue;
                }
            }
            else
            {
                this.ErrorType = ErrorTypeEnum.InvalidValue;
            }

            IsEvaluating = false;
            IsEvaluated = true;
        }

        #endregion

        #region PRIVATE METHODS

        private (char op, Cell cell1, Cell cell2) GetFormulaCells(string valueStr, Context context)
        {
            char op = ' ';
            string adress1 = null, adress2 = null;
            for (int i = 1; i < valueStr.Length; i++)
            {
                char c = valueStr[i];
                if (op == ' ' && (c == '+' || c == '-' || c == '*' || c == '/'))
                {
                    op = c;
                    adress1 = valueStr.Substring(1, i - 1);
                    adress2 = valueStr.Substring(i + 1, valueStr.Length - (i + 1));
                }
            }

            var adress1Indexes = GetRowAndColumnIndex(adress1);
            var adress2Indexes = GetRowAndColumnIndex(adress2);
            if (adress1Indexes.row == -1 || adress2Indexes.row == -1)
            {
                ErrorType = ErrorTypeEnum.InvalidFormula;
            }
            else
            {
                Cell cell1 = context.GetCell(adress1Indexes);
                Cell cell2 = context.GetCell(adress2Indexes);

                if (cell1 == null) cell1 = Empty;
                if (cell2 == null) cell2 = Empty;

                return (op, cell1, cell2);
            }
            return (op, null, null);
        } 

        private (int row, int column) GetRowAndColumnIndex(string adress)
        {
            int row = 0, column = 0;
            if (adress == null) return (-1, -1);
            for (int i = 0; i < adress.Length; i++)
            {
                char c = adress[i];
                if (CharIsValidLetter(c))
                {
                    column *= LettersInAlphabet;
                    column += c - ('A' - 1);
                }
                else
                {
                    if (int.TryParse(adress.Substring(i, adress.Length - i), out row))
                    {
                        column -= 1;
                        row -= 1;
                    }
                    else
                    {
                        row = -1;
                        column = -1;
                    }
                    break;
                }
            }
            return (row, column);
        }

        private bool CharIsValidLetter(char c) => (64 < c && c < 91);

        private bool CharIsValidDigit(char c) => (47 < c && c < 58);

        #endregion

        #region ErrorTypeEnum

        public enum ErrorTypeEnum
        {
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
