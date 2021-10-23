using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace LAB.Tests
{
    [TestClass]
    public class TextProcessorHelperTests
    {
        [TestMethod]
        [DataRow(new string[0])]
        [DataRow(new string[] { "fileIn.txt" })]
        [DataRow(new string[] { "fileIn.txt", "fileOut.txt" })]
        [DataRow(new string[] { "fileIn.txt", "fileOut.txt", "a" })]
        public void ArgumentErrorTest(string[] args)
        {
            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            TextProcessorHelper.RunAlignContent(args);

            StringReader reader = new StringReader(writer.ToString());
            string firstLine = reader.ReadLine();

            Assert.AreEqual(TextProcessor.ERROR_ARGUMENT, firstLine);
        }

        [TestMethod]
        public void FileErrorTest()
        {
            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            string[] args = new string[] { "file1.txt", "file2.txt", "40" };
            TextProcessorHelper.RunAlignContent(args);

            StringReader reader = new StringReader(writer.ToString());
            string firstLine = reader.ReadLine();

            Assert.AreEqual(TextProcessor.ERROR_FILE, firstLine);
        }
    }
}
