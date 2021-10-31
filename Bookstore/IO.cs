using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore
{
    #region Input

    public interface IInputReader
    {
        public string ReadLine();
    }

    public class ConsoleInputReader : IInputReader
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }

    public class FileInputReader : IInputReader, IDisposable
    {
        private StreamReader _streamReader;

        public FileInputReader(StreamReader streamReader)
        {
            this._streamReader = streamReader;
        }

        public string ReadLine()
        {
            return this._streamReader.EndOfStream ? null : this._streamReader.ReadLine();
        }

        public void Dispose()
        {
            this._streamReader.Dispose();
        }
    }

    #endregion

    #region Output

    public interface IOutputWriter
    {
        public void WriteLine(string line);
    }

    public class ConsoleOutputWriter : IOutputWriter
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }

    public class FileOutputWriter : IOutputWriter, IDisposable
    {
        private StreamWriter _streamWriter;

        public FileOutputWriter(StreamWriter streamWriter)
        {
            this._streamWriter = streamWriter;
        }

        public void WriteLine(string line)
        {
            this._streamWriter.WriteLine(line);
        }

        public void Dispose()
        {
            this._streamWriter.Dispose();
        }
    }

    #endregion

}
