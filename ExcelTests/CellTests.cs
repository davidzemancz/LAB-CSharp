using Microsoft.VisualStudio.TestTools.UnitTesting;
using Excel;

namespace ExcelTests
{
    [TestClass]
    public class CellTests
    {
        [TestMethod]
        public void TestAdressConversion()
        {
            Cell cell = new Cell(0,0);
            Assert.AreEqual("A1", cell.Adress);

            cell = new Cell(0, 25);
            Assert.AreEqual("Z1", cell.Adress);

            cell = new Cell(0, 26);
            Assert.AreEqual("AA1", cell.Adress);

            cell = new Cell(0, 702);
            Assert.AreEqual("AAA1", cell.Adress);
        }

        [TestMethod]
        public void TestIntegerValue()
        {
            Sheet sheet = new Sheet();
            Cell cell = new Cell(0, 0) { Value = 5 };
            sheet.AddCell(cell);
            foreach (var kvp in sheet.CellsByAdress)
            {
                Cell kvpCell = kvp.Value;
                kvpCell.Evaluate(sheet);
            }
            Assert.AreEqual(5, (int)cell.Value);
        }

        [TestMethod]
        public void TestDivByZero()
        {
            Sheet sheet = new Sheet();
            Cell cell1 = new Cell(0, 0) { Value = 5 };
            sheet.AddCell(cell1);
            Cell cell2 = new Cell(1, 0) { Value = 0 };
            sheet.AddCell(cell2);
            Cell cell3 = new Cell(2, 0) { Value = "=A1/A2" };
            sheet.AddCell(cell3);

            foreach (var kvp in sheet.CellsByAdress)
            {
                Cell kvpCell = kvp.Value;
                kvpCell.Evaluate(sheet);
            }
            Assert.AreEqual("#DIV0", cell3.Value);
        }

        [TestMethod]
        public void TestMissOp()
        {
            Sheet sheet = new Sheet();
            Cell cell1 = new Cell(0, 0) { Value = 5 };
            sheet.AddCell(cell1);
            Cell cell2 = new Cell(1, 0) { Value = 4 };
            sheet.AddCell(cell2);
            Cell cell3 = new Cell(2, 0) { Value = "=A1A2" };
            sheet.AddCell(cell3);

            foreach (var kvp in sheet.CellsByAdress)
            {
                Cell kvpCell = kvp.Value;
                kvpCell.Evaluate(sheet);
            }
            Assert.AreEqual("#MISSOP", cell3.Value);
        }

        [TestMethod]
        public void TestCycle()
        {
            Sheet sheet = new Sheet();
            Cell cell1 = new Cell(0, 0) { Value = "=A3+A2" };    // A1
            sheet.AddCell(cell1);
            Cell cell2 = new Cell(1, 0) { Value = 4 };           // A2
            sheet.AddCell(cell2);
            Cell cell3 = new Cell(2, 0) { Value = "=A4+A2" };    // A3
            sheet.AddCell(cell3);
            Cell cell4 = new Cell(3, 0) { Value = "=A1+A2" };    // A4
            sheet.AddCell(cell4);
            Cell cell5 = new Cell(4, 0) { Value = "=A4+A6" };    // A5
            sheet.AddCell(cell5);
            Cell cell6 = new Cell(5, 0) { Value = "2" };         // A6
            sheet.AddCell(cell6);
            Cell cell7 = new Cell(6, 0) { Value = "=A7+A6" };    // A7
            sheet.AddCell(cell7);

            foreach (var kvp in sheet.CellsByAdress)
            {
                Cell kvpCell = kvp.Value;
                kvpCell.Evaluate(sheet);
            }

            Assert.AreEqual("#CYCLE", cell1.Value);
            Assert.AreEqual(4, (int)cell2.Value);
            Assert.AreEqual("#CYCLE", cell3.Value);
            Assert.AreEqual("#CYCLE", cell4.Value);
            Assert.AreEqual("#ERROR", cell5.Value);
            Assert.AreEqual(2, (int)cell6.Value);
            Assert.AreEqual("#CYCLE", cell7.Value);
        }

        [TestMethod]
        public void TestInvalidValue()
        {
            Sheet sheet = new Sheet();
            Cell cell1 = new Cell(0, 0) { Value = "autobus" };    // A1
            sheet.AddCell(cell1);
            Cell cell2 = new Cell(1, 0) { Value = "" };          // A2
            sheet.AddCell(cell2);
            Cell cell3 = new Cell(2, 0) { Value = "-2" };    // A3
            sheet.AddCell(cell3);

            foreach (var kvp in sheet.CellsByAdress)
            {
                Cell kvpCell = kvp.Value;
                kvpCell.Evaluate(sheet);
            }

            Assert.AreEqual("#INVVAL", cell1.Value);
            Assert.AreEqual("#INVVAL", cell2.Value);
            Assert.AreEqual("#INVVAL", cell3.Value);
        }

        [TestMethod]
        public void TestInvalidFormula()
        {
            Sheet sheet = new Sheet();
            Cell cell1 = new Cell(0, 0) { Value = "=A1+A2+A3" };    // A1
            sheet.AddCell(cell1);
            Cell cell2 = new Cell(1, 0) { Value = "=A2+3" };        // A2
            sheet.AddCell(cell2);
            Cell cell3 = new Cell(2, 0) { Value = "=A2++A2" };      // A3
            sheet.AddCell(cell3);

            foreach (var kvp in sheet.CellsByAdress)
            {
                Cell kvpCell = kvp.Value;
                kvpCell.Evaluate(sheet);
            }

            Assert.AreEqual("#FORMULA", cell1.Value);
            Assert.AreEqual("#FORMULA", cell2.Value);
            Assert.AreEqual("#FORMULA", cell3.Value);
        }
    }
}
