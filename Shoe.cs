using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackjackSandbox
{
    public enum CountMethod
    {
        None = 6, 
        HiLo = 0,
        HiOptI = 1,
        HiOptII = 2,
        KO = 3,
        OmegaII = 4,
        ZenCount = 5,
    }

    /// <summary>
    /// Represents a shoe of cards
    /// </summary>
    public class Shoe
    {
        int m_deckCount;

        CountMethod m_countMethod = CountMethod.None;
        //Source: http://en.wikipedia.org/wiki/Card_counting
        static readonly Dictionary<CountMethod, int[]> s_countModifiers = new Dictionary<CountMethod, int[]>()
        {
            { CountMethod.None,       new int[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 } },

            { CountMethod.HiLo,       new int[] { 01, 01, 01, 01, 01, 00, 00, 00, -1, -1 } },
            { CountMethod.HiOptI,     new int[] { 00, 01, 01, 01, 01, 00, 00, 00, -1, 00 } },
            { CountMethod.HiOptII,    new int[] { 01, 01, 02, 02, 01, 01, 00, 00, -2, 00 } },
            { CountMethod.KO,         new int[] { 01, 01, 01, 01, 01, 01, 00, 00, -1, -1 } },
            { CountMethod.OmegaII,    new int[] { 01, 01, 02, 02, 02, 01, 00, -1, -2, 00 } },
            { CountMethod.ZenCount,   new int[] { 01, 01, 02, 02, 02, 01, 00, 00, -2, -1 } },
        };


        List<Card> m_cards = new List<Card>();

        /// <summary>
        /// Gets the current count of the shoe
        /// </summary>
        public int Count
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of cards left in the shoe
        /// </summary>
        public int CardsLeft
        {
            get { return m_cards.Count; }
        }


        /// <summary>
        /// Create a new shoe
        /// </summary>
        /// <param name="deckCount">The amount of decks to use in the shoe</param>
        /// <param name="countMethod">The count method to use for card counting</param>
        public Shoe(int deckCount, CountMethod countMethod)
        {
            m_deckCount = deckCount;
            m_countMethod = countMethod;

            Initialise();
        }


        void Initialise()
        {
            Deck deck = new Deck();

            for (int i = 0; i < m_deckCount; i++)
            {
                //Shuffle the deck
                deck.Shuffle();

                //Insert the deck into the shoe
                foreach (Card c in deck.Cards)
                {
                    m_cards.Add(c);
                }
            }

            //Shuffle the whole shoe
            Shuffle();
        }

        /// <summary>
        /// Gets the card at the front of the shoe and updates the count of the shoe
        /// </summary>
        /// <returns>The value of the card removed from the shoe</returns>
        public Card DealNextCard()
        {
            Card c = null;

            //Pop the front card
            if (m_cards.Count > 0)
            {
                c = m_cards[0];
                m_cards.RemoveAt(0);
            }

            if (c == null)
            {
                Initialise();
                c = DealNextCard();
            }

            //Update the count based on the shoe's counting method
            int cardValueIndex = HandHelper.GetCardValue(c.Value) - 2;
            Count += s_countModifiers[m_countMethod][cardValueIndex];

            return c;
        }


        /// <summary>
        /// Shuffle the whole shoe
        /// </summary>
        public void Shuffle()
        {
            for (int i = 0; i < m_cards.Count; i++)
            {
                //Swap the card with a random one in the shoe
                int randomIndex = Program.RNG.Next(m_cards.Count);

                Card intermediate = m_cards[i];
                m_cards[i] = m_cards[randomIndex];
                m_cards[randomIndex] = intermediate;
            }
        }
    }
}
