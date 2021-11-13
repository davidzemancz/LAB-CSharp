using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    public class HuffmanBinaryTreeNode
    {
        public HuffmanBinaryTreeNode Left { get; set; }

        public HuffmanBinaryTreeNode Right { get; set; }


        public byte Byte { get; set; }

        public long Frequency { get; set; }

        public bool IsLeaf => this.Left == null && this.Right == null;

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[8];

            if (this.IsLeaf)
            {
                
            }
            else
            {
                // bit[0] = 0
                long f = this.Frequency & 0;
            }

            return bytes.Concat(this.Left.ToBytes()).Concat(this.Right.ToBytes()).ToArray();
        }

        public override string ToString()
        {
            if (this.IsLeaf) return $"*{this.Byte}:{this.Frequency}";
            else return $"{this.Frequency} {this.Left} {this.Right}";
        }
    }


    public class HuffmanBinaryTree
    {
        public int TimeStamp { get; set; }

        public HuffmanBinaryTreeNode Root { get; set; }

        public void Build(long[] frequencies)
        {
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

            if (forest.Count == 0) this.Root = null; 
            else this.Root = forest[0].Root;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(this.HeaderBytes());
            bytes.AddRange(this.Root.ToBytes());
            bytes.AddRange(this.FooterBytes());

            return bytes.ToArray();
        }

        public override string ToString()
        {
            return this.Root?.ToString();
        }

        private byte[] HeaderBytes()
        {
            return new byte[] { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };
        }

        private byte[] FooterBytes()
        {
            return new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        public class RootFrequencyByteComparer : IComparer<HuffmanBinaryTree>
        {
            public int Compare(HuffmanBinaryTree x, HuffmanBinaryTree y)
            {
                if (x.Root.Frequency == y.Root.Frequency)
                {
                    if (x.Root.IsLeaf && y.Root.IsLeaf) return x.Root.Byte - y.Root.Byte;
                    else if (!x.Root.IsLeaf && y.Root.IsLeaf) return 1;
                    else if (x.Root.IsLeaf && !y.Root.IsLeaf) return -1;
                    else return x.TimeStamp - y.TimeStamp;
                }
                else
                {
                    return (int)(x.Root.Frequency - y.Root.Frequency);
                }
            }
        }


       
    }
}
