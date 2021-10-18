using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class TextProcessor
    {
        #region CONSTS

        public const string ERROR_FILE = "File Error";

        public const string ERROR_ARGUMENT = "Argument Error";

        public const string LF = "\n";

        public const string CR = "\r";

        #endregion

       
        #region PROPS

        /// <summary>
        /// Full file name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File reader
        /// </summary>
        public ITextReader Reader { get; protected set; }

        /// <summary>
        /// File writer
        /// </summary>
        public ITextWriter Writer { get; protected set; }

        /// <summary>
        /// New line character
        /// </summary>
        public string NewLine { get; set; } = LF;

        #endregion

        #region FIELDS

        private readonly char[] WhiteChars = new[] { ' ', '\t', '\n', '\r' };

        #endregion

        #region ENUMS

        public enum MethodEnum
        {
            CountWords,
            WordsFrequencies,
            AlignContent
        }

        #endregion

        #region CONSTRUCTORS

        public TextProcessor()
        {

        }

        public TextProcessor(string filename)
        {
            this.Name = filename;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Counts words in text file.
        /// </summary>
        /// <param name="err">Error text</param>
        /// <returns>Count of words in text file.</returns>
        public int CountWords(out string err)
        {
            err = "";
            int wordsCount = 0;
            try
            {
                if (this.ValidateFilename(out err))
                {
                    using (this.Reader = new StreamReaderEx(this.Name, this.NewLine))
                    {
                        while (!this.Reader.EndOfStream)
                        {
                            string line = this.Reader.ReadLine().Trim();
                            int lineWordsCount = line.Split(this.WhiteChars, StringSplitOptions.RemoveEmptyEntries).Length;
                            wordsCount += lineWordsCount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                err = this.CatchException(null, ERROR_FILE);
            }
            return wordsCount;
        }

        /// <summary>
        /// Counts occurrences of each word in text file
        /// </summary>
        /// <param name="err">Error text</param>
        /// <returns>Occurrences of each word in text file</returns>
        public Dictionary<string, int> WordsFrequencies(out string err)
        {
            err = "";
            Dictionary<string, int> wordFrequencies = new Dictionary<string, int>();
            try
            {
                if (this.ValidateFilename(out err))
                {
                    using (this.Reader = new StreamReaderEx(this.Name, this.NewLine))
                    {
                        while (!this.Reader.EndOfStream)
                        {
                            string line = this.Reader.ReadLine().Trim();
                            string[] lineWords = line.Split(this.WhiteChars, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string lineWord in lineWords)
                            {
                                if (wordFrequencies.ContainsKey(lineWord))
                                    wordFrequencies[lineWord]++;
                                else
                                    wordFrequencies.Add(lineWord, 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                err = this.CatchException(null, ERROR_FILE);
            }
            return wordFrequencies;
        }

        /// <summary>
        /// Aligns file content
        /// </summary>
        /// <param name="err">Error text</param>
        /// <returns>Filename</returns>
        public void AlignContent(string outputFilename, int maxLineLength, out string err)
        {
            err = "";
            List<string> lineWords = new List<string>();
            int lineLength = 0;
            try
            {
                if (this.ValidateFilename(out err))
                {
                    using (this.Writer = new StreamWriterEx(outputFilename, this.NewLine))
                    using (this.Reader = new StreamReaderEx(this.Name, this.NewLine))
                    {
                        while (!this.Reader.EndOfStream)
                        {
                            string word = this.Reader.ReadWord(out bool newParagraph);
                            if (string.IsNullOrEmpty(word)) continue;

                            if ((lineLength + word.Length > maxLineLength && lineWords.Count > 0) || newParagraph)
                            {
                                this.WriteLineToStream(lineWords, lineLength, maxLineLength, !newParagraph);
                                lineWords.Clear();
                                lineLength = 0;
                            }

                            if (newParagraph) this.Writer.WriteLine();

                            lineWords.Add(word);
                            lineLength += word.Length + 1;
                        }

                        if (lineWords.Count > 0)
                        {
                            this.WriteLineToStream(lineWords, lineLength, maxLineLength, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                err = this.CatchException(null, ERROR_FILE);
            }
        }

        /// <summary>
        /// Help method that runs method and validates args in console enviroment
        /// </summary>
        /// <param name="args">Console args</param>
        /// <param name="method">Method to be called</param>
        public static void Run(string[] args, MethodEnum method)
        {
            string err = "";
            TextProcessor textFile = new TextProcessor();

            // Runs in VS
            if (Debugger.IsAttached && args?.Length == 0)
            {
                args = textFile.GetTestArgs(method);
            }

            // Validate args
            if (textFile.ValidateArgs(method, args, out err))
            {
                // Fill props
                textFile.Name = args[0];

                // Call method
                switch (method)
                {
                    case MethodEnum.CountWords:
                        int words = textFile.CountWords(out err);
                        if (!string.IsNullOrEmpty(err)) Console.WriteLine(err);
                        else Console.WriteLine(words);
                        break;
                    case MethodEnum.WordsFrequencies:
                        Dictionary<string, int> wordsFreq = textFile.WordsFrequencies(out err);
                        if (!string.IsNullOrEmpty(err)) Console.WriteLine(err);
                        else foreach (KeyValuePair<string, int> kvp in wordsFreq.OrderBy(kvp => kvp.Key))
                            {
                                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                            }
                        break;
                    case MethodEnum.AlignContent:
                        textFile.AlignContent(args[1], int.Parse(args[2]), out err);
                        if (!string.IsNullOrEmpty(err)) Console.WriteLine(err);
                        break;
                }
            }
            else
            {
                Console.WriteLine(err);
            }
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Aligns list of words to line of desired length
        /// </summary>
        /// <param name="lineWords">List of words</param>
        /// <param name="lineLength">Lenght of line</param>
        /// <param name="maxLineLength">Desired length on line</param>
        /// <returns></returns>
        private string AlignWordsToLine(List<string> lineWords, int lineLength, int maxLineLength)
        {
            int neededWhitespaces = maxLineLength - (lineLength - 1);
            int extreNeededWhitespaces = 0;
            int neededWhitespacesPerWord = 0;

            if (lineWords.Count > 1)
            {
                extreNeededWhitespaces = neededWhitespaces % (lineWords.Count - 1);
                neededWhitespacesPerWord = (neededWhitespaces - extreNeededWhitespaces) / (lineWords.Count - 1);
            }

            StringBuilder alignedLineSb = new StringBuilder();
            alignedLineSb.Append(lineWords[0]);
            if (lineWords.Count > 1) alignedLineSb.Append(" ");

            for (int i = 1; i < lineWords.Count; i++)
            {
                string lineWord = new string(' ', neededWhitespacesPerWord) + lineWords[i];
                if (extreNeededWhitespaces-- > 0) lineWord = " " + lineWord;

                if (i == lineWords.Count - 1) alignedLineSb.Append(lineWord);
                else
                {
                    alignedLineSb.Append(lineWord);
                    alignedLineSb.Append(" ");
                }
            }
            return alignedLineSb.ToString();
        }

        /// <summary>
        /// Writes words to line to output stream
        /// </summary>
        /// <param name="lineWords">List of words</param>
        /// <param name="lineLength">Lenght of line</param>
        /// <param name="maxLineLength">Desired length on line</param>
        /// <param name="align">Put extra whitespaces between words to align words to maxLineLength</param>
        private void WriteLineToStream(List<string> lineWords, int lineLength, int maxLineLength, bool align)
        {
            if (lineLength - 1 > maxLineLength)
            {
                this.Writer.WriteLine(lineWords[0]);
            }
            else
            {
                if (align)
                {
                    this.Writer.WriteLine(this.AlignWordsToLine(lineWords, lineLength, maxLineLength));
                }
                else
                {
                    StringBuilder alignedLineSb = new StringBuilder();
                    for (int i = 0; i < lineWords.Count; i++)
                    {
                        string lineWord = lineWords[i];
                        if (i == lineWords.Count - 1) alignedLineSb.Append(lineWord);
                        else
                        {
                            alignedLineSb.Append(lineWord);
                            alignedLineSb.Append(" ");
                        }
                    }
                    this.Writer.WriteLine(alignedLineSb.ToString());
                }
            }
        }

        /// <summary>
        /// Validates Filename
        /// </summary>
        /// <param name="err">Error if Filename is not valid</param>
        /// <returns>True if Filename is valid</returns>
        private bool ValidateFilename(out string err)
        {
            err = "";
            bool ok = true;
            if (string.IsNullOrEmpty(this.Name))
            {
                err = ERROR_ARGUMENT;
                ok = false;
            }
            return ok;
        }

        /// <summary>
        /// Processes exception
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="err">Error text</param>
        /// <returns>Normalized error text</returns>
        private string CatchException(Exception exception, string err)
        {
            string ret = "";
            if (exception != null) ret = exception.Message;
            ret += err;
            return ret;
        }

        /// <summary>
        /// Returns test arguments for specific method
        /// </summary>
        /// <param name="method">Method name</param>
        /// <returns>String array of arguments</returns>
        private string[] GetTestArgs(MethodEnum method)
        {
            switch (method)
            {
                case MethodEnum.CountWords:
                    return new string[] { "lab1a_testfile.txt" };
                case MethodEnum.WordsFrequencies:
                    return new string[] { "lab1_testfile.txt" };
                case MethodEnum.AlignContent:
                    string filename = "TextFile_test3" + ".txt";
                    return new string[] { filename, "Aligned_" + filename, "40" };
            }
            return new string[0];
        }

        /// <summary>
        /// Validates arugemnts format and count
        /// </summary>
        /// <param name="method">Method to be called</param>
        /// <param name="args">Arguments passed to method</param>
        /// <param name="err">Error text</param>
        /// <returns>True if arguments are ok</returns>
        private bool ValidateArgs(MethodEnum method, string[] args, out string err)
        {
            err = "";
            bool ok = true;
            switch (method)
            {
                case MethodEnum.CountWords:
                    if (args.Length != 1)
                    {
                        ok = false;
                        err = ERROR_ARGUMENT;
                    }
                    break;
                case MethodEnum.WordsFrequencies:
                    if (args.Length != 1)
                    {
                        ok = false;
                        err = ERROR_ARGUMENT;
                    }
                    break;
                case MethodEnum.AlignContent:
                    if (args.Length != 3)
                    {
                        ok = false;
                        err = ERROR_ARGUMENT;
                    }
                    else if (!int.TryParse(args[2], out _))
                    {
                        ok = false;
                        err = ERROR_ARGUMENT;
                    }
                    break;
            }
            return ok;
        }


        #endregion
    }
}
