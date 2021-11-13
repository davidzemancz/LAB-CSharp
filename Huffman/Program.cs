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
            IOutputWriter outputWriterFile = null;
            IOutputWriter outputWriterConsole = new ConsoleOutputWriter();

            if (args != null && args.Length == 1)
            {
                // Read args
                string inFilename = args[0];
                string outFilename = inFilename + ".huff";

                try
                {
                    // IO
                    inputReader = new FileInputReader(new StreamReader(inFilename));
                    outputWriterFile = new FileOutputWriter(new StreamWriter(outFilename));

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
                        outputWriterFile.WriteLine(tree.ToString());
                    }
                }
                catch
                {
                    outputWriterConsole.WriteLine("File Error");
                }
                finally
                {
                    if (inputReader is IDisposable disposable1) disposable1.Dispose();
                    if (outputWriterFile is IDisposable disposable2) disposable2.Dispose();
                }
            }
            else
            {
                outputWriterConsole.WriteLine("Argument Error");
            }
        }
    }
}
