using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class TextFile
    {
        #region CONSTS

        public const string ERROR_FILE = "File Error";

        public const string ERROR_ARGUMENT = "Argument Error";

        #endregion

        #region PROPS

        /// <summary>
        /// Full file name
        /// </summary>
        public string Filename { get; set; }

        #endregion

        #region FIELDS

        /// <summary>
        /// Chars separating individual words
        /// </summary>
        private readonly char[] WhiteChars = new[] { ' ', '\t', '\n' };

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

        public TextFile()
        {

        }

        public TextFile(string filename)
        {
            this.Filename = filename;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Returns test arguments for specific method
        /// </summary>
        /// <param name="method">Method name</param>
        /// <returns>String array of arguments</returns>
        public string[] GetTestArgs(MethodEnum method)
        {
            switch (method)
            {
                case MethodEnum.CountWords:
                    return new string[] { "lab1a_testfile.txt" };
                case MethodEnum.WordsFrequencies:
                    return new string[] { "lab1_testfile.txt" };
                case MethodEnum.AlignContent:
                    return new string[] { "lab1_testfile2.txt", "lab1_testfile_alg2.txt", "40" };
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
        public bool ValidateArgs(MethodEnum method, string[] args, out string err)
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
                    using (StreamReader streamReader = new StreamReader(this.Filename))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            string line = streamReader.ReadLine().Trim();
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
                    using (StreamReader streamReader = new StreamReader(this.Filename))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            string line = streamReader.ReadLine().Trim();
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
                    using (StreamWriter streamWriter = new StreamWriter(outputFilename))
                    using (StreamReaderEx streamReader = new StreamReaderEx(this.Filename))
                    {
                        streamWriter.NewLine = "\n";
                        while (!streamReader.EndOfStream)
                        {
                            string word = streamReader.ReadWord(out bool newParagraph);
                            if (string.IsNullOrEmpty(word)) continue;

                            if ((lineLength + word.Length > maxLineLength && lineWords.Count > 0) || newParagraph)
                            {
                                this.WriteLineToStream(streamWriter, lineWords, lineLength, maxLineLength, !newParagraph);
                                lineWords.Clear();
                                lineLength = 0;
                            }

                            if (newParagraph) streamWriter.WriteLine();

                            lineWords.Add(word);
                            lineLength += word.Length + 1;
                        }

                        if (lineWords.Count > 0)
                        {
                            this.WriteLineToStream(streamWriter, lineWords, lineLength, maxLineLength, false);
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
        /// Runs method in console enviroment
        /// </summary>
        /// <param name="args">Console args</param>
        /// <param name="method">Method to be called</param>
        public static void Run(string[] args, MethodEnum method)
        {
            string err = "";
            TextFile textFile = new TextFile();

            // Runs in VS
            if (Debugger.IsAttached && args?.Length == 0)
            {
                args = textFile.GetTestArgs(method);
            }

            // Validate args
            if (textFile.ValidateArgs(method, args, out err))
            {
                // Fill props
                textFile.Filename = args[0];

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

        private string GetAlignedLine(List<string> lineWords, int lineLength, int maxLineLength)
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

        private void WriteLineToStream(StreamWriter streamWriter, List<string> lineWords, int lineLength, int maxLineLength, bool align)
        {
            if (lineLength - 1 > maxLineLength)
            {
                streamWriter.WriteLine(lineWords[0]);
            }
            else
            {
                if (align)
                {
                    streamWriter.WriteLine(this.GetAlignedLine(lineWords, lineLength, maxLineLength));
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
                    streamWriter.WriteLine(alignedLineSb.ToString());
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
            if (string.IsNullOrEmpty(this.Filename))
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

        #endregion
    }
}
