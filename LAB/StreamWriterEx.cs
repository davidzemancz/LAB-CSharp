using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LAB
{
    public class StreamWriterEx : StreamWriter, ITextWriter
    {
        public StreamWriterEx(string path) : base(path)
        {
        }
    }
}
