using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore
{
    public class Book
    {
        [CsvParser(2)]
        public int Id { get; set; }
        
        [CsvParser(3)]
        public string Title { get; set; }

        [CsvParser(4)]
        public string Author { get; set; }

        [CsvParser(5)]
        public int Price { get; set; }

    }

    public class Customer
    {
        [CsvParser(2)]
        public int Id { get; set; }

        [CsvParser(3)]
        public string Name { get; set; }

        [CsvParser(4)]
        public string Surname { get; set; }

        public Cart Cart { get; set; }

        public Customer()
        {
            this.Cart = new Cart() { Items = new Dictionary<int, CartItem>() };
        }

        public void AddCartItem(CartItem cartItem)
        {
            if (cartItem.CustomerId != this.Id) throw new Exception($"Not matching id. Customer id ({this.Id}) is not equal to cart item id ({cartItem.CustomerId})");
            else if (cartItem.BookCount <= 0) throw new Exception($"Cannont add negative amound of items");

            if (this.Cart.Items.ContainsKey(cartItem.BookId)) this.Cart.Items[cartItem.BookId].BookCount += cartItem.BookCount;
            else this.Cart.Items.Add(cartItem.BookId, cartItem);
        }

        public void RemoveCartItem(CartItem cartItem)
        {
            if (cartItem.CustomerId != this.Id) throw new Exception($"Not matching id. Customer id ({this.Id}) is not equal to cart item id ({cartItem.CustomerId})");
            else if (cartItem.BookCount <= 0) throw new Exception($"Cannont remove negative amound of items");
            else if (this.Cart.Items.ContainsKey(cartItem.BookId))
            {
                this.Cart.Items[cartItem.BookId].BookCount -= cartItem.BookCount;
                if (this.Cart.Items[cartItem.BookId].BookCount <= 0)
                {
                    this.Cart.Items.Remove(cartItem.BookId);
                }
            }
            else throw new Exception($"Book ({cartItem.BookId}) does not exists in customers ({this.Id}) cart.");
        }
    }

    public class Cart
    {
        /// <summary>
        /// Dictionary of cart items where key is book id
        /// </summary>
        public Dictionary<int, CartItem> Items { get; set; }
    }

    public class CartItem
    {
        [CsvParser(2)]
        public int CustomerId { get; set; }

        [CsvParser(3)]
        public int BookId { get; set; }

        [CsvParser(4)]
        public int BookCount { get; set; }
    }
}
