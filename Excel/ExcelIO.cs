using System;
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
                int lineIndex = -1;
                var keys = sheet.CellsById.Keys.ToList();
                keys.Sort();
                foreach (var key in keys)
                {
                    Cell cell = sheet.CellsById[key];

                    // Evaluate cell
                    cell.Evaluate(sheet);

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
