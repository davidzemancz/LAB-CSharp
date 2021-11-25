using System;
using System.Net.Sockets;

namespace _
{
    internal class Program
    {
        class a
        {
            public int x;

            public int this[int i]
            {
                get => 0;
            }

            public void set_Item(int i)
            {

            }
        }

        static void Main(string[] args)
        {
           a a = new a { x = 1 };
        }
    }
}
