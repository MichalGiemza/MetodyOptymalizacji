using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    class Program
    {
        static int citiesCount = 6;
        static int[,] m = new int[,]
            {
                { int.MaxValue, 3, 93, 13, 33, 9},
                { 4, int.MaxValue, 77, 42, 21, 16},
                { 45, 17, int.MaxValue, 36, 16, 28},
                { 39, 90, 80, int.MaxValue, 56, 7},
                { 28, 46, 88, 33, int.MaxValue, 25},
                { 3, 88, 18, 46, 92, int.MaxValue}
            };
        static int seed = 0;

        // Sprawdzic czy geny nie są modyfikowane między osobnikami (kopiowanie przy Genes.GetRange).
        static void Main(string[] args)
        {
            GeneticAlgorithm<int> ga = new GeneticAlgorithm<int>(
                GeneticAlgorithm<int>.RankingSelection,
                GeneticAlgorithm<int>.PMXCrossover,
                GeneticAlgorithm<int>.SwapMutation,
                FitnessFunction,
                CreateNewSolution,
                AcceptResult,
                100,
                0.1,
                40,
                10
                );

            var result = ga.Solve();
            Console.WriteLine(result.Fitness);

            for (int i = 0; i < result.Chromosomes[0].Genes.Count; i++)
            {
                Console.Write(" " + result.Chromosomes[0].Genes[i]);
            }
            Console.ReadLine();
        }
        
        static IComparable FitnessFunction(Solution<int> a)
        {
            var cities = a.Chromosomes[0].Genes;

            int sum = 0;
            for (int i = 1; i < cities.Count; i++)
            {
                sum += m[cities[i - 1], cities[i]];
            }
            sum += m[cities[cities.Count - 1], cities[0]];

            return sum;
        }
        static Solution<int> CreateNewSolution()
        {
            Random r = new Random(seed);
            List<int> cities = new List<int>();
            List<int> genes = new List<int>();

            for (int i = 0; i < citiesCount; i++)
                cities.Add(i);
            
            while (cities.Count > 0)
            {
                int pos = r.Next(cities.Count);
                int city = cities[pos];
                cities.RemoveAt(pos);

                genes.Add(city);
            }

            seed = r.Next();

            return new Solution<int>(
                new List<Chromosome<int>>() {
                    new Chromosome<int>(genes)
                });
        }
        static bool AcceptResult(Solution<int> bestSolution, int generation)
        {
            if ((int)bestSolution.Fitness < 80 || generation > 100)
                return true;
            return false;
        }
    }
}