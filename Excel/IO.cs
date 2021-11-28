using System;
using System.IO;

namespace Excel
{
    #region Input

    public interface IInputReader : IDisposable
    {
        public void Open();

        public string ReadLine();
    }

    public class ConsoleInputReader : IInputReader
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
      
        public void Open()
        {

        }

        public void Dispose()
        {

        }
    }

    public class FileInputReader : IInputReader, IDisposable
    {
        private StreamReader _streamReader;

        private string _fileName;

        public FileInputReader(string filename)
        {
            _fileName = filename;
        }

        public string ReadLine()
        {
            return this._streamReader.ReadLine();
        }

        public void Open()
        {
            this._streamReader = new StreamReader(_fileName);
        }

        public void Dispose()
        {
            this._streamReader.Dispose();
        }
    }

    #endregion

    #region Output

    public interface IOutputWriter : IDisposable
    {
        public void Open();

        public void Write(string value);

        public void WriteLine(string line);
    }

    public class ConsoleOutputWriter : IOutputWriter
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void Write(string value)
        {
            Console.Write(value);
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class FileOutputWriter : IOutputWriter, IDisposable
    {
        private StreamWriter _streamWriter;

        private string _fileName;

        public FileOutputWriter(string filename)
        {
            _fileName = filename;
        }

        public void Write(string value)
        {
            this._streamWriter.Write(value);
        }

        public void WriteLine(string line)
        {
            this._streamWriter.WriteLine(line);
        }

        public void Open()
        {
            this._streamWriter = new StreamWriter(_fileName);
        }

        public void Dispose()
        {
            this._streamWriter.Dispose();
        }
    }

    #endregion

}
