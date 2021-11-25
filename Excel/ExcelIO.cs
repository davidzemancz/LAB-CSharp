using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        sheet.Cells.Add(new Cell(new Adress(row, col)) { Value = rowCells[col]});
                    }

                    row++;
                }
            }
            return sheet;
        }
    }
}
