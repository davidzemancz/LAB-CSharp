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

        /// <summary>
        /// Character in leaf
        /// </summary>
        public byte Character { get; set; }

        /// <summary>
        /// Frequency of character
        /// </summary>
        public long Frequency { get; set; }

        /// <summary>
        /// Bit path to node where 0 means 'go left' and 1 means 'go right'. It is fixed length of 16, use this.BitPathLength to find out valid bits
        /// </summary>
        public BitArray BitPath { get; set; }

        /// <summary>
        /// Length of this.BitPath
        /// </summary>
        public int BitPathLength { get; set; }

        /// <summary>
        /// True if node is leaf, otherwise false
        /// </summary>
        public bool IsLeaf => this.Left == null && this.Right == null;

        public HuffmanBinaryTreeNode()
        {
            this.BitPath = new BitArray(16);
        }

        /// <summary>
        /// Encodes node to prefix notation in specific format.
        /// <para>    
        ///     Inner node:
        ///         bit 0: 0 indicates inner node
        ///         bits 1-55: last 55 bits of node frequency
        ///         bits 56-63: all 0
        /// </para>
        /// <para>
        ///    Leaf:
        ///         bit 0: 1 indicates leaf
        ///         bits 1-55: last 55 bits of node frequency
        ///         bits 56-63: character in leaf
        /// </para>
        /// </summary>
        /// <returns>Array of bytes</returns>
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

        /// <summary>
        /// Encodes node to prefix notaion
        /// <para>    
        ///     Inner node: $"{this.Frequency} {this.Left} {this.Right}"
        /// </para>
        /// <para>
        ///    Leaf: $"*{this.Character}:{this.Frequency}"
        /// </para>
        /// </summary>
        /// <returns>String represenation of node</returns>
        public override string ToString()
        {
            // Just for debugging
            string bitsStr = "";
            for(int i = 0; i < this.BitPathLength; i++) { bitsStr += this.BitPath[i] ? "1" : "0"; }

            if (this.IsLeaf) return $"*{this.Character}:{this.Frequency}";
            else return $"{this.Frequency} {this.Left} {this.Right}";
        }
    }


    public class HuffmanBinaryTree
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        public int TimeStamp { get; set; }

        /// <summary>
        /// Root node
        /// </summary>
        public HuffmanBinaryTreeNode Root { get; set; }

        /// <summary>
        /// Leaves where key is leaf character
        /// </summary>
        public Dictionary<byte, HuffmanBinaryTreeNode> Leaves { get; protected set; }

        /// <summary>
        /// Build Huffman tree
        /// </summary>
        /// <param name="frequencies">Frequencies of characters</param>
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

        /// <summary>
        /// Ecnodes tree (basiclly the root) to bytes in prexif notation. Adds header to the begin and footer to the end
        /// </summary>
        /// <returns>Array of bytes</returns>
        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(this.HeaderBytes());
            bytes.AddRange(this.Root.ToBytes());
            bytes.AddRange(this.FooterBytes());

            return bytes.ToArray();
        }

        /// <summary>
        /// Encodes tree (basiclly the root) to prefix notaion
        /// </summary>
        /// <returns>String represenation of tree</returns>
        public override string ToString()
        {
            return this.Root?.ToString();
        }

        /// <summary>
        /// Builds Bitpaths to all nodes where 0 means 'go left' and 1 means 'go right'
        /// </summary>
        /// <param name="node"></param>
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

        /// <summary>
        /// Returns fixed header bytes
        /// </summary>
        /// <returns></returns>
        private byte[] HeaderBytes()
        {
            return new byte[] { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };
        }

        /// <summary>
        /// Returns fixed footer bytes
        /// </summary>
        /// <returns></returns>
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
