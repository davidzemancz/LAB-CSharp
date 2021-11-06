using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // IO
            IInputReader inputReader = null;
            IOutputWriter outputWriter = new ConsoleOutputWriter(); ;

            if (args != null && args.Length == 1)
            {
                // Read args
                string filename = args[0];

                try
                {
                    // IO
                    inputReader = new FileInputReader(new StreamReader(filename));

                    // Count frequencies
                    long[] frequencies = new long[byte.MaxValue + 1];
                    while (true)
                    {
                        int b = inputReader.ReadByte();
                        if (b == -1) break;
                        frequencies[b] += 1;
                    }

                    // Build tree
                    HuffmanBinaryTree tree = new HuffmanBinaryTree();
                    tree.Build(frequencies);

                    // Write tree to output
                    if (tree.Root != null)
                    {
                       outputWriter.WriteLine(tree.ToString());
                    }
                }
                catch
                {
                    outputWriter.WriteLine("File Error");
                }
                finally
                {
                    if (inputReader is IDisposable disposable1) disposable1.Dispose();
                    if (outputWriter is IDisposable disposable2) disposable2.Dispose();
                }
            }
            else
            {
                outputWriter.WriteLine("Argument Error");
            }
        }
    }
}
