using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackjackSandbox
{
    /// <summary>
    /// A blackjack entity that prompts the user for actions
    /// </summary>
    public class InteractiveEntity : Entity
    {
        public InteractiveEntity(int startCash)
            : base(startCash, "Player")
        {

        }

        /// <summary>
        /// Prompt the user for a bet
        /// </summary>
        /// <param name="count">The current count of the shoe</param>
        /// <returns>The amount that this entity wishes to bet</returns>
        public override int GetBet(int count)
        {
            int bet = 0;

            while (true)
            {
                Console.Write("Please input your bet (the current count is {0}): ", count);
                string input = Console.ReadLine();
                if (int.TryParse(input, out bet))
                {
                    break;
                }
            }

            return bet;
        }

        /// <summary>
        /// Prompt the user for an action
        /// </summary>
        /// <param name="hand">The user's current hand</param>
        /// <param name="dealerHoleCard">The dealer's hole card</param>
        /// <returns>The action that the user would like to perform</returns>
        public override TurnAction TakeTurn(List<Card> hand, Card dealerHoleCard)
        {
            while (true)
            {
                Console.Write("Would you like to [S]tick, [H]it, [D]ouble or S[P]lit?: ");

                switch (Console.ReadKey().KeyChar)
                {
                    case 's':
                    case 'S':
                        Console.WriteLine();
                        return TurnAction.Stick;
                        
                    case 'h':
                    case 'H':
                        Console.WriteLine();
                        return TurnAction.Hit;

                    case 'd':
                    case 'D':
                        Console.WriteLine();
                        return TurnAction.Double;

                    case 'p':
                    case 'P':
                        Console.WriteLine();
                        return TurnAction.Split;

                    default:
                        Console.WriteLine();
                        break;

                }
            }
        }

        /// <summary>
        /// Prompts the user for the amount they would like to double down for
        /// </summary>
        /// <param name="hand">The user's current hand</param>
        /// <param name="count">The current count of the shoe</param>
        /// <returns></returns>
        public override int GetDoubleDownBet(List<Card> hand, int count)
        {
            int bet = 0;

            while (true)
            {
                Console.Write("Please input your double down bet: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out bet))
                {
                    break;
                }
                Console.WriteLine();
            }

            return bet;
        }
    }
}
