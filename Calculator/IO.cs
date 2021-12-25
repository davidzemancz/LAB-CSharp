using System;
using System.IO;
using System.Text;

namespace Calculator
{
    #region Input

    public interface IInputReader : IDisposable
    {
        public void Open();

        public string ReadLine();

        public string ReadWord(out bool newLine);
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
            return _streamReader.ReadLine();
        }

        public void Open()
        {
            _streamReader = new StreamReader(_fileName);
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }

        public string ReadWord(out bool newLine)
        {
            newLine = false;
            string word = "";
            while (word.Length == 0)
            {
                while (!_streamReader.EndOfStream)
                {
                    char c = (char)_streamReader.Read();
                    if (c == '\r' || c == '\n')
                    {
                        newLine = true;
                        break;
                    }
                    else if(c == ' ')
                    {
                        break;
                    }
                    else
                    {
                        word += c;
                    }
                }
                if (_streamReader.EndOfStream) break;
            }
            return word.ToString();
        }
    }

    public class ConsoleInputReader : IInputReader
    {
        public void Dispose()
        {
        }

        public void Open()
        {
        }

        public int ReadByte()
        {
            return Console.In.Read();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public string ReadWord(out bool newLine)
        {
            throw new NotImplementedException();
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
