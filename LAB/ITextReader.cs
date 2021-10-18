using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public interface ITextReader : IDisposable
    {
        public bool EndOfStream { get; }

        public int Read();

        public string ReadWord();

        public string ReadWord(out bool newParagraph);

        public string ReadLine();

        public string ReadLine(out bool newParagraph);
    }
}
