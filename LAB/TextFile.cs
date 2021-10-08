using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    internal class TextFile
    {
        public const string METHOD_COUNTWORDS = "CountWords";

        /// <summary>
        /// Chars separating individual words
        /// </summary>
        public readonly char[] WhiteChars = new[] { ' ', '\t', '\n' };

        /// <summary>
        /// Returns test arguments for specific method
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <returns>String array of arguments</returns>
        public string[] GetTestArgs(string methodName)
        {
            switch (methodName)
            {
                case METHOD_COUNTWORDS: 
                    return new string[] { "lab1_testfile.txt" };
            }
            return new string[0];
        }

        /// <summary>
        /// Counts words in text file.
        /// </summary>
        /// <param name="filename">Full file name</param>
        /// <param name="err">Error text</param>
        /// <returns>Count of words in text file.</returns>
        public int CountWords(string filename, out string err)
        {
            err = "";
            int words = 0;
            try
            {
                if (string.IsNullOrEmpty(filename))
                {
                    err = "Argument Error";
                }
                else
                {
                    using (StreamReader streamReader = new StreamReader(filename))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            string line = streamReader.ReadLine().Trim();
                            int lineWords = line.Split(this.WhiteChars, StringSplitOptions.RemoveEmptyEntries).Length;
                            words += lineWords;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                err = "File Error";
            }
            return words;
        }

        /// <summary>
        /// Runs method in console enviroment
        /// </summary>
        /// <param name="args">Console args</param>
        /// <param name="methodName">Method to be called</param>
        public static void Run(string[] args, string methodName)
        {
            TextFile textFile = new TextFile(); 
            if (Debugger.IsAttached && args?.Length == 0)
            {
                args = textFile.GetTestArgs(methodName);
            }

            switch (methodName)
            {
                case METHOD_COUNTWORDS:
                    int words = textFile.CountWords(args[0], out string err);
                    if (!string.IsNullOrEmpty(err)) Console.WriteLine(err);
                    else Console.WriteLine(words);
                    break;
            }
        }
    }
}
