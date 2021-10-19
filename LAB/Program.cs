using System;
using System.Diagnostics;
using System.IO;

namespace LAB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TextProcessorHelper.RunAlignContent(args);
        }
    }

    internal class TextProcessorHelper
    {
        public static void RunAlignContent(string[] args)
        {
            // ----- Init vars -----
            string err;
            string inputFile;
            string outputFile;
            int maxLineLength;

            // ----- Validate and prepare args -----
            if (args.Length < 3 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]) || !int.TryParse(args[2], out maxLineLength))
            {
                Console.WriteLine(TextProcessor.ERROR_ARGUMENT);
                return;
            }
            inputFile = args[0];
            outputFile = args[1];

            // ----- Run text processor -----
            try
            {
                using (TextProcessor textProcessor = new TextProcessor(new StreamReaderEx(inputFile), new StreamWriterEx(outputFile), TextProcessor.LF))
                {
                    textProcessor.AlignContent(maxLineLength, out err);
                    if (!string.IsNullOrEmpty(err)) Console.WriteLine(err);
                }
            }
            catch
            {
                Console.WriteLine(TextProcessor.ERROR_FILE);
            }
        }
    }
}
