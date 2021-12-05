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
                List<Cell> rowCells = new List<Cell>();
                while (true)
                {
                    string word = reader.ReadWord(out bool newLine);
                    if (word == "")
                    {
                        if (rowCells.Count > 0)
                        {
                            sheet.AddRow(rowCells.ToArray());
                            rowCells.Clear();
                        }
                        break;
                    }

                    rowCells.Add(new Cell() { Value = word });

                    if (newLine)
                    {
                        sheet.AddRow(rowCells.ToArray());
                        rowCells.Clear();
                    }
                }
            }
            return sheet;
        }

        /// <summary>
        /// Evaluates all sheet cells in context and writes sheet to file
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="context"></param>
        public void EvaluateAndWriteSheet(Sheet sheet)
        {
            this.OutputWriter.Open();
            using (var writer = this.OutputWriter)
            {
                //for(int row = 0; row < sheet.Rows.Count; row++)
                for (int row = 0; row < sheet.Rows.Count; row++)
                {
                    for (int col = 0; col < sheet.Rows[row].Length ; col++)
                    {
                        // Evaluate cell
                        if(!sheet.Rows[row][col].IsEvaluated) sheet.Rows[row][col].EvaluateExpression(sheet);

                        writer.Write(sheet.Rows[row][col].Value.ToString());
                        if (col < sheet.Rows[row].Length - 1) writer.Write(" ");
                    }
                    writer.WriteLine("");
                    
                    //Console.WriteLine($"{cell.Adress} = {cell.Value}");
                }
            }
        }
    }
}
