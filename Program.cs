using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackjackSandbox
{
    class Program
    {
        static Random s_rng = new Random(1);
        public static Random RNG { get { return s_rng; } }

        static void Main(string[] args)
        {
            const int BET_AMOUNT = 10; //flat rate bet
            const int BANKROLL = 100000; //initial starting money
            const int TIME_TO_SLEEP_BETWEEN_HANDS = 1000; //Stop a while each hand so you can read the output
            const int HAND_LIMIT = int.MaxValue; //Stop after playing this many hands
            const int CASH_LIMIT = int.MaxValue; //Stop after gaining this much cash
            const int NUMBER_OF_DECKS_IN_SHOE = 6;


            DateTime startTime = DateTime.Now;

            BlackjackGame t = new BlackjackGame(true, NUMBER_OF_DECKS_IN_SHOE, CountMethod.HiLo, TIME_TO_SLEEP_BETWEEN_HANDS);

            t.AddPlayer(new BasicStrategyAIEntity(2, BANKROLL, BET_AMOUNT, 9999));

            try
            {
                Entity e = t.RunGame(HAND_LIMIT, CASH_LIMIT);

                int estimatedHours = t.HandsPlayed / 250 / 6;
                Console.WriteLine("Gratz! You've turned £25,000 into £50,000! It only took {1} days (playing 6 hours a day, 250 hands an hour).", (DateTime.Now - startTime).ToString(), estimatedHours);
            }
            catch (OutOfMoneyException)
            {
                Console.WriteLine("Ran out of money in " + (DateTime.Now - startTime).ToString());
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
