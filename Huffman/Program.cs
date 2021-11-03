using System;
using System.Collections.Generic;
using System.IO;

namespace Huffman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool test = true;
            if (test)
            {
               args = new string[] { @"C:\Users\david.zeman\Downloads\Model.cs" };
            }

            if (ValidateArguments(args))
            {
                try
                {
                    string filename = args[0];
                    long[] frequencies = new long[byte.MaxValue];
                    using (FileStream fileStream = new FileStream(filename, FileMode.Open))
                    {
                        while (true)
                        {
                            int i = fileStream.ReadByte();
                            if (i == -1) break;
                            frequencies[(byte)i] += 1;
                        }
                    }
                }
                catch
                {
                    if (test) throw;
                    Console.WriteLine("File Error");
                }
            }
            else
            {
                Console.WriteLine("Argument Error");
            }
        }

        /// <summary>
        /// Checks validity of input arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns>true if all arguments are valid, otherwise false</returns>
        static bool ValidateArguments(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                return false;
            }
            return true;
        }
    }
}
