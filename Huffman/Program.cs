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
               args = new string[] { @"C:\Users\david.zeman\Downloads\test.in" };
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
                            int b = fileStream.ReadByte();
                            if (b == -1) break;
                            frequencies[(byte)b] += 1;
                        }
                    }
                    Array.Sort(frequencies);
                    WriteFrequencies(frequencies);
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

        /// <summary>
        /// Writes array to standart output
        /// </summary>
        static void WriteFrequencies(long[] frequencies)
        {
            Console.Write("[");
            for (int b = 0; b < frequencies.Length; b++)
            {
                if(frequencies[b] > 0)
                {
                    Console.Write($" *{b}:{frequencies[b]} ");
                }
            }
            Console.WriteLine("]");
        }
    }
}
