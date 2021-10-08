using System;
using System.Diagnostics;
using System.IO;

namespace LAB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (Debugger.IsAttached && args?.Length == 0)
            {
                args = PocitaniSlov.GetArgs();
            }

            PocitaniSlov.Run(args);
        }
    }
}
