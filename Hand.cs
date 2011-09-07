using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackjackSandbox
{
    /// <summary>
    /// Represents a hand of cards
    /// </summary>
    public class Hand
    {
        /// <summary>
        /// Who owns this hand?
        /// </summary>
        public Entity Owner { get; private set; }

        /// <summary>
        /// The cards in this hand
        /// </summary>
        public List<Card> Cards { get; private set; }

        /// <summary>
        /// What bet has been placed on this hand
        /// </summary>
        public int Bet { get; set; }

        /// <summary>
        /// Is this hand the result of a split?
        /// </summary>
        public bool IsSplit { get; set; }


        public Hand(Entity owner, Card a, Card b, int bet)
        {
            Owner = owner;
            Cards = new List<Card>();
            Cards.Add(a);
            Cards.Add(b);

            Bet = bet;
        }

        /// <summary>
        /// Gets the string representation of this hand (cards, followed by the hand value)
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            string ret = "";

            foreach (Card c in Cards)
            {
                ret += c.ToShortString() + " ";
            }

            ret += string.Format("[{0}]", HandHelper.GetHandValue(Cards.AsReadOnly()));

            return ret;
        }
    }
}
