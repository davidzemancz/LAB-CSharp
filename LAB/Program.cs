using System;
using System.Diagnostics;
using System.IO;

namespace LAB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TextFile.Run(args, TextFile.METHOD_COUNTWORDS);
        }
    }
}
