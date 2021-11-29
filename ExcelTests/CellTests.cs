using Microsoft.VisualStudio.TestTools.UnitTesting;
using Excel;

namespace ExcelTests
{
    [TestClass]
    public class CellTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Cell cell = new Cell(0,0);
            Assert.AreEqual("A1", cell.Adress);
        }
    }
}
