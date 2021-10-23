using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class StreamReaderEx : StreamReader, ITextReader
    {
        public string NewLine { get; set; }

        public StreamReaderEx(string path) : base(path)
        {

        }

        public StreamReaderEx(string path, string newLine) : base(path) 
        { 
            this.NewLine = newLine;
        }

        public string ReadWord()
        {
            return this.ReadWord(out bool newParagraph);
        }

        public string ReadWord(out bool newParagraph)
        {
            newParagraph = false;
            bool whitespacesOnly = false;
            StringBuilder wordSb = new StringBuilder();
            while (wordSb.Length == 0)
            {
                while (!this.EndOfStream)
                {
                    char c = (char)this.Read();
                    if (char.IsWhiteSpace(c))
                    {
                        if (c == this.NewLine[0] && (whitespacesOnly || wordSb.Length == 0)) newParagraph = true;
                        whitespacesOnly = true;
                        break;
                    }
                    else whitespacesOnly = false;
                    wordSb.Append(c);
                }
                if (this.EndOfStream) break;
            }
            return wordSb.ToString();
        }

        public string ReadLine(out bool newParagraph)
        {
            throw new NotImplementedException();
        }

    }

    public class StreamWriterEx : StreamWriter, ITextWriter
    {
        public StreamWriterEx(string path) : base(path)
        {
        }
    }
}
