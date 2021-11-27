using System;
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
                uint row = 0;
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    string[] rowCells = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (uint col = 0; col < rowCells.Length; col++)
                    {
                        sheet.AddCell(new Cell(row, col) { Value = rowCells[col]});
                    }

                    row++;
                }
            }
            return sheet;
        }

        public void WriteSheet(Sheet sheet)
        {
            this.OutputWriter.Open();
            using (var writer = this.OutputWriter)
            {
                int lineIndex = 0;
                StringBuilder lineBuilder = new StringBuilder();
                foreach (var kvp in sheet.Cells)
                {
                    Cell cell = kvp.Value;

                    // Evaluate cell
                    cell.Evaluate(sheet);

                    // Write cell to file
                    if (cell.AdressRow > lineIndex)
                    {
                        // Remove last space
                        if (lineBuilder.Length > 0) lineBuilder.Remove(lineBuilder.Length - 1, 1);

                        lineIndex++;
                        writer.WriteLine(lineBuilder.ToString());

                        lineBuilder.Clear();
                    }
                    
                    lineBuilder.Append(cell.Value);
                    lineBuilder.Append(' ');
                }

                if (lineBuilder.Length > 0)
                {
                    lineBuilder.Remove(lineBuilder.Length - 1, 1);
                    writer.WriteLine(lineBuilder.ToString());
                }

            }
        }
    }
}
