using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class Reader : StreamReader
    {
        public Reader(string path) : base(path) { }

        public string ReadWord()
        {
            string word = null;
            List<char> buffer = new List<char>();
            while (!this.EndOfStream)
            {
                char c = (char)this.Read();
                if (char.IsWhiteSpace(c)) break;
                buffer.Add(c);
            }
            word = new string(buffer.ToArray());
            return word;
        }
    }
}
