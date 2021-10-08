using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    internal class PocitaniSlov
    {
        public static string[] GetArgs()
        {
            return new string[] { "lab1_testfile.txt" };
        }

        public static void Run(string[] args)
        {
            if (args?.Length == 0)
            {
                Console.WriteLine("Argument Error");
                return;
            }
            try
            {
                int words = 0;
                char[] whiteChars = new[] { ' ', '\t', '\n' };
                string filename = args[0];

                using (StreamReader streamReader = new StreamReader(filename))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine().Trim();
                        int lineWords = line.Split(whiteChars, StringSplitOptions.RemoveEmptyEntries).Length;
                        words += lineWords;
                    }
                }

                Console.WriteLine(words);
            }
            catch (Exception ex)
            {
                Console.WriteLine("File Error");
            }
        }
    }
}
