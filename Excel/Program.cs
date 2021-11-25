using System;
using System.IO;

namespace Excel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IOutputWriter outputWriterConsole = new ConsoleOutputWriter();

            if (args != null && args.Length == 2)
            {
                // Read args
                string inFilename = args[0];
                string outFilename = args[1];

                try
                {
                    // IO
                    IInputReader inputReaderFile = new FileInputReader(inFilename);
                    IOutputWriter outputWriterFile = new FileOutputWriter(outFilename);
                    ExcelIO excelIO = new ExcelIO(inputReaderFile, outputWriterFile);

                    Sheet sheet = excelIO.ReadSheet();

                    foreach(Cell cell in sheet.Cells)
                    {
                        Console.WriteLine($"{cell.Adress} = {cell.Value}");
                    }
                }
                catch
                {
                    // Unexpected error
                    outputWriterConsole.WriteLine("File Error");
                }
            }
            else
            {
                outputWriterConsole.WriteLine("Argument Error");
            }
        }
    }
}
