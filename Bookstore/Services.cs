using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore
{
    #region General

    public interface IService
    {
        /// <summary>
        /// Runs the service
        /// </summary>
        /// <param name="line">Current line that trigged service</param>
        /// <returns>Service result</returns>
        ServiceResult Run(string line);
    }

    public class ServiceResult
    {
        public const string ERRMSG_DATAERROR = "Data error.";

        public bool Success { get; set; }

        public string Message { get; set; }

        public ServiceResult()
        {
            this.Success = true;
        }

        public ServiceResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }
    }

    #endregion

    #region CsvDataImporter

    public class CsvDataImporterService : IService
    {
        public IInputReader InputReader { get; set; }

        public IDataSource DataSource { get; set; }

        public CsvDataImporterService(IInputReader inputReader, IDataSource dataSource)
        {
            this.InputReader = inputReader;
            this.DataSource = dataSource;
        }

        public ServiceResult Run(string line)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                IDataParser<Book> bookDataParser = new CsvDataParser<Book>();
                IDataParser<Customer> customerDataParser = new CsvDataParser<Customer>();
                IDataParser<CartItem> cartItemDataParser = new CsvDataParser<CartItem>();

                while (true)
                {
                    line = this.InputReader.ReadLine();
                    if (line == null) throw new Exception("Data loading failed");
                    else if (line == Program.Commands.DATA_END) break;
                    string[] lineParts = line.Split(';');

                    if (lineParts[0] == "BOOK")
                    {
                        Book book = bookDataParser.Parse(line);
                        this.DataSource.AddBook(book);
                    }
                    else if (lineParts[0] == "CUSTOMER")
                    {
                        Customer customer = customerDataParser.Parse(line);
                        this.DataSource.AddCustomer(customer);
                    }
                    else if (lineParts[0] == "CART-ITEM")
                    {
                        CartItem cartItem = cartItemDataParser.Parse(line);

                        if (!this.DataSource.ExistsCustomerId(cartItem.CustomerId)) throw new Exception($"Customer with id {cartItem.CustomerId} does not exists");
                        if (!this.DataSource.ExistsBookId(cartItem.BookId)) throw new Exception($"Book with id {cartItem.BookId} does not exists");
                        this.DataSource.AddCartItem(cartItem);
                    }
                    else
                    {
                        throw new Exception("Unknow data type");
                    }
                }
            }
            catch
            {
                result.Success = false;
                result.Message = ServiceResult.ERRMSG_DATAERROR;
            }

            return result;
        }
    }

    #endregion

    #region HtmlDataProvider

    public class HtmlDataProviderService : IService
    {
        public IOutputWriter OutputWriter { get; set; }

        public IDataSource DataSource { get; set; }

        public HtmlDataProviderService(IOutputWriter outputWriter, IDataSource dataSource)
        {
            this.OutputWriter = outputWriter;
            this.DataSource = dataSource;
        }

        public ServiceResult Run(string line)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                string[] parts = line.Split(' ');
                
                int customerId = int.Parse(parts[1]);
                if (!this.DataSource.ExistsCustomerId(customerId)) throw new Exception($"Customer with id {customerId} does not exists");
                Customer customer = this.DataSource.Customers[customerId];

                int bookId;
                Uri uri = new Uri(parts[2]);

                if (uri.Host != "www.nezarka.net") throw new Exception("Invalid uri");
                else if (uri.Segments.Length > 4) throw new Exception("Invalid uri");

                if (uri.Segments[1] == "Books")
                {
                    this.WriteBooks(customer);
                }
                else if (uri.Segments[1] == "Books/" && uri.Segments[2] == "Detail/" && int.TryParse(uri.Segments[3], out bookId))
                {
                    if (!this.DataSource.ExistsBookId(bookId)) throw new Exception($"Book with id {bookId} does not exists");
                    Book book = this.DataSource.Books[bookId];
                    this.WriteBookDetail(customer, book);
                }
                else if (uri.Segments[1] == "ShoppingCart")
                {
                    this.WriteShoppingCart(customer);
                }
                else if (uri.Segments[1] == "ShoppingCart/" && uri.Segments[2] == "Add/" && int.TryParse(uri.Segments[3], out bookId)) 
                {
                    if (!this.DataSource.ExistsBookId(bookId)) throw new Exception($"Book with id {bookId} does not exists");
                    this.DataSource.AddCartItem(new CartItem() { CustomerId = customer.Id, BookId = bookId, BookCount = 1 });
                    this.WriteShoppingCart(customer);
                }
                else if (uri.Segments[1] == "ShoppingCart/" && uri.Segments[2] == "Remove/" && int.TryParse(uri.Segments[3], out bookId)) 
                {
                    if (!this.DataSource.ExistsBookId(bookId)) throw new Exception($"Book with id {bookId} does not exists");
                    this.DataSource.RemoveCartItem(new CartItem() { CustomerId = customer.Id, BookId = bookId, BookCount = 1 });
                    this.WriteShoppingCart(customer);
                }
                else
                {
                    throw new Exception("Invalid path");
                }
            }
            catch
            {
                WriteInvalidRequest(this.OutputWriter);
            }

            return result;
        }

        /// <summary>
        /// Writes invalid request info in HTML format via Output writer
        /// </summary>
        public static void WriteInvalidRequest(IOutputWriter outputWriter)
        {
            outputWriter.WriteLine("<!DOCTYPE html>");
            outputWriter.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
            outputWriter.WriteLine("<head>");
            outputWriter.WriteLine("	<meta charset=\"utf-8\" />");
            outputWriter.WriteLine("	<title>Nezarka.net: Online Shopping for Books</title>");
            outputWriter.WriteLine("</head>");
            outputWriter.WriteLine("<body>");
            outputWriter.WriteLine("<p>Invalid request.</p>");
            outputWriter.WriteLine("</body>");
            outputWriter.WriteLine("</html>");
        }

        /// <summary>
        /// Returns data source path from uri
        /// </summary>
        /// <param name="uri">Uri</param>
        private (string adress, string protocol, string[]) GetUriInfo(string uri)
        {
            string[] uriArr = uri.Split('/');
            int uriPathOffset = 3;
            string[] dataSourcePath = new string[uriArr.Length - uriPathOffset];
            Array.Copy(uriArr, uriPathOffset, dataSourcePath, 0, uriArr.Length - uriPathOffset);
            return (uriArr[0].Substring(0, uriArr[0].Length - 1), uriArr[2], dataSourcePath);
        }

        /// <summary>
        /// Writes header in HTML format via Output writer
        /// </summary>
        private void WriteHeader(Customer customer)
        {
            this.OutputWriter.WriteLine("<!DOCTYPE html>");
            this.OutputWriter.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
            this.OutputWriter.WriteLine("<head>");
            this.OutputWriter.WriteLine("	<meta charset=\"utf-8\" />");
            this.OutputWriter.WriteLine("	<title>Nezarka.net: Online Shopping for Books</title>");
            this.OutputWriter.WriteLine("</head>");
            this.OutputWriter.WriteLine("<body>");
            this.OutputWriter.WriteLine("	<style type=\"text/css\">");
            this.OutputWriter.WriteLine("		table, th, td {");
            this.OutputWriter.WriteLine("			border: 1px solid black;");
            this.OutputWriter.WriteLine("			border-collapse: collapse;");
            this.OutputWriter.WriteLine("		}");
            this.OutputWriter.WriteLine("		table {");
            this.OutputWriter.WriteLine("			margin-bottom: 10px;");
            this.OutputWriter.WriteLine("		}");
            this.OutputWriter.WriteLine("		pre {");
            this.OutputWriter.WriteLine("			line-height: 70%;");
            this.OutputWriter.WriteLine("		}");
            this.OutputWriter.WriteLine("	</style>");
            this.OutputWriter.WriteLine("	<h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>");
            this.OutputWriter.WriteLine($"	{customer.Name}, here is your menu:");
            this.OutputWriter.WriteLine("	<table>");
            this.OutputWriter.WriteLine("		<tr>");
            this.OutputWriter.WriteLine("			<td><a href=\"/Books\">Books</a></td>");
            this.OutputWriter.WriteLine($"			<td><a href=\"/ShoppingCart\">Cart ({customer.Cart.Items.Count})</a></td>");
            this.OutputWriter.WriteLine("		</tr>");
            this.OutputWriter.WriteLine("	</table>");
        }

        /// <summary>
        /// Writes header in HTML format via Output writer
        /// </summary>
        private void WriteFooter()
        {
            this.OutputWriter.WriteLine("</body>");
            this.OutputWriter.WriteLine("</html>");
        }

        /// <summary>
        /// Writes list of all books in HTML format via Output writer
        /// </summary>
        private void WriteBooks(Customer customer)
        {
            this.WriteHeader(customer);

            this.OutputWriter.WriteLine("	Our books for you:");
            this.OutputWriter.WriteLine("	<table>");

            bool booksAny = this.DataSource.Books.Any();
            int bookCounter = 0;

            if (booksAny) this.OutputWriter.WriteLine("		<tr>");
            foreach (KeyValuePair<int, Book> kvp in this.DataSource.Books)
            {
                if (bookCounter == 3)
                {
                    bookCounter = 0;

                    this.OutputWriter.WriteLine("		</tr>");
                    this.OutputWriter.WriteLine("		<tr>");
                }
                bookCounter++;

                Book book = kvp.Value;
                this.OutputWriter.WriteLine("			<td style=\"padding: 10px;\">");
                this.OutputWriter.WriteLine($"				<a href=\"/Books/Detail/{book.Id}\">{book.Title}</a><br />");
                this.OutputWriter.WriteLine($"				Author: {book.Author}<br />");
                this.OutputWriter.WriteLine($"				Price: {book.Price} EUR &lt;<a href=\"/ShoppingCart/Add/{book.Id}\">Buy</a>&gt;");
                this.OutputWriter.WriteLine("			</td>");
            }
            if (booksAny) this.OutputWriter.WriteLine("		</tr>");

            this.OutputWriter.WriteLine("	</table>");

            this.WriteFooter();
        }

        /// <summary>
        /// Writes detail of one book in HTML format via Output writer
        /// </summary>
        private void WriteBookDetail(Customer customer, Book book)
        {
            this.WriteHeader(customer);

            this.OutputWriter.WriteLine("	Book details:");
            this.OutputWriter.WriteLine($"	<h2>{book.Title}</h2>");
            this.OutputWriter.WriteLine("	<p style=\"margin-left: 20px\">");
            this.OutputWriter.WriteLine($"	Author: {book.Author}<br />");
            this.OutputWriter.WriteLine($"	Price: {book.Price} EUR<br />");
            this.OutputWriter.WriteLine("	</p>");
            this.OutputWriter.WriteLine($"	<h3>&lt;<a href=\"/ShoppingCart/Add/{book.Id}\">Buy this book</a>&gt;</h3>");

            this.WriteFooter();
        }

        /// <summary>
        /// Writes customers shopping cart in HTML format via Output writer
        /// </summary>
        private void WriteShoppingCart(Customer customer)
        {
            if (customer.Cart.Items.Any())
            {
                this.WriteHeader(customer);

                this.OutputWriter.WriteLine("	Your shopping cart:");
                this.OutputWriter.WriteLine("	<table>");
                this.OutputWriter.WriteLine("		<tr>");
                this.OutputWriter.WriteLine("			<th>Title</th>");
                this.OutputWriter.WriteLine("			<th>Count</th>");
                this.OutputWriter.WriteLine("			<th>Price</th>");
                this.OutputWriter.WriteLine("			<th>Actions</th>");
                this.OutputWriter.WriteLine("		</tr>");

                int totalPrice = 0;

                foreach (KeyValuePair<int, CartItem> kvp in customer.Cart.Items)
                {
                    CartItem cartItem = kvp.Value;
                    Book book = this.DataSource.Books[cartItem.BookId];
                    totalPrice += cartItem.BookCount * book.Price;

                    this.OutputWriter.WriteLine("		<tr>");
                    this.OutputWriter.WriteLine($"			<td><a href=\"/Books/Detail/{book.Id}\">{book.Title}</a></td>");
                    this.OutputWriter.WriteLine($"			<td>{cartItem.BookCount}</td>");
                    if (cartItem.BookCount > 1) this.OutputWriter.WriteLine($"			<td>{cartItem.BookCount} * {book.Price} = {cartItem.BookCount * book.Price} EUR</td>");
                    else this.OutputWriter.WriteLine($"			<td>{book.Price} EUR</td>");
                    this.OutputWriter.WriteLine($"			<td>&lt;<a href=\"/ShoppingCart/Remove/{book.Id}\">Remove</a>&gt;</td>");
                    this.OutputWriter.WriteLine("		</tr>");
                }

                this.OutputWriter.WriteLine("	</table>");
                this.OutputWriter.WriteLine($"	Total price of all items: {totalPrice} EUR");

                this.WriteFooter();
            }
            else
            {
                this.WriteShoppingCartEmpty(customer);
            }
        }

        /// <summary>
        /// Writes customers shopping cart empty page in HTML format via Output writer
        /// </summary>
        private void WriteShoppingCartEmpty(Customer customer)
        {
            this.WriteHeader(customer);

            this.OutputWriter.WriteLine("	Your shopping cart is EMPTY.");

            this.WriteFooter();
        }

     
    }

    #endregion
}
