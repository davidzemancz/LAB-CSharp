using System;
using System.Collections;
using System.Globalization;

namespace Graphs
{
    internal class M
    {
        internal static long Pow(int b, int e)
        {
            long ret = 1;
            for(int i = 0; i < e; i++)
            {
                ret *= b;
            }
            return ret;
        }
    }

    internal class Program
    {
        static readonly NumberFormatInfo numberFormat = new NumberFormatInfo() { NumberGroupSeparator = " " };

        static int[] vertices;

        static Tuple<int, int>[] edges;

        static void InitGraph(int verticesCount)
        {
            // Init vertices
            vertices = new int[verticesCount];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = i;
            }

            // Init edges
            int edgesCount = (vertices.Length * (vertices.Length - 1) / 2);
            edges = new Tuple<int, int>[edgesCount];
            int e = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    edges[e++] = new Tuple<int, int>(i, j);
                }
            }
        }

        static void FindCycleInColorings(int cycleLength)
        {
            long coloringsCount = M.Pow(2, edges.Length);

            Console.WriteLine($"Colorings count: {coloringsCount.ToString(numberFormat)}");
            for (long i = 0; i < coloringsCount; i++)
            {
                // Create coloring
                int size = 0;
                BitArray bitmap = new BitArray(BitConverter.GetBytes(i));
                for (int j = 0; j < bitmap.Length && j < edges.Length; j++)
                {
                    if (bitmap[j]) size++;
                }

                Tuple<int, int>[] coloring = new Tuple<int, int>[size];
                Tuple<int, int>[] coloringSupplement = new Tuple<int, int>[edges.Length - size];
                int c = 0, s = 0;
                for (int j = 0; j < bitmap.Length && j < edges.Length; j++)
                {
                    if (bitmap[j])
                    {
                        coloring[c++] = edges[j];
                    }
                    else
                    {
                        coloringSupplement[s++] = edges[j];
                    }
                }

                if (i % 1_000_000 == 0) Console.WriteLine($"Colorings computed: {i.ToString(numberFormat)}");

                // Find cycle

            }
        }


        static void Main(string[] args)
        {
            int verticesCount = 8;
            int cycleLength = 4;

            InitGraph(verticesCount);
            FindCycleInColorings(cycleLength);

        }
    }
}
