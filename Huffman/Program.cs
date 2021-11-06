using System;
using System.Collections.Generic;
using System.IO;

namespace Huffman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool test = false;
            if (test)
            {
               args = new string[] { @"C:\Users\david.zeman\Downloads\huffman-data\binary.in" };
            }

            if (ValidateArguments(args))
            {
                try
                {
                    // Count frequencies
                    string filename = args[0];
                    long[] frequencies = new long[byte.MaxValue + 1];
                    using (FileStream fileStream = new FileStream(filename, FileMode.Open))
                    {
                        while (true)
                        {
                            int b = fileStream.ReadByte();
                            if (b == -1) break;
                            frequencies[(byte)b] += 1;
                        }
                    }

                    // Create forest of one node binary trees
                    List<HuffmanBinaryTree> forest = new List<HuffmanBinaryTree>();
                    for (int b = 0; b < frequencies.Length; b++)
                    {
                        if (frequencies[b] > 0)
                        {
                            forest.Add(new HuffmanBinaryTree() { Root = new HuffmanBinaryTreeNode() { Byte = (byte)b, Frequency = frequencies[b] } });
                        }
                    }

                    // Sort forest trees using RootFrequencyByteComparer
                    HuffmanBinaryTree.RootFrequencyByteComparer comparer = new HuffmanBinaryTree.RootFrequencyByteComparer();
                    forest.Sort(comparer);

                    // Build one tree
                    int timeStamp = 0;
                    while (forest.Count > 1)
                    {
                        // Get two with smallest value
                        HuffmanBinaryTree treeLeft = forest[0];
                        HuffmanBinaryTree treeRight = forest[1];

                        // Remove them from forest
                        forest.RemoveRange(0, 2);

                        // Merge
                        HuffmanBinaryTree newTree = new HuffmanBinaryTree() { TimeStamp = ++timeStamp };
                        newTree.Root = new HuffmanBinaryTreeNode() { Frequency = treeLeft.Root.Frequency + treeRight.Root.Frequency };
                        newTree.Root.Left = treeLeft.Root;
                        newTree.Root.Right = treeRight.Root;

                        // Insert into forest
                        int i = forest.BinarySearch(newTree, comparer);
                        if (i < 0) i = ~i;
                        forest.Insert(i, newTree);
                    }

                    // Write tree to output
                    if (forest.Count == 1)
                    {
                        Console.WriteLine(forest[0].ToString());
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
