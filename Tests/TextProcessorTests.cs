using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace LAB.Tests
{
    [TestClass]
    public class TextProcessorTests
    {
        [TestMethod]
        public void MissingArgumentsTest()
        {
            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            string[] args = new string[0];
            TextProcessorHelper.RunAlignContent(args);

            StringReader reader = new StringReader(writer.ToString());
            string firstLine = reader.ReadLine();

            Assert.AreEqual(TextProcessor.ERROR_ARGUMENT, firstLine);
        }

        [TestMethod]
        public void WrongArguments1Test()
        {
            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            string[] args = new string[] { "file1.txt" };
            TextProcessorHelper.RunAlignContent(args);

            StringReader reader = new StringReader(writer.ToString());
            string firstLine = reader.ReadLine();

            Assert.AreEqual(TextProcessor.ERROR_ARGUMENT, firstLine);
        }

        [TestMethod]
        public void WrongArguments2Test()
        {
            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            string[] args = new string[] { "file1.txt", "file2.txt" };
            TextProcessorHelper.RunAlignContent(args);

            StringReader reader = new StringReader(writer.ToString());
            string firstLine = reader.ReadLine();

            Assert.AreEqual(TextProcessor.ERROR_ARGUMENT, firstLine);
        }

        [TestMethod]
        public void FileError1Test()
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
