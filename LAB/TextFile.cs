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
            WordsFrequencies
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
                    return new string[] { "lab1_testfile.txt" };
                case MethodEnum.WordsFrequencies:
                    return new string[] { "lab1_testfile.txt" };
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
                            foreach (string lineWord  in lineWords)
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
