using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public class Card : IComparable<Card>
    {
        public int Value { get; protected set; }
        public string Sign { get; }
        public Colors Color { get; }
        public bool IsAce => Value == 14;

        public Card(Colors color, string sign)
        {
            Color = color;
            Sign = sign.ToUpper();
            SetSignValue();
        }

        public enum Colors
        {
            Clubs = 1,
            Diamonds = 2,
            Hearts = 3,
            Spades = 4
        }
        private void SetSignValue()
        {
            if (Sign == "A") Value = 14;
            else if (Sign == "K") Value = 13;
            else if (Sign == "Q") Value = 12;
            else if (Sign == "J") Value = 11;
            else if (Sign == "X") Value = 10;
            else if (int.TryParse(Sign, out int value) && 1 < value && value < 10) Value = value;
            else throw new FormatException("Invalid card sign format");
        }

        public int CompareTo(Card other)
        {
            return Value - other.Value;
        }
    }

}
