using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public class Hand
    {
        public string Name { get; }
        public Weights Weight { get; }

        public Card[] Cards { get; protected set; }

        private Card[] _allCards;

        public Hand(Card[] allCards)
        {
            Card[] outCards;
            _allCards = allCards;
            if (IsStraightFlush(out outCards))
            {
                Cards = outCards;
                Weight = Weights.StraightFlush;
                Name = "Straight flush";
            }
            else if (IsFourOfKind(out outCards))
            {
                Cards = outCards;
                Weight = Weights.FourOfKind;
                Name = "Four of a kind";
            }
            else if (IsFlush(out outCards))
            {
                Cards = outCards;
                Weight = Weights.Flush;
                Name = "Flush";
            }
            else if (ISStraight(out outCards))
            {
                Cards = outCards;
                Weight = Weights.Straight;
                Name = "Straight";
            }
            else if (IsThreeOfKind(out outCards))
            {
                Cards = outCards;
                Weight = Weights.ThreeOfKind;
                Name = "Three of a kind";
            }
            else if (IsOnePair(out outCards))
            {
                Cards = outCards;
                Weight = Weights.OnePair;
                Name = "One pair";
            }
            else
            {
                Cards = new Card[] { _allCards[_allCards.Length - 1] };
                Weight = Weights.HighCard;
                Name = "High card";
            }

        }

        private bool IsStraightFlush(out Card[] outCards)
        {
            outCards = _allCards;

            bool ret = true;
            if (_allCards.Length < 5) ret = false;
            {
                for (int i = 1; i < _allCards.Length; i++)
                {
                    if (_allCards[i - 1].Color != _allCards[i].Color || _allCards[i - 1].Value + 1 != _allCards[i].Value)
                    {
                        // A,2,... of same color
                        if (i == 1 && _allCards[i - 1].IsAce && _allCards[i].Value == 2 && _allCards[i - 1].Color == _allCards[i].Color) continue;

                        ret = false;
                        break;
                    }
                }
            }

            return ret;
        }

        private bool IsFourOfKind(out Card[] outCards)
        {
            return IsNOfKind(4, out outCards);
        }

        private bool IsFlush(out Card[] outCards)
        {
            outCards = _allCards;
            bool ret = true;
            if (_allCards.Length < 5) ret = false;
            {
                for (int i = 1; i < _allCards.Length; i++)
                {
                    if (_allCards[i - 1].Color != _allCards[i].Color)
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        private bool ISStraight(out Card[] outCards)
        {
            outCards = _allCards;
            bool ret = true;
            if (_allCards.Length < 5) ret = false;
            {
                for (int i = 1; i < _allCards.Length; i++)
                {
                    if (_allCards[i - 1].Value + 1 != _allCards[i].Value)
                    {
                        // A,2,...
                        if (i == 1 && _allCards[i - 1].IsAce && _allCards[i].Value == 2) continue;

                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        private bool IsThreeOfKind(out Card[] outCards)
        {
            return IsNOfKind(3, out outCards);
        }

        private bool IsOnePair(out Card[] outCards)
        {
            return IsNOfKind(2, out outCards);
        }

        private bool IsNOfKind(int n, out Card[] outCards)
        {
            outCards = null;
            bool ret = false;
            if (_allCards.Length >= n)
            {
                for (int i = 0; i <= _allCards.Length - n; i++)
                {
                    int sameValue = 0;
                    Card card = _allCards[i];
                    Card[] tempOutCards = new Card[n];
                    tempOutCards[sameValue++] = card;
                    for (int j = i + 1; j < _allCards.Length; j++)
                    {
                        if (_allCards[j].Value == card.Value)
                        {
                            tempOutCards[sameValue++] = _allCards[j];
                        }
                    }
                    if (sameValue >= n)
                    {
                        if (outCards == null || outCards.Length == 0 || outCards[0].Value < tempOutCards[0].Value) outCards = tempOutCards;
                        ret = true;
                    }
                }
            }
            return ret;
        }

        public enum Weights
        {
            StraightFlush = 64000000,
            FourOfKind = 3200000,
            Flush = 160000,
            Straight = 8000,
            ThreeOfKind = 400,
            OnePair = 20,
            HighCard = 1,
            None = 0,
        }
    }

}
