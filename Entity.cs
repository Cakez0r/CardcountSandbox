using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BlackjackSandbox
{
    public enum TurnAction
    {
        Hit, Split, Stick, Double
    }

    /// <summary>
    /// Notify the game that an entity has run out of cashmoniez
    /// </summary>
    public class OutOfMoneyException : Exception 
    {
        /// <summary>
        /// The entity that has run out of money
        /// </summary>
        public Entity Entity { get; private set; }

        /// <summary>
        /// Create a new out of money exception
        /// </summary>
        /// <param name="e">The entity that has run out of money</param>
        public OutOfMoneyException(Entity e) : base("Player has run out of money.")
        {
            Entity = e;
        }
    }


    /// <summary>
    /// Base class of all entities who can play blackjack
    /// </summary>
    public abstract class Entity
    {
        public string Name { get; private set; }

        public int CashOut { get; private set; }
        public int CashIn { get; private set; }
        public double CashInOutRatio 
        {
            get { return (double)CashIn / CashOut; }
        }

        int m_balance;
        public int Balance 
        {
            get { return m_balance; }
            set 
            {
                int diff = value - m_balance;

                //If our balance is going up, add it on to CashIn
                if (diff > 0)
                {
                    CashIn += diff;
                }
                //If our balance is going down, add it on to CashOut
                else
                {
                    CashOut -= diff;
                }

                //If we're out of money, then shout about it :)
                if (m_balance <= 0)
                {
                    throw new OutOfMoneyException(this);
                }

                m_balance = value; 
            }
        }


        /// <summary>
        /// Creates a blackjack entity
        /// </summary>
        /// <param name="initialBalance">The starting cash for this entity</param>
        /// <param name="name">The friendly name of this entity</param>
        public Entity(int initialBalance, string name)
        {
            m_balance = initialBalance;
            Name = name;
        }


        public abstract int GetBet(int count);


        public abstract TurnAction TakeTurn(List<Card> hand, Card dealerHoleCard);


        public abstract int GetDoubleDownBet(List<Card> hand, int count);
    }
}
