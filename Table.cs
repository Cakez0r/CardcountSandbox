using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BlackjackSandbox
{
    /// <summary>
    /// Represents a game of blackjack
    /// </summary>
    public class BlackjackGame
    {
        int m_sleepTimeBetweenhands;

        int m_deckCount;

        DealerEntity m_dealer;

        Shoe m_shoe;

        List<Entity> m_entities = new List<Entity>();

        List<Hand> m_hands;

        CountMethod m_countMethod;

        /// <summary>
        /// Creates a new blackjack game
        /// </summary>
        /// <param name="dealerHitSoftSeventeen">Should the dealer hit on soft 17s?</param>
        /// <param name="deckCount">How many decks to use in the shoe</param>
        /// <param name="countMethod">The counting method to use on the shoe</param>
        /// <param name="sleepTimeBetweenHands">The amount of milliseconds to sleep between each hand</param>
        public BlackjackGame(bool dealerHitSoftSeventeen, int deckCount, CountMethod countMethod, int sleepTimeBetweenHands)
        {
            m_dealer = new DealerEntity(dealerHitSoftSeventeen);
            m_deckCount = deckCount;
            m_countMethod = countMethod;

            m_sleepTimeBetweenhands = sleepTimeBetweenHands;
        }

        /// <summary>
        /// Add an entity into this game
        /// </summary>
        /// <param name="e">The entity to add</param>
        public void AddPlayer(Entity e)
        {
            m_entities.Add(e);
        }

        int hands = 0;

        /// <summary>
        /// Advance the game by one round, querying all entities for their actions until their hand is complete
        /// before playing the dealer's hand
        /// </summary>
        /// <returns>Should another round be played after this one?</returns>
        public bool PlayRound()
        {
            //OutputLine(5, "");

            //If the shoe is running low or not created yet, create it
            if (m_shoe == null || m_shoe.CardsLeft < 25)
            {
                OutputLine(2, "Under 15 cards left in the shoe. Creating a new shoe.");
                OutputLine(5, "");
                m_shoe = new Shoe(m_deckCount, m_countMethod);
            }

            //Clear all hands
            m_hands = new List<Hand>();

            //Take bets from players
            foreach (Entity e in m_entities)
            {
                int bet = 0;

                //If the entity isn't a dealer, get their bet
                if (!(e is DealerEntity))
                {
                    bet = e.GetBet(m_shoe.Count);
                    OutputLine(1, "{0} bets {1}. The count is {2}", e.Name, bet, m_shoe.Count);
                    e.Balance -= bet;
                }

                //Deal the entity a hand 
                Hand hand = new Hand(e, m_shoe.DealNextCard(), m_shoe.DealNextCard(), bet);
                m_hands.Add(hand);

                OutputLine(1, "{0} receives {1} and {2}", e.Name, hand.Cards[0].ToShortString(), hand.Cards[1].ToShortString());
            }

            //Find the dealers hand
            Hand dealerHand = m_hands.First(hand => hand.Owner is DealerEntity);

            //Declare the dealer's hole card
            OutputLine(1, "{0}'s hole card is {1}", m_dealer.Name, dealerHand.Cards[0].ToShortString());

            //Now evaluate each hand in the game
            for (int i = 0; i < m_hands.Count; i++)
            {
                Hand currentHand = m_hands[i];

                //Output who's turn it is to act and what their hand is
                OutputLine(1, "{0}'s turn to act on {1}...", currentHand.Owner.Name, currentHand.ToShortString());

                //Keep getting the hand owner's chosen action until their hand is complete
                //No enforcement of game rules is done here! It's up to the entities to play nicely :)
                while (true)
                {
                    TurnAction action = currentHand.Owner.TakeTurn(currentHand.Cards, dealerHand.Cards[0]);

                    if (action == TurnAction.Stick)
                    {
                        //If they stuck, output their final hand and end their turn
                        OutputLine(1, "{0} sticks with {1}", currentHand.Owner.Name, currentHand.ToShortString());
                        break;
                    }
                    else if (action == TurnAction.Hit)
                    {
                        //If they hit, add a card on to their hand
                        Card hitCard = m_shoe.DealNextCard();
                        currentHand.Cards.Add(hitCard);
                        OutputLine(1, "{0} hits and receives a {1}. New hand is {2}", currentHand.Owner.Name, hitCard.ToShortString(), currentHand.ToShortString());
                    }
                    else if (action == TurnAction.Split)
                    {
                        //If the entity wishes to split their hand, create them a new hand with one card from their current hand + a new one
                        Hand newHand = new Hand(currentHand.Owner, currentHand.Cards[1], m_shoe.DealNextCard(), currentHand.Bet);

                        //Make them pay for the hand
                        currentHand.Owner.Balance -= currentHand.Bet;

                        //Flag both of these hands as having been split
                        newHand.IsSplit = true;
                        currentHand.IsSplit = true;

                        //Add the new hand in to take its turn after the current hand
                        m_hands.Insert(i + 1, newHand);

                        //Replace the card that was split out from this hand with a new card
                        currentHand.Cards[1] = m_shoe.DealNextCard();

                        //Output what their hand is now
                        OutputLine(1, "{0} splits. Current hand is now{1}", currentHand.Owner.Name, currentHand.ToShortString());
                    }
                    else if (action == TurnAction.Double)
                    {
                        //If the entity wishes to double down, find out how much they want to double down for and bill them for it
                        int doubleBet = currentHand.Owner.GetDoubleDownBet(currentHand.Cards, m_shoe.Count);
                        currentHand.Bet += doubleBet;
                        currentHand.Owner.Balance -= doubleBet;

                        //Deal them the card they bought
                        Card doubleCard = m_shoe.DealNextCard();
                        currentHand.Cards.Add(doubleCard);

                        //Output their action and the card they got
                        OutputLine(1, "{0} doubles down for an extra {1} and receives a {2}. Final hand is {3}", currentHand.Owner.Name, doubleBet, doubleCard.ToShortString(), currentHand.ToShortString());

                        //End their turn
                        break;
                    }

                    if (HandHelper.GetHandValue(currentHand.Cards.AsReadOnly()) > 21)
                    {
                        //If the entity is now bust, end their turn
                        OutputLine(1, "{0} is bust.", currentHand.Owner.Name);
                        break;
                    }
                }
            }

            //When all hands have been played, evaluate the cash prizes

            //Evaluate the dealer's hand
            int dealerTotal = HandHelper.GetHandValue(dealerHand.Cards.AsReadOnly());
            bool dealerBlackjack = HandHelper.IsBlackjack(dealerHand);

            foreach (Hand hand in m_hands)
            {
                //If it's not the dealer's hand...
                if (hand != dealerHand)
                {
                    //Output the hand match-up
                    OutputLine(2, "{0} vs {1}: ", hand.ToShortString(), dealerHand.ToShortString());

                    //Calcuate this hands value
                    int handTotal = HandHelper.GetHandValue(hand.Cards.AsReadOnly());

                    if (dealerTotal > 21 && handTotal <= 21)
                    {
                        //If the dealer is bust and this hand is not, this hand wins
                        hand.Owner.Balance += hand.Bet * 2;
                        OutputLine(3, "Dealer is bust. {0} wins {1}", hand.Owner.Name, hand.Bet * 2);
                    }
                    else if (HandHelper.IsBlackjack(hand))
                    {
                        if (dealerBlackjack)
                        {
                            //If both this hand and the dealer have blackjack, it's a tie
                            hand.Owner.Balance += hand.Bet;
                            OutputLine(3, "{0} and the dealer have blackjack. {1} receives {2}", hand.Owner.Name, hand.Owner.Name, hand.Bet);
                        }
                        else
                        {
                            //If this hand has blackjack and the dealer does not, this hand wins a 3:2
                            hand.Owner.Balance += (int)(hand.Bet * 2.5);
                            OutputLine(3, "{0} has blackjack. {1} receives {2}", hand.Owner.Name, hand.Owner.Name, (int)(hand.Bet * 2.5));
                        }
                    }
                    else if (handTotal > dealerTotal && handTotal <= 21)
                    {
                        //If this hands total is greater than the dealers and is not bust, this hand wins
                        hand.Owner.Balance += hand.Bet * 2;
                        OutputLine(3, "{0} has a higher hand value than the dealer. {1} receives {2}", hand.Owner.Name, hand.Owner.Name, hand.Bet * 2);
                    }
                    else if (handTotal == dealerTotal && !dealerBlackjack)
                    {
                        //If this hand is equal to the dealers and the dealer does not have blackjack, this hand wins
                        hand.Owner.Balance += hand.Bet;
                        OutputLine(3, "{0} has tied with the dealer. {1} receives {2}", hand.Owner.Name, hand.Owner.Name, hand.Bet);
                    }
                    else
                    {
                        //In any other case, the dealer has one
                        OutputLine(3, "Dealer has beaten {0}", hand.Owner.Name);
                    }

                    //Output this entitys new balance
                    OutputLine(4, "{0}'s new balance is {1}", hand.Owner.Name, hand.Owner.Balance);
                    if (hands > 100)
                    {
                        OutputLine(10, "{0}", hand.Owner.Balance);
                        hands = 0;
                    }
                    hands++;
                }
            }

            //Output some stats about how each entity is doing
            foreach (Entity entity in m_entities)
            {
                //Don't care about the dealer
                if (entity is DealerEntity)
                {
                    continue;
                }

                OutputLine(5, "");
            }

            Thread.Sleep(m_sleepTimeBetweenhands);

            //Just keep going forever (or likely until an entity throws an OutOfMoneyException)
            bool ret = true;

            return ret;
        }

        public int HandsPlayed;

        /// <summary>
        /// Start the game simulation!
        /// </summary>
        public BasicStrategyAIEntity RunGame(int handLimit, int cashLimit)
        {
            OutputLine(5, "Game Starting...");

            //Add in the dealer
            m_entities.Add(m_dealer);

            BasicStrategyAIEntity aiEntity = (BasicStrategyAIEntity)m_entities.First(e => e is BasicStrategyAIEntity);
            int handsPlayed = 0;

            while (PlayRound() && handsPlayed < handLimit && aiEntity.Balance < cashLimit)
            {
                handsPlayed++;
                HandsPlayed = handsPlayed;
            }

            return aiEntity;
        }

        const int LOG_LEVEL = 0;
        void OutputLine(int logLevel, string text, params object[] obj)
        {
            if (logLevel >= LOG_LEVEL)
            {
                Console.WriteLine(String.Format(text, obj));
            }
        }

        void Output(int logLevel, string text, params object[] obj)
        {
            if (logLevel >= LOG_LEVEL)
            {
                Console.Write(String.Format(text, obj));
            }
        }
    }
}
