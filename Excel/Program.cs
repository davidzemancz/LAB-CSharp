using System;
using System.IO;

namespace Excel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Context context = new Context();

            IOutputWriter outputWriterConsole = new ConsoleOutputWriter();

            if (args != null && args.Length == 2)
            {
                // Read args
                string inFilename = args[0];
                string inFilenameShort = Path.GetFileNameWithoutExtension(inFilename);
                string outFilename = args[1];

                try
                {
                    // IO
                    IInputReader inputReaderFile = new FileInputReader(inFilename);
                    IOutputWriter outputWriterFile = new FileOutputWriter(outFilename);
                    ExcelIO excelIO = new ExcelIO(inputReaderFile, outputWriterFile);

                    // Read sheet from file
                    Sheet sheet = excelIO.ReadSheet();
                    sheet.Name = inFilenameShort;

                    // Add sheet to context
                    context.AddSheet(sheet);

                    // Evaluate cell values in context
                    foreach (Cell cell in sheet.Cells)
                    {
                        cell.Evaluate(context);

                        // Print cells for DEBUGGING
                        Console.WriteLine($"{cell.Adress} = {cell.Value}");
                    }

                    // Write sheet to file
                    excelIO.WriteSheet(sheet);
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
