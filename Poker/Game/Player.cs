using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public class Player
    {
        public bool Winner { get; set; }
        public string Name { get; set; }
        public Hand Hand { get; protected set; }
        public Card[] Cards { get; protected set; }

        public void SetCards(Card[] cards)
        {
            Cards = cards;
            Array.Sort(Cards);
            Hand = new Hand(Cards);
        }
    }

}
