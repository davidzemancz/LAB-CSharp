﻿using System;
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

        public Sheet ReadSheet()
        {
            Sheet sheet = new Sheet();
            this.InputReader.Open();
            using (var reader = this.InputReader)
            {
                int row = 0;
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    string[] rowCells = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (int col = 0; col < rowCells.Length; col++)
                    {
                        sheet.AddCell(new Cell(row, col) { Value = rowCells[col]});
                    }

                    row++;
                }
            }
            return sheet;
        }

        public void EvaluateAndWriteSheet(Sheet sheet, Context context)
        {
            this.OutputWriter.Open();
            using (var writer = this.OutputWriter)
            {
                int lineIndex = -1;
                foreach (var kvp in sheet.CellsByAdress)
                {
                    Cell cell = kvp.Value;

                    // Evaluate cell
                    cell.Evaluate(context);

                    // Write cell to file
                    if (cell.AdressRow > lineIndex)
                    {
                        lineIndex++;

                        if(lineIndex > 0) writer.WriteLine("");
                        writer.Write(cell.Value?.ToString());
                    }
                    else
                    {
                        writer.Write(" ");
                        writer.Write(cell.Value?.ToString());
                    }
                    //Console.WriteLine($"{cell.Adress} = {cell.Value}");
                }
            }
        }
    }
}
