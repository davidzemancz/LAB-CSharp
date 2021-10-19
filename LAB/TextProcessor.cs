using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class TextProcessor : IDisposable
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
        public ITextReader Reader { get; set; }

        /// <summary>
        /// File writer
        /// </summary>
        public ITextWriter Writer { get; set; }

        /// <summary>
        /// New line character
        /// </summary>
        public string NewLine
        {
            get => this._newLine;
            set
            {
                this._newLine = value;
                if (this.Writer != null) this.Writer.NewLine = value;
                if (this.Reader != null) this.Reader.NewLine = value;
            }
        }

        #endregion

        #region FIELDS

        private readonly char[] WhiteChars = new[] { ' ', '\t', '\n', '\r' };

        private string _newLine;

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

        public TextProcessor(ITextReader reader, ITextWriter writer, string newLine)
        {
            this.Reader = reader;
            this.Writer = writer;
            this.NewLine = newLine;
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
                while (!this.Reader.EndOfStream)
                {
                    string line = this.Reader.ReadLine().Trim();
                    int lineWordsCount = line.Split(this.WhiteChars, StringSplitOptions.RemoveEmptyEntries).Length;
                    wordsCount += lineWordsCount;
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
        public void AlignContent(int maxLineLength, out string err)
        {
            err = "";
            List<string> lineWords = new List<string>();
            int lineLength = 0;
            try
            {
                while (!this.Reader.EndOfStream)
                {
                    string word = this.Reader.ReadWord(out bool newParagraph);
                    if (string.IsNullOrEmpty(word)) continue;

                    if ((lineLength + word.Length > maxLineLength && lineWords.Count > 0) || newParagraph)
                    {
                        this.WriteLine(lineWords, lineLength, maxLineLength, !newParagraph);
                        lineWords.Clear();
                        lineLength = 0;
                    }

                    if (newParagraph) this.Writer.WriteLine();

                    lineWords.Add(word);
                    lineLength += word.Length + 1;
                }

                if (lineWords.Count > 0)
                {
                    this.WriteLine(lineWords, lineLength, maxLineLength, false);
                }
            }
            catch (Exception ex)
            {
                err = this.CatchException(null, ERROR_FILE);
            }
        }

        /// <summary>
        /// Dispose writer and reader
        /// </summary>
        public void Dispose()
        {
            if (this.Writer != null) this.Writer.Dispose();
            if (this.Reader != null) this.Reader.Dispose();
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
        private void WriteLine(List<string> lineWords, int lineLength, int maxLineLength, bool align)
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
