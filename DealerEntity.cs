using System;
using System.Collections.Generic;

namespace BlackjackSandbox
{
    /// <summary>
    /// An entity that will play like a dealer would
    /// </summary>
    public class DealerEntity : Entity
    {
        bool m_hitSoftSeventeen;

        /// <summary>
        /// Creates a new entity that will play as a dealer would
        /// </summary>
        /// <param name="hitSoftSeventeen">Should this entity hit on soft 17?</param>
        public DealerEntity(bool hitSoftSeventeen) : base(0, "Dealer")
        {
            m_hitSoftSeventeen = hitSoftSeventeen;
        }


        /// <summary>
        /// Dealers dont bet!
        /// </summary>
        public override int GetBet(int count)
        {
            throw new NotSupportedException();
        }


        public override TurnAction TakeTurn(List<Card> hand, Card dealerHoleCard)
        {
            bool soft;
            int value = HandHelper.GetHandValue(hand.AsReadOnly(), out soft);

            //If we're less than 17, always hit
            if (value < 17)
            {
                return TurnAction.Hit;
            }
            //If we're at 17, soft and should hit on soft 17s... 
            else if (value == 17 && soft && m_hitSoftSeventeen)
            {
                return TurnAction.Hit;
            }

            return TurnAction.Stick;
        }


        /// <summary>
        /// Dealers don't double down!
        /// </summary>
        public override int GetDoubleDownBet(List<Card> hand, int count)
        {
            throw new NotSupportedException();
        }
    }
}
