using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests")]

namespace LAB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string error = "Error!";

            try
            {
                int a = 0, b = 0;

                string line = Console.ReadLine();
                if (line == null) throw new Exception("No input");
                else if (!int.TryParse(line, out a)) throw new Exception("Invalid input");
                else if (a < 0) throw new Exception("Input less than zero");

                line = Console.ReadLine();
                if (line == null) throw new Exception("No input");
                else if (!int.TryParse(line, out b)) throw new Exception("Invalid input");
                else if (b < 0) throw new Exception("Input less than zero");

                int result = a - b;

                Console.WriteLine("Result: {0}", result);
            }
            catch (Exception)
            {
                Console.WriteLine(error);
            }
        }
    }
    /*
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
            if (args.Length != 3 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]) || !int.TryParse(args[2], out maxLineLength))
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
                    textProcessor.AlignContent(maxLineLength, false, out err);
                    if (!string.IsNullOrEmpty(err)) Console.WriteLine(err);
                }
            }
            catch
            {
                Console.WriteLine(TextProcessor.ERROR_FILE);
            }
        }

        public static void RunAlignContentMultipleFiles(string[] args)
        {
            const string highlightSpacesArgument = "--highlight-spaces";

            // ----- Init vars -----
            string err;
            List<string> inputFiles;
            string outputFile;
            bool highlightSpaces;
            int maxLineLength;

            // ----- Parse args using ArgumentParser -----
            ArgumentParser argumentParser = new ArgumentParser();
            argumentParser.Define(new List<Argument>()
            {
                new Argument(highlightSpacesArgument, typeof(bool), false)
            });
            (Dictionary<string, Argument> arguments, List<string> operands) = argumentParser.Parse(args);
            highlightSpaces = (bool)arguments[highlightSpacesArgument].Value;

            // ----- Validate args -----
            if (operands.Count < 3 || string.IsNullOrEmpty(operands[operands.Count - 2]) || !int.TryParse(operands[operands.Count - 1], out maxLineLength))
            {
                Console.WriteLine(TextProcessor.ERROR_ARGUMENT);
                return;
            }
            inputFiles = operands.GetRange(0, operands.Count - 2);
            outputFile = operands[operands.Count - 2];

            // ----- Run text processor -----
            try
            {
                using (TextProcessor textProcessor = new TextProcessor(new StreamWriterEx(outputFile), TextProcessor.LF, true))
                {
                    foreach (string inputFile in inputFiles)
                    {
                        try { textProcessor.Reader = new StreamReaderEx(inputFile); }
                        catch { continue; }
                        
                        textProcessor.AlignContent(maxLineLength, highlightSpaces, out err);
                        if (!string.IsNullOrEmpty(err)) Console.WriteLine(err);
                        textProcessor.Reader.Dispose();
                    }
                    textProcessor.WriteLineWords();
                }
            }
            catch
            {
                Console.WriteLine(TextProcessor.ERROR_FILE);
            }

        }
    }
    */
}
