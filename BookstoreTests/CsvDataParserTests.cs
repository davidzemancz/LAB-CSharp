using Bookstore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreTests
{
    [TestClass]
    public class CsvDataParserTests
    {
        [TestMethod]
        public void BookParsingTest()
        {
            IDataParser<Book> bookDataParser = new CsvDataParser<Book>();
            int bookId = 16;
            string bookTitle = "Svìt podle Šprota";
            string bookAuthor = "Jaromír Kajetán Pil";
            int bookPrice = 1728;
            Book book = bookDataParser.Parse($"BOOK;{bookId};{bookTitle};{bookAuthor};{bookPrice}");
            Assert.AreEqual(bookId, book.Id);
            Assert.AreEqual(bookTitle, book.Title);
            Assert.AreEqual(bookAuthor, book.Author);
            Assert.AreEqual(bookPrice, book.Price);
        }

        [TestMethod]
        public void CustomerParsingTest()
        {
            IDataParser<Customer> customerDataParser = new CsvDataParser<Customer>();
            int custId = 12;
            string custName = "Anotnín";
            string custSurname = "Šnek";
            Customer customer = customerDataParser.Parse($"CUSTOMER;{custId};{custName};{custSurname}");
            Assert.AreEqual(custId, customer.Id);
            Assert.AreEqual(custName, customer.Name);
            Assert.AreEqual(custSurname, customer.Surname);
        }

        [TestMethod]
        public void CartItemParsingTest()
        {
            IDataParser<CartItem> cartItemDataParser = new CsvDataParser<CartItem>();
            int custId = 12;
            int bookId = 16;
            int bookCount = 2;
            CartItem cartItem = cartItemDataParser.Parse($"CART-ITEM;{custId};{bookId};{bookCount}");
            Assert.AreEqual(custId, cartItem.CustomerId);
            Assert.AreEqual(bookId, cartItem.BookId);
            Assert.AreEqual(bookCount, cartItem.BookCount);
        }


    }
}
