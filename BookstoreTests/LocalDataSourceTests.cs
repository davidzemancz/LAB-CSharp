using Bookstore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookstoreTests
{
    [TestClass]
    public class LocalDataSourceTests
    {
        private static IDataSource dataSource;

        [TestMethod]
        public void _0_Init()
        {
            dataSource = new LocalDataSource();
        }
       
        [TestMethod]
        public void _1_AddBooksTest()
        {
            dataSource.AddBook(new Book(){  Id = 1, Title = "Kuchařka Zdeňka a Jiřky", Author = "Zdeněk a Jiřka", Price = 256 });
            dataSource.AddBook(new Book() { Id = 2, Title = "Jak se stát lovcem žen?", Author = "Roman", Price = 1480 });
            dataSource.AddBook(new Book() { Id = 3, Title = "Pohádky z jedné ohrádky", Author = "Vít Nejedlý", Price = 366 });

            Assert.AreEqual(3, dataSource.Books.Count);
            Assert.AreEqual("Kuchařka Zdeňka a Jiřky", dataSource.Books[1].Title);
            Assert.AreEqual("Jak se stát lovcem žen?", dataSource.Books[2].Title);
            Assert.AreEqual("Pohádky z jedné ohrádky", dataSource.Books[3].Title);

            Assert.AreEqual(256, dataSource.Books[1].Price);
            Assert.AreEqual(1480, dataSource.Books[2].Price);
            Assert.AreEqual(366, dataSource.Books[3].Price);
        }

        [TestMethod]
        public void _2_AddCustomersTest()
        {
            dataSource.AddCustomer(new Customer() { Id = 1, Name = "Pavel" });
            dataSource.AddCustomer(new Customer() { Id = 2, Name = "Jonáš" });

            Assert.AreEqual(2, dataSource.Customers.Count);
            Assert.AreEqual("Pavel", dataSource.Customers[1].Name);
            Assert.AreEqual("Jonáš", dataSource.Customers[2].Name);
        }

        [TestMethod]
        public void _3_AddCartItems()
        {
            dataSource.AddCartItem(new CartItem() { CustomerId = 1, BookId = 1, BookCount = 1 });
            dataSource.AddCartItem(new CartItem() { CustomerId = 1, BookId = 1, BookCount = 1 });
            dataSource.AddCartItem(new CartItem() { CustomerId = 1, BookId = 2, BookCount = 2 });

            Assert.AreEqual(2, dataSource.Customers[1].Cart.Items[1].BookCount);
            Assert.AreEqual(2, dataSource.Customers[1].Cart.Items[2].BookCount);

            dataSource.AddCartItem(new CartItem() { CustomerId = 2, BookId = 1, BookCount = 2 });
            dataSource.AddCartItem(new CartItem() { CustomerId = 2, BookId = 2, BookCount = 1 });
            dataSource.AddCartItem(new CartItem() { CustomerId = 2, BookId = 2, BookCount = 1 });
            dataSource.AddCartItem(new CartItem() { CustomerId = 2, BookId = 3, BookCount = 1 });

            Assert.AreEqual(2, dataSource.Customers[2].Cart.Items[1].BookCount);
            Assert.AreEqual(2, dataSource.Customers[2].Cart.Items[2].BookCount);
            Assert.AreEqual(1, dataSource.Customers[2].Cart.Items[3].BookCount);
        }
    }
}
