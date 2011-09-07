using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackSandbox
{
    /// <summary>
    /// Plays using basic blackjack strategy, assisted by knowing the count of a shoe
    /// </summary>
    public class BasicStrategyAIEntity : Entity
    {
        #region Basic Strategy Actions
        //Source: http://www.blackjackinfo.com/bjbse.php?numdecks=6+decks&soft17=h17&dbl=all&das=yes&surr=ns&peek=yes
        static readonly char[,] s_pairActions = new char[10, 10]
        {  /*  2    3    4    5    6    7    8    9    10   A  */
            { 'p', 'p', 'p', 'p', 'p', 'p', 'h', 'h', 'h', 'h' }, /* 2, 2 */
            { 'p', 'p', 'p', 'p', 'p', 'p', 'h', 'h', 'h', 'h' }, /* 3, 3 */
            { 'h', 'h', 'h', 'p', 'p', 'h', 'h', 'h', 'h', 'h' }, /* 4, 4 */
            { 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'h', 'h' }, /* 5, 5 */
            { 'p', 'p', 'p', 'p', 'p', 'h', 'h', 'h', 'h', 'h' }, /* 6, 6 */
            { 'p', 'p', 'p', 'p', 'p', 'p', 'h', 'h', 'h', 'h' }, /* 7, 7 */
            { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' }, /* 8, 8 */
            { 'p', 'p', 'p', 'p', 'p', 's', 'p', 'p', 's', 's' }, /* 9, 9 */
            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* T, T */
            { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' }, /* A, A */
        };

        static readonly char[,] s_softActions = new char[10, 10]
        {  /*  2    3    4    5    6    7    8    9    10   A  */
            { 'h', 'h', 'h', 'd', 'd', 'h', 'h', 'h', 'h', 'h' }, /* A, 2 */
            { 'h', 'h', 'h', 'd', 'd', 'h', 'h', 'h', 'h', 'h' }, /* A, 3 */
            { 'h', 'h', 'd', 'd', 'd', 'h', 'h', 'h', 'h', 'h' }, /* A, 4 */
            { 'h', 'h', 'd', 'd', 'd', 'h', 'h', 'h', 'h', 'h' }, /* A, 5 */
            { 'h', 'd', 'd', 'd', 'd', 'h', 'h', 'h', 'h', 'h' }, /* A, 6 */
            { 'd', 'd', 'd', 'd', 'd', 's', 's', 'h', 'h', 'h' }, /* A, 7 */
            { 's', 's', 's', 's', 'd', 's', 's', 's', 's', 's' }, /* A, 8 */
            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* A, 9 */

            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* 10 */
            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* A */
        };

        static readonly char[,] s_hardActions = new char[20, 10]
        {  /*  2    3    4    5    6    7    8    9    10   A  */
            { 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h' }, /* 2 */
            { 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h' }, /* 3 */
            { 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h' }, /* 4 */

            { 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h' }, /* 5 */
            { 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h' }, /* 6 */
            { 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h' }, /* 7 */
            { 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h', 'h' }, /* 8 */
            { 'h', 'd', 'd', 'd', 'd', 'h', 'h', 'h', 'h', 'h' }, /* 9 */
            { 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'h', 'h' }, /* 10 */
            { 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd', 'd' }, /* 11 */
            { 'h', 'h', 's', 's', 's', 'h', 'h', 'h', 'h', 'h' }, /* 12 */
            { 's', 's', 's', 's', 's', 'h', 'h', 'h', 'h', 'h' }, /* 13 */
            { 's', 's', 's', 's', 's', 'h', 'h', 'h', 'h', 'h' }, /* 14 */
            { 's', 's', 's', 's', 's', 'h', 'h', 'h', 'h', 'h' }, /* 15 */
            { 's', 's', 's', 's', 's', 'h', 'h', 'h', 'h', 'h' }, /* 16 */
            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* 17 */

            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* 18 */
            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* 19 */
            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* 20 */
            { 's', 's', 's', 's', 's', 's', 's', 's', 's', 's' }, /* 21 */
        };

        static readonly Dictionary<char, TurnAction> s_actionMap = new Dictionary<char, TurnAction>()
        {
            { 's', TurnAction.Stick },
            { 'h', TurnAction.Hit },
            { 'd', TurnAction.Double },
            { 'p', TurnAction.Split },
        };
        #endregion


        int m_lastBet;
        int m_betAmount;
        float m_countScalar;
        float m_maxCountModifier;

        /// <summary>
        /// Creates an entity that will play using basic blackjack strategy assuming a ruleset of:
        /// Dealer hits soft 17
        /// 6 decks in shoe
        /// Double after split allowed
        /// Split on any 2 cards of the same value
        /// No surrender
        /// </summary>
        /// <param name="countScalar">Bet Amount = Standard Bet + (Min(Count, Max Count Modifier) * Count Scalar) * Standard Bet</param>
        /// <param name="bankroll">The amount of money this AI has to toy with</param>
        /// <param name="standardBet">The standard bet that this AI will place</param>
        /// <param name="maxCountModifier">The count modifier limit</param>
        public BasicStrategyAIEntity(float countScalar, int bankroll, int standardBet, float maxCountModifier) : base(bankroll, "AI Player")
        {
            m_betAmount = standardBet;
            m_countScalar = countScalar;
            m_maxCountModifier = maxCountModifier;
        }


        /// <summary>
        /// Gets a bet for this entity
        /// </summary>
        /// <param name="count">The current count of the deck</param>
        /// <returns>The amount this entity will bet</returns>
        public override int GetBet(int count)
        {
            //source: Bellagio http://wizardofvegas.com/guides/blackjack-survey//
            const int MAX_BET = 5000;
            const int MIN_BET = 10;

            float countScalar = Math.Min(m_maxCountModifier, count) * m_countScalar ;
            int countModifier = (int)(countScalar * m_betAmount);
            int betAmount = m_betAmount + countModifier;

            betAmount *= (int)Math.Ceiling((float)Balance / 100000);

            //Enforce table limits
            betAmount = Math.Max(betAmount, MIN_BET);
            betAmount = Math.Min(betAmount, MAX_BET);

            //Note last bet incase we come to double down
            m_lastBet = betAmount;

            return betAmount;
        }


        /// <summary>
        /// Evaluate what this entity will do for its turn
        /// </summary>
        /// <param name="hand">The hand to evaluate</param>
        /// <param name="dealerHoleCard">The dealer's shown card</param>
        /// <returns>What action to take for this turn</returns>
        public override TurnAction TakeTurn(List<Card> hand, Card dealerHoleCard)
        {
            //Is this hand soft?
            bool soft;

            //Get the current count of this hand
            int total = HandHelper.GetHandValue(hand.AsReadOnly(), out soft);

            //Index into the action array for the dealer's hole card
            int holeCardIndex = HandHelper.GetCardValue(dealerHoleCard.Value) - 2;

            //If the hand is a pair....
            if (hand.Count == 2 && hand[0].Value == hand[1].Value)
            {
                //Check what we should do with the pair
                int pairIndexValue = HandHelper.GetCardValue(hand[0].Value) - 2;
                return s_actionMap[s_pairActions[pairIndexValue, holeCardIndex]];
            }
            //If the hand is soft and it's our first move...
            else if (soft && hand.Count == 2)
            {
                //this means we have A + ? in our hand. Find the card that is not an ace
                Card nonAce = hand.First(c => c.Value != CardName.Ace);

                //Check what we should do with this hand...
                int softIndexValue = HandHelper.GetCardValue(nonAce.Value) - 2;
                return s_actionMap[s_softActions[softIndexValue, holeCardIndex]];
            }

            //If none of the above cases match, check what action we should perform based on the hand's value
            return s_actionMap[s_hardActions[total-2, holeCardIndex]];
        }


        /// <summary>
        /// How much should we bet when doubling down?
        /// </summary>
        /// <param name="hand">The hand to evaluate</param>
        /// <param name="count">The current count of the shoe</param>
        /// <returns>How much this entity will bet for a double down</returns>
        public override int GetDoubleDownBet(List<Card> hand, int count)
        {
            //source: Bellagio http://wizardofvegas.com/guides/blackjack-survey//
            const int MAX_BET = 5000;
            const int MIN_BET = 10;

            float countScalar = Math.Min(m_maxCountModifier, count) * m_countScalar;
            int countModifier = (int)(countScalar * m_betAmount);
            int betAmount = m_betAmount + countModifier;

            betAmount *= (int)Math.Ceiling((float)Balance / 100000);

            //Enforce table limits
            betAmount = Math.Max(betAmount, MIN_BET);
            betAmount = Math.Min(betAmount, MAX_BET);

            return Math.Min(betAmount, m_lastBet);
        }
    }
}
