using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public class GameIO : IGameIO
    {
        public Card[] ReadPlayerCards(string line)
        {
            string[] cardsStr = line.Split(' ');
            Card[] cards = new Card[cardsStr.Length];
            for (int i = 0; i < cardsStr.Length; i++)
            {
                cards[i] = ReadCard(cardsStr[i]);
            }
            return cards;
        }

        public Card ReadCard(string cardStr)
        {
            if (cardStr.Length != 2) throw new FormatException("Invalid card format.");
            Card.Colors cardColor;
            switch (cardStr[0])
            {
                case 'c': cardColor = Card.Colors.Clubs; break;
                case 'd': cardColor = Card.Colors.Diamonds; break;
                case 'h': cardColor = Card.Colors.Hearts; break;
                case 's': cardColor = Card.Colors.Spades; break;
                default: throw new FormatException("Invalid card color format.");
            }
            return new Card(cardColor, cardStr[1].ToString());
        }

        public string WritePlayerInfo(Player player)
        {
            string str = $"Player {player.Name}: {WriteHandFullName(player.Hand.Name, player.Hand.Cards)}";
            if (player.Winner) str += " (WINNER)";
            return str;
        }

        public string WriteCard(Card card)
        {
            string colorName = "";
            switch (card.Color)
            {
                case Card.Colors.Clubs: colorName = "c"; break;
                case Card.Colors.Diamonds: colorName = "d"; break;
                case Card.Colors.Hearts: colorName = "h"; break;
                case Card.Colors.Spades: colorName = "s"; break;
            }
            return colorName + card.Sign;
        }

        private string WriteHandFullName(string name, Card[] handCards)
        {
            name += " (";
            for (int i = 0; i < handCards.Length; i++)
            {
                name += WriteCard(handCards[i]);
                if (i < handCards.Length - 1) name += ", ";
            }
            name += ")";
            return name;
        }
    }

    public interface IGameIO
    {
        Card[] ReadPlayerCards(string line);

        string WritePlayerInfo(Player player);
    }
}
