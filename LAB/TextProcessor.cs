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

        public const string HIGHLIGHT_NEWLINE = "<-";

        public const string HIGHLIGHT_SPACE = ".";

        #endregion

        #region PROPS

        /// <summary>
        /// Text reader
        /// </summary>
        public ITextReader Reader
        {
            get => this._reader;
            set
            {
                this._reader = value;
                if (this._reader != null) this._reader.NewLine = this.NewLine;
            }
        }

        /// <summary>
        /// Text writer
        /// </summary>
        public ITextWriter Writer
        {
            get => this._writer;
            set
            {
                this._writer = value;
                if (this._writer != null) this._writer.NewLine = this.NewLine;
            }
        }

        /// <summary>
        /// New line string
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

        /// <summary>
        /// Space string
        /// </summary>
        public string Space { get; set; }

        /// <summary>
        /// White characters that separate words
        /// </summary>
        public char[] WhiteChars { get; set; }

        #endregion

        #region FIELDS

        private string _newLine;

        private ITextReader _reader;

        private ITextWriter _writer;

        private List<string> _lineWordsBuffer;

        private int _lineLength;

        private bool _keepLineWords;

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
            this.WhiteChars = new[] { ' ', '\t', '\n', '\r' };
            this.NewLine = Environment.NewLine;
            this.Space = " ";
        }

        public TextProcessor(ITextReader reader, ITextWriter writer, string newLine) : this()
        {
            this.Reader = reader;
            this.Writer = writer;
            this.NewLine = newLine;
        }

        public TextProcessor(ITextWriter writer, string newLine) : this()
        {
            this.Writer = writer;
            this.NewLine = newLine;
        }

        public TextProcessor(ITextWriter writer, string newLine, bool keepLineWords) : this(writer, newLine)
        {
            this._keepLineWords = keepLineWords;
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
        public void AlignContent(int maxLineLength, bool highlight, out string err)
        {
            err = "";
            if (!this._keepLineWords || this._lineWordsBuffer == null) this._lineWordsBuffer = new List<string>();

            if (highlight)
            {
                this.Writer.NewLine = HIGHLIGHT_NEWLINE + this.NewLine;
                this.Space = HIGHLIGHT_SPACE;
            }

            try
            {
                while (!this.Reader.EndOfStream)
                {
                    string word = this.Reader.ReadWord(out bool newParagraph);
                    if (string.IsNullOrEmpty(word)) continue;

                    if ((_lineLength + word.Length > maxLineLength && this._lineWordsBuffer.Count > 0) || newParagraph)
                    {
                        this.WriteLine(maxLineLength, !newParagraph);
                        this._lineWordsBuffer.Clear();
                        _lineLength = 0;
                    }

                    if (newParagraph)
                    {
                        this.Writer.WriteLine();
                    }

                    this._lineWordsBuffer.Add(word);
                    _lineLength += word.Length + 1;
                }

                if (!this._keepLineWords && this._lineWordsBuffer.Count > 0)
                {
                    this.WriteLine(maxLineLength, false);
                }
            }
            catch (Exception ex)
            {
                err = this.CatchException(null, ERROR_FILE);
            }
        }

        /// <summary>
        /// Writes words in buffer to writer
        /// </summary>
        public void WriteLineWords()
        {
            if (this._lineWordsBuffer?.Any() ?? false) this.WriteLine( -1, false);
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
        private string AlignWordsToLine(int maxLineLength)
        {
            int neededWhitespaces = maxLineLength - (this._lineLength - 1);
            int extreNeededWhitespaces = 0;
            int neededWhitespacesPerWord = 0;

            if (this._lineWordsBuffer.Count > 1)
            {
                extreNeededWhitespaces = neededWhitespaces % (this._lineWordsBuffer.Count - 1);
                neededWhitespacesPerWord = (neededWhitespaces - extreNeededWhitespaces) / (this._lineWordsBuffer.Count - 1);
            }

            StringBuilder alignedLineSb = new StringBuilder();
            alignedLineSb.Append(this._lineWordsBuffer[0]);
            if (this._lineWordsBuffer.Count > 1) alignedLineSb.Append(this.Space);

            for (int i = 1; i < this._lineWordsBuffer.Count; i++)
            {
                string lineWord = new string(this.Space[0], neededWhitespacesPerWord) + this._lineWordsBuffer[i];
                if (extreNeededWhitespaces-- > 0) lineWord = this.Space + lineWord;

                if (i == this._lineWordsBuffer.Count - 1) alignedLineSb.Append(lineWord);
                else
                {
                    alignedLineSb.Append(lineWord);
                    alignedLineSb.Append(this.Space);
                }
            }
            return alignedLineSb.ToString();
        }

        /// <summary>
        /// Writes words to line to output stream
        /// </summary>
        /// <param name="lineWords">List of words</param>
        /// <param name="lineLength">Lenght of line</param>
        /// <param name="maxLineLength">Desired length on line. Ignored if set to -1</param>
        /// <param name="align">Put extra whitespaces between words to align words to maxLineLength</param>
        private void WriteLine(int maxLineLength, bool align)
        {
            if (this._lineLength - 1 > maxLineLength && maxLineLength > -1)
            {
                this.Writer.WriteLine(this._lineWordsBuffer[0]);
            }
            else
            {
                if (align)
                {
                    this.Writer.WriteLine(this.AlignWordsToLine(maxLineLength));
                }
                else
                {
                    StringBuilder alignedLineSb = new StringBuilder();
                    for (int i = 0; i < this._lineWordsBuffer.Count; i++)
                    {
                        string lineWord = this._lineWordsBuffer[i];
                        if (i == this._lineWordsBuffer.Count - 1) alignedLineSb.Append(lineWord);
                        else
                        {
                            alignedLineSb.Append(lineWord);
                            alignedLineSb.Append(this.Space);
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
