using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public class Game
    {
        public Player[] Players { get; }

        public Game(int players)
        {
            Players = new Player[players];
            for (int i = 0; i < players; i++)
            {
                Players[i] = new Player() { Name = (i + 1).ToString() };
            }
        }

        public void StartRound(Card[][] cardsForPlayers)
        {
            // Deal cards to players
            for (int i = 0; i < cardsForPlayers.GetLength(0); i++)
            {
                Players[i].SetCards(cardsForPlayers[i]);
            }
        }

        public void EvaluateRound()
        {
            // Find maximum hand weight
            int handWeightMax = 0;
            for (int i = 0; i < Players.Length; i++)
            {
                int handWeight = (int)Players[i].Hand.Weight * Players[i].Hand.Cards[Players[i].Hand.Cards.Length - 1].Value;
                if (handWeight > handWeightMax) handWeightMax = handWeight;
            }

            // Mark players with maxium hand weight as winners
            for (int i = 0; i < Players.Length; i++)
            {
                int handWeight = (int)Players[i].Hand.Weight * Players[i].Hand.Cards[Players[i].Hand.Cards.Length - 1].Value;
                if (handWeight == handWeightMax) Players[i].Winner = true;
                else Players[i].Winner = false;
            }
        }
    }

}
