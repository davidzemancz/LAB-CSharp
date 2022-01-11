using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{

    public interface IOutputWriter : IDisposable
    {
        public IOutputWriter Open();

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

        public IOutputWriter Open()
        {
            return this;
        }

        public void Dispose()
        {
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

        public IOutputWriter Open()
        {
            this._streamWriter = new StreamWriter(_fileName);
            return this;
        }

        public void Dispose()
        {
            this._streamWriter.Dispose();
        }
    }

}
