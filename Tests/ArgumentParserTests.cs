using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB.Tests
{
    [TestClass]
    public class ArgumentParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentsDefinitionException))]
        public void ParseWithoutDefinitionTest()
        {
            string[] args = new string[] { "-p", "true" };
            ArgumentParser argumentParser = new ArgumentParser();
            (Dictionary<string, Argument> arguments, List<string> operands) = argumentParser.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(UnexpectedArgumentException))]
        public void ParseUnexpectedArgumentTest()
        {
            string[] args = new string[] { "-p", "true" };
            ArgumentParser argumentParser = new ArgumentParser();
            argumentParser.Define(new List<Argument>());
            (Dictionary<string, Argument> arguments, List<string> operands) = argumentParser.Parse(args);
        }

        [TestMethod]
        public void ParseArgumentsTest()
        {
            string[] args = new string[] { "--action", "copy" ,"-f", "--mode", "2", "file1.txt", "file2.txt" };
            ArgumentParser argumentParser = new ArgumentParser();
            argumentParser.Define(new List<Argument>()
            {
                new Argument("-f", typeof(bool)),
                new Argument("--mode", typeof(int)),
                new Argument("--action", typeof(string)),
            });
            (Dictionary<string, Argument> arguments, List<string> operands) = argumentParser.Parse(args);

            Assert.AreEqual(Argument.TypeEnum.Option, arguments["-f"].Type);
            Assert.AreEqual(true, arguments["-f"].Value);

            Assert.AreEqual(Argument.TypeEnum.OptionLong, arguments["--mode"].Type);
            Assert.AreEqual(2, arguments["--mode"].Value);

            Assert.AreEqual(Argument.TypeEnum.OptionLong, arguments["--action"].Type);
            Assert.AreEqual("copy", arguments["--action"].Value);

            Assert.AreEqual(operands[0], "file1.txt");
            Assert.AreEqual(operands[1], "file2.txt");
        }
    }
}
