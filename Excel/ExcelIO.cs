using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel
{
    public class ExcelIO
    {
        public IInputReader InputReader { get; set; }

        public IOutputWriter OutputWriter { get; set; }

        public ExcelIO()
        {

        }

        public ExcelIO(IInputReader inputReader, IOutputWriter outputWriter)
        {
            this.InputReader = inputReader;
            this.OutputWriter = outputWriter;
        }

        /// <summary>
        /// Reads sheet from file
        /// </summary>
        /// <returns></returns>
        public Sheet ReadSheet()
        {
            Sheet sheet = new Sheet();
            this.InputReader.Open();
            using (var reader = this.InputReader)
            {
                int row = 0;
                List<Cell> rowCells;
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    string[] rowCellsStr = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    rowCells = new List<Cell>();
                    for (int col = 0; col < rowCellsStr.Length; col++)
                    {
                        rowCells.Add(new Cell(row, col) { Value = rowCellsStr[col]});   
                    }
                    sheet.AddRow(rowCells.ToArray());
                    row++;
                }
            }
            return sheet;
        }

        /// <summary>
        /// Evaluates all sheet cells in context and writes sheet to file
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="context"></param>
        public void EvaluateAndWriteSheet(Sheet sheet, Context context)
        {
            this.OutputWriter.Open();
            using (var writer = this.OutputWriter)
            {
                foreach (var row in sheet.Cells)
                {
                    for (int col = 0; col < row.Length ; col++)
                    {
                        Cell cell = row[col];

                        // Evaluate cell
                        if(!cell.IsEvaluated) cell.Evaluate(context);

                        writer.Write(cell.Value.ToString());
                        if (col < row.Length - 1) writer.Write(" ");
                    }
                    writer.WriteLine("");
                    
                    //Console.WriteLine($"{cell.Adress} = {cell.Value}");
                }
            }
        }
    }
}
