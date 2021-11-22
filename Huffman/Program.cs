using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*/
            Test();
            return;
            /**/

            // IO
            IInputReader inputFileReader = null;
            IOutputWriter outputWriterFile = null;
            IOutputWriter outputWriterConsole = new ConsoleOutputWriter();

            if (args != null && args.Length == 1)
            {
                // Read args
                string inFilename = args[0];
                string outFilename = inFilename + ".huff";

                try
                {
                    // ------ Build huffman tree ------
                    // IO
                    inputFileReader = new FileInputReader(new StreamReader(inFilename));
                    outputWriterFile = new FileOutputWriter(new StreamWriter(outFilename));

                    // Count frequencies
                    long[] frequencies = new long[byte.MaxValue + 1];
                    while (true)
                    {
                        int i = inputFileReader.ReadByte();
                        if (i == -1) break;
                        frequencies[i] += 1;
                    }

                    // Build tree
                    HuffmanBinaryTree tree = new HuffmanBinaryTree();
                    tree.Build(frequencies);

                    // Write tree to output
                    if (tree.Root != null)
                    {
                        // Write tree structure to console
                        outputWriterConsole.WriteLine(tree.ToString());

                        // Write encoded tree to file
                        foreach (byte b in tree.ToBytes()) 
                        {
                            outputWriterFile.WriteByte(b);
                        }
                    }

                    // ------ Data coding ------
                    // IO 
                    inputFileReader = new FileInputReader(new StreamReader(inFilename));
                    
                    // Buffer
                    List<bool> buffer = new List<bool>();

                    // Read bytes from file
                    bool end = false;
                    while (!end)
                    {
                        int i = inputFileReader.ReadByte();
                        // End of file
                        if (i == -1)
                        {
                            end = true;

                            // If at end of stream ... append zeros to whole byte if any
                            if (buffer.Count > 0)
                            {
                                for (int k = buffer.Count - 1; k < 8; k++) buffer.Add(false);
                            }
                        }
                        else
                        {
                            // Find leaf by character
                            HuffmanBinaryTreeNode leaf = tree.Leaves[(byte)i];

                            // Push BitPath to leaf to buffer
                            for (int j = 0; j < leaf.BitPathLength; j++) buffer.Add(leaf.BitPath[j]);
                        }
                       

                        // If there is at least one byte in buffer
                        while (buffer.Count >= 8)
                        {

                            // Push bits to byte
                            BitArray bitArr = new BitArray(8);
                            for (int k = 0; k < 8; k++)
                            {
                                bitArr[k] = buffer[k];
                            }
                            byte[] bytes = new byte[1];
                            bitArr.CopyTo(bytes, 0);

                            // Remove from buffer
                            buffer.RemoveRange(0, 8);

                            // Write byte to file
                            outputWriterFile.WriteByte(bytes[0]);
                        }
                    }
                }
                catch
                {
                    // Unexpected error
                    outputWriterConsole.WriteLine("File Error");
                }
                finally
                {
                    // Dispose all
                    if (inputFileReader is IDisposable disposable1) disposable1.Dispose();
                    if (outputWriterFile is IDisposable disposable2) disposable2.Dispose();
                    if (outputWriterConsole is IDisposable disposable3) disposable3.Dispose();
                }
            }
            else
            {
                outputWriterConsole.WriteLine("Argument Error");
            }
        }

        static void Test()
        {
            long freq = 1;//Convert.ToInt64(Math.Pow(2, 54));
            byte ch = 1;

            BitArray nodeBits = new BitArray(64);
            nodeBits[0] = true; // 1 indicates leaf

            BitArray frequencyBits = new BitArray(BitConverter.GetBytes(freq));
            for (int i = 0, j = 1; j <= 55; i++, j++) nodeBits[j] = frequencyBits[i];

            BitArray charBits = new BitArray(BitConverter.GetBytes(ch));
            for (int i = 0, j = 56; j <= 63; i++, j++) nodeBits[j] = charBits[i];

            string bitsStr = "";
            foreach (bool b in nodeBits) { bitsStr += b ? "1" : "0"; }
            Console.WriteLine(bitsStr);
        }
    }
}
