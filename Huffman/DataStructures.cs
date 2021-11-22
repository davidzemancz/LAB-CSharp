using System;
using System.Collections;
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

        public byte Character { get; set; }

        public long Frequency { get; set; }

        public BitArray BitPath { get; set; }

        public int BitPathLength { get; set; }

        public bool IsLeaf => this.Left == null && this.Right == null;

        public HuffmanBinaryTreeNode()
        {
            this.BitPath = new BitArray(16);
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[8];
            BitArray nodeBits = new BitArray(64);

            if (this.IsLeaf)
            {
                nodeBits[0] = true; // 1 indicates leaf
                BitArray frequencyBits = new BitArray(BitConverter.GetBytes(this.Frequency));
                for (int i = 0, j = 1; j <= 55; i++, j++) nodeBits[j] = frequencyBits[i];

                BitArray charBits = new BitArray(BitConverter.GetBytes(this.Character));
                for (int i = 0, j = 56; j <= 63; i++, j++) nodeBits[j] = charBits[i];

                nodeBits.CopyTo(bytes, 0);
                return bytes;
            }
            else
            {
                nodeBits[0] = false; // 0 indicates inner node
                BitArray frequencyBits = new BitArray(BitConverter.GetBytes(this.Frequency));
                for (int i = 0, j = 1; j <= 55; i++, j++) nodeBits[j] = frequencyBits[i];

                nodeBits.CopyTo(bytes, 0);
                return bytes.Concat(this.Left.ToBytes()).Concat(this.Right.ToBytes()).ToArray();
            }
        }

        public override string ToString()
        {
            string bitsStr = "";
            for(int i = 0; i < this.BitPathLength; i++) { bitsStr += this.BitPath[i] ? "1" : "0"; }

            if (this.IsLeaf) return $"*{this.Character}:{this.Frequency}:({bitsStr})";
            else return $"{this.Frequency} {this.Left} {this.Right}";
        }
    }


    public class HuffmanBinaryTree
    {
        public int TimeStamp { get; set; }

        public HuffmanBinaryTreeNode Root { get; set; }

        public Dictionary<byte, HuffmanBinaryTreeNode> Leaves { get; protected set; }

        public void Build(long[] frequencies)
        {
            // Create forest of one node binary trees from leafs
            List<HuffmanBinaryTree> forest = new List<HuffmanBinaryTree>();
            this.Leaves = new Dictionary<byte,HuffmanBinaryTreeNode>();
            for (int b = 0; b < frequencies.Length; b++)
            {
                if (frequencies[b] > 0)
                {
                    HuffmanBinaryTreeNode node = new HuffmanBinaryTreeNode() { Character = (byte)b, Frequency = frequencies[b] };
                    forest.Add(new HuffmanBinaryTree() { Root = node });
                    this.Leaves.Add(node.Character, node);
                }
            }

            // Sort forest trees using RootFrequencyByteComparer
            RootFrequencyByteComparer comparer = new RootFrequencyByteComparer();
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

                // Create new node
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

            // Build bitpaths to nodes, where 0~'go left' and 1~'go right'
            // Nodes with greater frequency are higher in tree, so theirs paths are shorter
            BuildBitpaths(this.Root);
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

        private void BuildBitpaths(HuffmanBinaryTreeNode node)
        {
            if(node == null) return;

            if (node.Left != null)
            {
                node.Left.BitPathLength = node.BitPathLength;
                for (int i = 0; i < node.BitPathLength; i++) node.Left.BitPath[i] = node.BitPath[i];
                node.Left.BitPath[node.Left.BitPathLength++] = false;
                BuildBitpaths(node.Left);
            }

            if (node.Right != null)
            {
                node.Right.BitPathLength = node.BitPathLength;
                for (int i = 0; i < node.BitPathLength; i++) node.Right.BitPath[i] = node.BitPath[i];
                node.Right.BitPath[node.Right.BitPathLength++] = true;
                BuildBitpaths(node.Right);
            }
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
                    if (x.Root.IsLeaf && y.Root.IsLeaf) return x.Root.Character - y.Root.Character;
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
