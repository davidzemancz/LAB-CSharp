using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public interface ITextWriter : IDisposable
    {
        public void WriteLine();

        public void WriteLine(string line);
    }

}
