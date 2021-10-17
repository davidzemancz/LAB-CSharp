using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class StreamReaderEx : StreamReader
    {
        public StreamReaderEx(string path) : base(path) { }

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
                        if (c == '\n' && (whitespacesOnly || wordSb.Length == 0)) newParagraph = true;
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
    }
}
