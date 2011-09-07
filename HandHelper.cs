using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BlackjackSandbox
{
    /// <summary>
    /// Helper methods for evaluating hands
    /// </summary>
    public static class HandHelper
    {
        static readonly Dictionary<CardName, int> s_cardValues = new Dictionary<CardName, int>()
        {
            { CardName.Ace, 11 },
            { CardName.Two, 2 },
            { CardName.Three, 3 },
            { CardName.Four, 4 },
            { CardName.Five, 5 },
            { CardName.Six, 6 },
            { CardName.Seven, 7 },
            { CardName.Eight, 8 },
            { CardName.Nine, 9 },
            { CardName.Ten, 10 },
            { CardName.Jack, 10 },
            { CardName.Queen, 10 },
            { CardName.King, 10 },
        };


        /// <summary>
        /// Gets the value of a card
        /// </summary>
        /// <param name="c">The card value to check</param>
        /// <returns>The value of the specified card</returns>
        public static int GetCardValue(CardName c)
        {
            return s_cardValues[c];
        }

        /// <summary>
        /// Gets the value of a hand
        /// </summary>
        /// <param name="hand">The hand to evaluate</param>
        /// <returns>The value of the specified hand</returns>
        public static int GetHandValue(ReadOnlyCollection<Card> hand)
        {
            bool soft;

            return GetHandValue(hand, out soft);
        }

        /// <summary>
        /// Gets the value of a hand and whether it is soft or not
        /// </summary>
        /// <param name="hand">The hand to evaluate</param>
        /// <param name="soft">[OUT] Is the hand's value soft?</param>
        /// <returns>The value of the specified hand</returns>
        public static int GetHandValue(ReadOnlyCollection<Card> hand, out bool soft)
        {
            int total = 0;

            foreach (Card c in hand)
            {
                total += s_cardValues[c.Value];
            }

            int aceCount = hand.Count(c => c.Value == CardName.Ace);
            while (total > 21 && aceCount > 0)
            {
                total -= 10;
                aceCount--;
            }

            soft = aceCount > 0;

            return total;
        }


        /// <summary>
        /// Checks if a hand is blackjack or not
        /// </summary>
        /// <param name="hand">The hand to evaluate</param>
        /// <returns>Whether the hand is blackjack or not</returns>
        public static bool IsBlackjack(Hand hand)
        {
            return !hand.IsSplit && hand.Cards.Count == 2 && hand.Cards.Count(c => c.Value == CardName.Ace) > 0 && GetHandValue(hand.Cards.AsReadOnly()) == 21;
        }
    }
}
