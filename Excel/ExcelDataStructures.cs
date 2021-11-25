using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel
{
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
        /// Get or set cell by row index and column index
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <returns>Cell reference</returns>
        public Cell this[uint row, uint column]
        {
            get => Cells[Adress.GetId(row, column)];
            set => Cells[Adress.GetId(row, column)] = value;
        }

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
        public bool Exists(Cell cell)
        {
            return Cells.ContainsKey(cell.Adress.GetId());
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
        #region PROPS

        public Adress Adress { get; }

        public object Value { get; set; }

        public ErrorTypeEnum ErrorType { get; set; }

        public bool IsError => (int)this.ErrorType > 0;

        public string ErrorMessage => ErrorTypeEnumToString(this.ErrorType);

        #endregion

        #region CONSTRUCTORS

        public Cell (Adress adress)
        {
            this.Adress = adress;
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
            InvalidFormula = 5
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
                default: return "";
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents adress in sheet
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
