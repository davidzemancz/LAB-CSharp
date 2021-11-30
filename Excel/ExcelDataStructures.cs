using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Excel
{

    /// <summary>
    /// Sheet with cells
    /// </summary>
    public class Sheet
    {
        public Dictionary<string, Cell> CellsByAdress { get; }

        public string Name { get; set; }

        public Sheet()
        {
            CellsByAdress = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Get cell from sheet by adress
        /// </summary>
        /// <param name="adress">Cell adress</param>
        /// <returns>Cell or null if cell does not exists</returns>
        public Cell GetCell(string adress)
        {
            return CellsByAdress?.ContainsKey(adress) ?? false ? CellsByAdress[adress] : null;
        }

        /// <summary>
        /// Add cell to sheet
        /// </summary>
        /// <param name="cell">Cell</param>
        public void AddCell(Cell cell)
        {
            CellsByAdress.Add(cell.Adress, cell);
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

        public string Adress { get; }

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

        #endregion

        #region FIELDS

        private ErrorTypeEnum _errorType;

        private bool _isEvaluating;

        #endregion

        #region CONSTRUCTORS

        public Cell(int row, int column)
        {
            AdressRow = row;
            AdressColumn = column;
            Adress = GetAdressString();
        }

        #endregion

        #region PUBLIC METHODS

        public void Evaluate(Sheet sheet)
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
            else if (Value is string valueStr && valueStr.Length > 0 && valueStr[0] == '=') // Expression
            {
                char op = ' ';
                bool readingDigits = false;
                string adress1 = null, adress2 = null;
                StringBuilder adressBuilder = new StringBuilder();
                for (int i = 1; i < valueStr.Length; i++)
                {
                    char c = valueStr[i];
                    if (op == ' ' && (c == '+' || c == '-' || c == '*' || c == '/'))
                    {
                        op = c;
                        adress1 = adressBuilder.ToString();
                        adressBuilder.Clear();
                        readingDigits = false;
                    }
                    else
                    {
                        if (!readingDigits)
                        {
                            if (!CharIsValidLetter(c))
                            {
                                if (CharIsValidDigit(c) && adressBuilder.Length > 0)
                                {
                                    readingDigits = true;
                                }
                                else
                                {
                                    ErrorType = ErrorTypeEnum.InvalidFormula;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (!CharIsValidDigit(c))
                            {
                                ErrorType = ErrorTypeEnum.InvalidFormula;
                                break;
                            }
                        }
                        
                        adressBuilder.Append(c);
                    }
                }
                adress2 = adressBuilder.ToString();

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
                    Cell cell1 = sheet.GetCell(adress1);
                    Cell cell2 = sheet.GetCell(adress2);

                    if (cell1 == null) cell1 = Empty;
                    if (cell2 == null) cell2 = Empty;

                    bool cell1Evaluated = cell1.IsEvaluated;

                    if (!cell1Evaluated) cell1.Evaluate(sheet);
                    if (cell1.ErrorType > 0)
                    {
                        if (!cell1Evaluated && cell1.ErrorType == ErrorTypeEnum.Cycle) ErrorType = ErrorTypeEnum.Cycle;
                        else ErrorType = ErrorTypeEnum.Error;
                    }
                    else
                    {
                        bool cell2Evaluated = cell2.IsEvaluated;

                        if (!cell2Evaluated) cell2.Evaluate(sheet);
                        if (cell2.ErrorType > 0)
                        {
                            if (!cell2Evaluated && cell2.ErrorType == ErrorTypeEnum.Cycle) ErrorType = ErrorTypeEnum.Cycle;
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

            _isEvaluating = false;
            IsEvaluated = true;
        }

        #endregion

        #region PRIVATE METHODS

        private string GetAdressString()
        {
            StringBuilder builder = new StringBuilder();
            int col = AdressColumn + 1;
            while (col > 0)
            {
                int r = (col - 1) % LettersInAlphabet;
                builder.Append(Alphabet[r]);
                col = (col - r) / LettersInAlphabet;
            }
            builder = ReverseStringBuilder(builder);

            return builder.Append(AdressRow + 1).ToString();
        }

        private StringBuilder ReverseStringBuilder(StringBuilder sb)
        {
            char t;
            int end = sb.Length - 1;
            int start = 0;

            while (end - start > 0)
            {
                t = sb[end];
                sb[end] = sb[start];
                sb[start] = t;
                start++;
                end--;
            }
            return sb;
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
