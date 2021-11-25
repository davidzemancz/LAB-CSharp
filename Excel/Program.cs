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
                    context.Sheets.Add(sheet.Name, sheet);



                    // Print cells for DEBUGGING
                    foreach (Cell cell in sheet.Cells)
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
