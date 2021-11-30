using System;

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

                    // Read sheet from file
                    Sheet sheet = excelIO.ReadSheet();

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
