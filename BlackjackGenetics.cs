using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackjackSandbox
{
    public class Chromosome
    {
        public CountMethod CountMethod { get; set; }
        public int CountScalar { get; set; }
        public int BetSize { get; set; }

        public double Fitness { get; set; }
        public double TimeAchieved { get; set; }
        public int BalanceAchieved { get; set; }

        public Chromosome(CountMethod countMethod, int countScalar, int betSize)
        {
            CountMethod = countMethod;
            CountScalar = countScalar;
            BetSize = betSize;
        }
    }

    public class BlackjackGenetics
    {
        static readonly CountMethod[] m_countMethods = (CountMethod[])Enum.GetValues(typeof(CountMethod));

        const int BANKROLL = 5000;

        int m_generation = 1;

        List<Chromosome> m_populationA;
        List<Chromosome> m_populationB;

        List<Chromosome> m_currentPopulation;
        List<Chromosome> m_breedingPool;

        double m_highestSeenFitness = double.MinValue;
        BasicStrategyAIEntity m_fittestEntityResult;
        Chromosome m_fittestChromosome;

        public BlackjackGenetics()
        {
            m_populationA = GeneratePopulation(50);
            m_populationB = new List<Chromosome>();

            m_currentPopulation = m_populationA;
            m_breedingPool = m_populationB;

            while (true)
            {
                BreedNewGeneration();
            }
        }


        List<Chromosome> GeneratePopulation(int count)
        {
            List<Chromosome> ret = new List<Chromosome>();

            for (int i = 0; i < count; i++)
            {
                Chromosome chromosome = new Chromosome(m_countMethods[Program.RNG.Next(m_countMethods.Length)],
                    2, /* Program.RNG.Next(1, 21), /* Count Scalar */
                    5+i /* Bet size */
                );

                chromosome.Fitness = TestFitness(chromosome);

                Console.WriteLine(i);

                ret.Add(chromosome);
            }

            return ret;
        }

        double TestFitness(Chromosome chromosome)
        {
            DateTime startTime = DateTime.Now;

            BlackjackGame game = new BlackjackGame(true, 6, chromosome.CountMethod, 0);
            game.AddPlayer(new BasicStrategyAIEntity(chromosome.CountScalar, BANKROLL, chromosome.BetSize, 9999));

            BasicStrategyAIEntity entity = game.RunGame(10000, BANKROLL * 100);

            TimeSpan timeTaken = DateTime.Now - startTime;

            double fitness = (double)entity.Balance;// *Math.Max(10000 - (float)timeTaken.TotalMilliseconds, 0);

            chromosome.TimeAchieved = timeTaken.TotalMilliseconds;
            chromosome.BalanceAchieved = entity.Balance;

            if (fitness > m_highestSeenFitness)
            {
                m_highestSeenFitness = fitness;
                m_fittestEntityResult = entity;
                m_fittestChromosome = chromosome;
            }

            return fitness;
        }

        Chromosome[] GetRandomPair()
        {
            int a = Program.RNG.Next(m_currentPopulation.Count);
            int b = a;

            while (b == a)
            {
                b = Program.RNG.Next(m_currentPopulation.Count);
            }

            return new Chromosome[] { m_currentPopulation[a], m_currentPopulation[b] };
        }

        void BreedNewGeneration()
        {
            while (m_breedingPool.Count < m_currentPopulation.Count)
            {
                Chromosome[] pairA = GetRandomPair();
                Chromosome[] pairB = GetRandomPair();

                Chromosome fittestA = pairA[0].Fitness > pairA[1].Fitness ? pairA[0] : pairA[1];
                Chromosome fittestB = pairB[0].Fitness > pairB[1].Fitness ? pairB[0] : pairB[1];

                m_breedingPool.Add(Breed(fittestA, fittestB));
                m_breedingPool.Add(Breed(fittestA, fittestB));

                //Console.WriteLine(m_breedingPool.Count);
            }

            Console.WriteLine("Generation {0} complete.", m_generation);
            Console.WriteLine("Average balance: {0}", m_currentPopulation.Average(c => c.BalanceAchieved));
            Console.WriteLine("Average time: {0}", m_currentPopulation.Average(c => c.TimeAchieved));
            Console.WriteLine("Average fitness: {0}", m_currentPopulation.Average(c => c.Fitness));
            Console.WriteLine("Average Bet: {0}", m_currentPopulation.Average(c => c.BetSize));
            Console.WriteLine("Average Count Scalar: {0}", m_currentPopulation.Average(c => c.CountScalar));
            Console.WriteLine();

            m_currentPopulation = m_breedingPool;
            m_breedingPool = new List<Chromosome>();

            m_generation++;
        }

        Chromosome GetFittest(Chromosome a, Chromosome b)
        {
            double aFitness = TestFitness(a);
            double bFitness = TestFitness(b);

            return aFitness > bFitness ? a : b;
        }

        Chromosome Breed(Chromosome a, Chromosome b)
        {
            bool mutateBet = Program.RNG.Next(500) == 0;
            bool mutateScalar = Program.RNG.Next(500) == 0;

            Chromosome child = new Chromosome(m_countMethods[Program.RNG.Next(m_countMethods.Length)],
                Program.RNG.Next(2) == 0 ? a.CountScalar : b.CountScalar,
                Program.RNG.Next(2) == 0 ? a.BetSize : b.BetSize);


            child.Fitness = TestFitness(child);

            return child;
        }
    }
}
