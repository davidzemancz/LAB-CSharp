using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore
{
    #region DataSource

    public interface IDataSource
    {
        Dictionary<int, Book> Books { get; set; }

        Dictionary<int, Customer> Customers { get; set; }

        public void AddBook(Book book);

        public void AddCustomer(Customer customer);

        public void AddCartItem(CartItem cartItem);

        public void RemoveCartItem(CartItem cartItem);

    }

    public class LocalDataSource : IDataSource
    {
        public Dictionary<int, Book> Books { get; set; }

        public Dictionary<int, Customer> Customers { get; set; }

        public LocalDataSource()
        {
            this.Books = new Dictionary<int, Book>();
            this.Customers = new Dictionary<int, Customer>();
        }

        public void AddBook(Book book)
        {
            this.Books[book.Id] = book;
        }

        public void AddCustomer(Customer customer)
        {
            this.Customers[customer.Id] = customer;
        }

        public void AddCartItem(CartItem cartItem)
        {
            if (!this.Customers.ContainsKey(cartItem.CustomerId)) throw new Exception($"Customer with id {cartItem.CustomerId} does not exists.");
            this.Customers[cartItem.CustomerId].AddCartItem(cartItem);
        }

        public void RemoveCartItem(CartItem cartItem)
        {
            if (!this.Customers.ContainsKey(cartItem.CustomerId)) throw new Exception($"Customer with id {cartItem.CustomerId} does not exists.");
            this.Customers[cartItem.CustomerId].RemoveCartItem(cartItem);
        }
    }

    #endregion

    #region Parsing

    public interface IDataParser<T> where T : new()
    {
        /// <summary>
        /// Parses data from string to object model
        /// </summary>
        /// <param name="line">Line of raw data</param>
        /// <returns>Model object</returns>
        public T Parse(string line);

    }

    #region Csv

    public class CsvDataParser<T> : IDataParser<T> where T : new()
    {
        public char Separator { get; set; }

        private IEnumerable<PropertyInfo> _modelProps;
        
        public CsvDataParser()
        {
            this.Separator = ';';

            Type type = typeof(T);
            this._modelProps = type.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(CsvParserAttribute)));
        }

        public T Parse(string line)
        {
            T result = new T();

            string[] parts = line.Split(Separator);

            foreach (PropertyInfo prop in this._modelProps)
            {
                CsvParserAttribute csvParserAttr = (CsvParserAttribute)prop.GetCustomAttribute(typeof(CsvParserAttribute));
                string strValue = parts[csvParserAttr.Column - 1];

                if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(result, int.Parse(strValue));
                }
                else if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(result, strValue);
                }
            }

            return result;
        }

    }

    public class CsvParserAttribute : Attribute
    {
        /// <summary>
        /// Column in which is stored property value
        /// </summary>
        public int Column { get; set; }

        public CsvParserAttribute(int column)
        {
            this.Column = column;
        }
    }

    #endregion

    #endregion
}
