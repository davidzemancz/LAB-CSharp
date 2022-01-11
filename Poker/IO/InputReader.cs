using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public interface IInputReader : IDisposable
    {
        public bool AtEnd { get; }

        public IInputReader Open();

        public string ReadLine();
    }

    public class FileInputReader : IInputReader, IDisposable
    {
        private StreamReader _streamReader;

        private string _fileName;

        public bool AtEnd => _streamReader.EndOfStream;

        public FileInputReader(string filename)
        {
            _fileName = filename;
        }

        public string ReadLine()
        {
            return _streamReader.ReadLine();
        }

        public IInputReader Open()
        {
            _streamReader = new StreamReader(_fileName);
            return this;
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }
    }

    public class ConsoleInputReader : IInputReader
    {
        public bool AtEnd => Console.In.Peek() == -1;

        public void Dispose()
        {
        }

        public IInputReader Open()
        {
            return this;
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }


}
