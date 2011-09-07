using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackjackSandbox
{
    public enum SuitName
    {
        Heart = 0, Diamond = 1, Spade = 2, Club = 3
    }


    public enum CardName
    {
        Ace = 0, Two = 1, Three = 2, Four = 3, Five = 4, Six = 5, Seven = 6, Eight = 7, Nine = 8, Ten = 9, Jack = 10, Queen = 11, King = 12
    }


    /// <summary>
    /// Represents a card
    /// </summary>
    public class Card
    {
        public SuitName Suit { get; private set; }
        public CardName Value { get; private set; }

        public Card(SuitName suit, CardName value)
        {
            Suit = suit;
            Value = value;
        }

        /// <summary>
        /// Gets this cards 2-character string representation
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            string name = "";

            switch (Value)
            {
                case CardName.Ace:
                    name += "A";
                    break;

                case CardName.Jack:
                    name += "J";
                    break;

                case CardName.Queen:
                    name += "Q";
                    break;

                case CardName.King:
                    name += "K";
                    break;

                case CardName.Ten:
                    name += "T";
                    break;
                default:
                    name += HandHelper.GetCardValue(Value);
                    break;
            }

            switch (Suit)
            {
                case SuitName.Club:
                    name += "♣";
                    break;

                case SuitName.Diamond:
                    name += "♦";
                    break;

                case SuitName.Heart:
                    name += "♥";
                    break;

                case SuitName.Spade:
                    name += "♠";
                    break;
            }

            return name;
        }
    }


    /// <summary>
    /// Represents a standard deck of 52 cards
    /// </summary>
    public class Deck
    {
        Card[] m_cards = new Card[52];

        public Card[] Cards
        {
            get { return m_cards; }
        }

        /// <summary>
        /// Creates an unshuffled deck of cards
        /// </summary>
        public Deck()
        {
            int cardNumber = 0;
            for (int suit = 0; suit < 4; suit++)
            {
                for (int card = 0; card < 13; card++)
                {
                    m_cards[cardNumber++] = new Card((SuitName)suit, (CardName)card);
                }
            }
        }


        /// <summary>
        /// Shuffle the deck
        /// </summary>
        public void Shuffle()
        {
            //Is this sufficient?
            for (int i = 0; i < m_cards.Length; i++)
            {
                Swap(ref m_cards[i], ref m_cards[Program.RNG.Next(52)]);
            }
        }


        /// <summary>
        /// Swaps two cards
        /// </summary>
        static void Swap(ref Card a, ref Card b)
        {
            Card intermediate = a;
            a = b;
            b = intermediate;
        }
    }
}
