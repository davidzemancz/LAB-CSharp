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

        public override string ToString()
        {
            return this.Root?.ToString();
        }
    }
}
