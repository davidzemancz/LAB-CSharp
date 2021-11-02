using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore
{
    public class Program
    {
        public static class Commands
        {
            public const string DATA_BEGIN = "DATA-BEGIN";
            public const string GET = "GET";
            public const string DATA_END = "DATA-END";
        }

        public static void Main(string[] args)
        {
            bool fileIO = false;

            IInputReader inputReader;
            IOutputWriter outputWriter;
            IDataSource dataSource = new LocalDataSource();

            if (fileIO)
            {
                inputReader = new FileInputReader(new StreamReader(@"C:\Users\david.zeman\Downloads\Example\NezarkaTest.in"));
                outputWriter = new FileOutputWriter(new StreamWriter(@"C:\Users\david.zeman\Downloads\Example\NezarkaTestDZ.out"));
            }
            else
            {
                inputReader = new ConsoleInputReader();
                outputWriter = new ConsoleOutputWriter();
            }

            try
            {
                while (true)
                {
                    string line = inputReader.ReadLine();
                    if (line == null) break;
                    string[] lineParts = line.Split(' ');

                    IService service = null;
                    ServiceResult serviceResult = null;

                    if (line == Commands.DATA_BEGIN)
                    {
                        service = new CsvDataImporterService(inputReader, dataSource);
                        serviceResult = service.Run(line);
                    }
                    else if (lineParts[0] == Commands.GET && lineParts.Length == 3)
                    {
                        service = new HtmlDataProviderService(outputWriter, dataSource);
                        serviceResult = service.Run(line);
                        outputWriter.WriteLine("====");
                    }
                    else
                    {
                        HtmlDataProviderService.WriteInvalidRequest(outputWriter);
                        outputWriter.WriteLine("====");
                    }

                    if (serviceResult != null && !serviceResult.Success)
                    {
                        outputWriter.WriteLine(serviceResult.Message);
                        break;
                    }
                }
            }
            catch
            {
                HtmlDataProviderService.WriteInvalidRequest(outputWriter);
            }

            if (inputReader is IDisposable disposable1) disposable1.Dispose();
            if (outputWriter is IDisposable disposable2) disposable2.Dispose();   
        }
    }
}
