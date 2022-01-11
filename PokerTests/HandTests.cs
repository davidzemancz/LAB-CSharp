using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker;
using System;

namespace PokerTests
{
    [TestClass]
    public class HandTests
    {
        [TestMethod]
        public void TestHandStraightFlush()
        {
            Hand hand = new Hand( new Card[] { 
                new Card(Card.Colors.Diamonds, "7"),
                new Card(Card.Colors.Diamonds, "8"),
                new Card(Card.Colors.Diamonds, "9"),
                new Card(Card.Colors.Diamonds, "X"),
                new Card(Card.Colors.Diamonds, "J"),
            });

            Assert.AreEqual(Hand.Weights.StraightFlush, hand.Weight);
            Assert.AreEqual(5, hand.Cards.Length);
        }

        [TestMethod]
        public void TestHandStraightFlushWithAce()
        {
            Hand hand = new Hand(new Card[] {
                new Card(Card.Colors.Diamonds, "A"),
                new Card(Card.Colors.Diamonds, "2"),
                new Card(Card.Colors.Diamonds, "3"),
                new Card(Card.Colors.Diamonds, "4"),
                new Card(Card.Colors.Diamonds, "5"),
            });

            Assert.AreEqual(Hand.Weights.StraightFlush, hand.Weight);
            Assert.AreEqual(5, hand.Cards.Length);
        }

        [TestMethod]
        public void TestHandPoker()
        {
            Hand hand = new Hand(new Card[] {
                new Card(Card.Colors.Clubs, "6"),
                new Card(Card.Colors.Diamonds, "7"),
                new Card(Card.Colors.Spades, "7"),
                new Card(Card.Colors.Hearts, "7"),
                new Card(Card.Colors.Clubs, "7"),
            });

            Assert.AreEqual(Hand.Weights.FourOfKind, hand.Weight);
            Assert.AreEqual(4, hand.Cards.Length);
            Assert.IsTrue(hand.Cards[0].Sign == "7" && hand.Cards[0].Color == Card.Colors.Diamonds);
            Assert.IsTrue(hand.Cards[1].Sign == "7" && hand.Cards[1].Color == Card.Colors.Spades);
            Assert.IsTrue(hand.Cards[2].Sign == "7" && hand.Cards[2].Color == Card.Colors.Hearts);
            Assert.IsTrue(hand.Cards[3].Sign == "7" && hand.Cards[3].Color == Card.Colors.Clubs);
        }

        [TestMethod]
        public void TestHandFlush()
        {
            Hand hand = new Hand(new Card[] {
                new Card(Card.Colors.Diamonds, "6"),
                new Card(Card.Colors.Diamonds, "7"),
                new Card(Card.Colors.Diamonds, "8"),
                new Card(Card.Colors.Diamonds, "X"),
                new Card(Card.Colors.Diamonds, "A"),
            });

            Assert.AreEqual(Hand.Weights.Flush, hand.Weight);
            Assert.AreEqual(5, hand.Cards.Length);
        }

        [TestMethod]
        public void TestHandStraight()
        {
            Hand hand = new Hand(new Card[] {
                new Card(Card.Colors.Diamonds, "A"),
                new Card(Card.Colors.Clubs, "2"),
                new Card(Card.Colors.Diamonds, "3"),
                new Card(Card.Colors.Diamonds, "4"),
                new Card(Card.Colors.Clubs, "5"),
            });

            Assert.AreEqual(Hand.Weights.Straight, hand.Weight);
            Assert.AreEqual(5, hand.Cards.Length);
        }

        [TestMethod]
        public void TestHandThreeOfKind()
        {
            Hand hand = new Hand(new Card[] {
                new Card(Card.Colors.Diamonds, "A"),
                new Card(Card.Colors.Clubs, "2"),
                new Card(Card.Colors.Diamonds, "Q"),
                new Card(Card.Colors.Hearts, "Q"),
                new Card(Card.Colors.Clubs, "Q"),
            });

            Assert.AreEqual(Hand.Weights.ThreeOfKind, hand.Weight);
            Assert.AreEqual(3, hand.Cards.Length);
            Assert.IsTrue(hand.Cards[0].Sign == "Q" && hand.Cards[0].Color == Card.Colors.Diamonds);
            Assert.IsTrue(hand.Cards[1].Sign == "Q" && hand.Cards[1].Color == Card.Colors.Hearts);
            Assert.IsTrue(hand.Cards[2].Sign == "Q" && hand.Cards[2].Color == Card.Colors.Clubs);
        }

        [TestMethod]
        public void TestHandPair()
        {
            Hand hand = new Hand(new Card[] {
                new Card(Card.Colors.Diamonds, "A"),
                new Card(Card.Colors.Clubs, "2"),
                new Card(Card.Colors.Diamonds, "X"),
                new Card(Card.Colors.Hearts, "Q"),
                new Card(Card.Colors.Clubs, "Q"),
            });

            Assert.AreEqual(Hand.Weights.OnePair, hand.Weight);
            Assert.AreEqual(2, hand.Cards.Length);
            Assert.IsTrue(hand.Cards[0].Sign == "Q" && hand.Cards[0].Color == Card.Colors.Hearts);
            Assert.IsTrue(hand.Cards[1].Sign == "Q" && hand.Cards[1].Color == Card.Colors.Clubs);
        }

        [TestMethod]
        public void TestHandHighCard()
        {
            Hand hand = new Hand(new Card[] {
                new Card(Card.Colors.Diamonds, "A"),
                new Card(Card.Colors.Clubs, "2"),
                new Card(Card.Colors.Diamonds, "X"),
                new Card(Card.Colors.Hearts, "Q"),
                new Card(Card.Colors.Clubs, "K"),
            });

            Assert.AreEqual(Hand.Weights.HighCard, hand.Weight);
            Assert.AreEqual(1, hand.Cards.Length);
            Assert.IsTrue(hand.Cards[0].Sign == "K" && hand.Cards[0].Color == Card.Colors.Clubs);
        }
    }
}
