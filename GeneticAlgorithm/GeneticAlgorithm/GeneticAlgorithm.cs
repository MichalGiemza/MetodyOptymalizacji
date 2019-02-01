using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    public delegate List<Solution<T>> Selection<T>(SortedList<IComparable, Solution<T>> population, int count);
    public delegate Solution<T>[] Crossover<T>(Solution<T> a, Solution<T> b, FitnessFunction<T> fitnessFunction);
    public delegate void Mutation<T>(Solution<T> a, CreateNewSolution<T> createSolution, FitnessFunction<T> fitnessFunction, double mutationProbability);
    public delegate IComparable FitnessFunction<T>(Solution<T> a);
    public delegate Solution<T> CreateNewSolution<T>();
    public delegate bool AcceptResult<T>(Solution<T> bestSolution, int generation);

    public class GeneticAlgorithm<T>
    {
        SortedList<IComparable, Solution<T>> population;
        int maxPopulation;
        int generation;
        double mutationProbability;
        int eliteCount;
        int selectionCount;
        Selection<T> selection;
        Crossover<T> crossover;
        Mutation<T> mutation;
        FitnessFunction<T> fitness;
        CreateNewSolution<T> createSolution;
        AcceptResult<T> accept;

        public GeneticAlgorithm(Selection<T> selectionMethod, Crossover<T> crossoverMethod, Mutation<T> mutationMethod, FitnessFunction<T> fitnessFunction, CreateNewSolution<T> createNewSolution, AcceptResult<T> acceptResult, int population, double mutationProbability, int selectionCount, int eliteCount = 0)
        {
            if (population < 2)
                throw new Exception("Population must be greater or equal 2.");

            if (eliteCount >= population)
                throw new Exception("EliteCount must be less than population.");

            selection = selectionMethod;
            crossover = crossoverMethod;
            mutation = mutationMethod;
            fitness = fitnessFunction;
            createSolution = createNewSolution;
            accept = acceptResult;
            
            maxPopulation = population;
            this.mutationProbability = mutationProbability;
            this.selectionCount = selectionCount;
            this.eliteCount = eliteCount;
        }

        void DebugPrint() // Debug
        {
            foreach (var osobnik in population)
            {
                for (int i = 0; i < osobnik.Value.Chromosomes[0].Genes.Count; i++)
                {
                    Console.Write(" " + osobnik.Value.Chromosomes[0].Genes[i]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public Solution<T> Solve()
        {
            population = CreateNewPopulation();

            DebugPrint(); // Debug

            generation = 1;

            while (accept(population.Values[0], generation) == false)
            {
                population = CreateNewGeneration(population);
                generation += 1;
            }

            DebugPrint(); // Debug

            return population.Values[0];
        }

        public SortedList<IComparable, Solution<T>> CreateNewPopulation()
        {
            SortedList<IComparable, Solution<T>> newPopulation = new SortedList<IComparable, Solution<T>>(new DuplicateKeyComparer<IComparable>());

            for (int i = 0; i < maxPopulation; i++)
            {
                Solution<T> solution = createSolution();
                solution.Fitness = fitness(solution);
                newPopulation.Add(solution.Fitness, solution);
            }

            return newPopulation;
        }
        public SortedList<IComparable, Solution<T>> CreateNewGeneration(SortedList<IComparable, Solution<T>> population)
        {
            Random r = new Random();
            List<Solution<T>> newGeneration = new List<Solution<T>>();
            
            // Selection
            var elite = SelectElite(population, eliteCount);
            var selected = selection(population, selectionCount);

            // Crossover
            int currentPopulation = elite.Count;
            while (currentPopulation < maxPopulation)
            {
                var solution1 = selected[r.Next(selected.Count)];
                var solution2 = selected[r.Next(selected.Count)];
                
                foreach (var solution in crossover(solution1, solution2, fitness))
                {
                    newGeneration.Add(solution);
                    currentPopulation += 1;
                    if (currentPopulation >= maxPopulation)
                        break;
                }
            }

            // Mutation
            foreach (var solution in newGeneration)
            {
                mutation(solution, createSolution, fitness, mutationProbability);
            }

            SortedList<IComparable, Solution<T>> result = new SortedList<IComparable, Solution<T>>(new DuplicateKeyComparer<IComparable>());
            foreach (var solution in elite)
                result.Add(solution.Key, solution.Value);
            foreach (var solution in newGeneration)
                result.Add(solution.Fitness, solution);

            return result;
        }

        #region Selection_methods
        // Elite selection
        public SortedList<IComparable, Solution<T>> SelectElite(SortedList<IComparable, Solution<T>> population, int count)
        {
            SortedList<IComparable, Solution<T>> elite = new SortedList<IComparable, Solution<T>>(new DuplicateKeyComparer<IComparable>());

            for (int i = 0; i < count; i++)
                elite.Add(population.Keys[i], population.Values[i].Clone());

            return elite;
        }

        // Selection
        public static List<Solution<T>> RankingSelection(SortedList<IComparable, Solution<T>> population, int count)
        {
            List<Solution<T>> selected = new List<Solution<T>>();

            for (int i = 0; i < count; i++)
                selected.Add(population.Values[i]);

            return selected;
        }
        #endregion

        #region Crossover_methods
        public static Solution<T>[] SinglePointCrossover(Solution<T> a, Solution<T> b, FitnessFunction<T> fitnessFunction)
        {
            Random r = new Random();

            var ae = a.Chromosomes.GetEnumerator();
            var be = b.Chromosomes.GetEnumerator();

            List<Chromosome<T>> cl1 = new List<Chromosome<T>>();
            List<Chromosome<T>> cl2 = new List<Chromosome<T>>();

            // For each chromosome
            while (ae.MoveNext() && be.MoveNext())
            {
                Chromosome<T> ac = ae.Current;
                Chromosome<T> bc = be.Current;
                
                int point = r.Next(Math.Min(ac.GenesCount, bc.GenesCount));
                
                // Split chromosomes into parts
                var aFrontGenes = ac.Genes.GetRange(0, point);
                var bFrontGenes = bc.Genes.GetRange(0, point);
                var aTailGenes = ac.Genes.GetRange(point, ac.GenesCount - point);
                var bTailGenes = bc.Genes.GetRange(point, bc.GenesCount - point);
                
                // Create new, crossed chromosomes
                List<T> gl1 = new List<T>();
                gl1.AddRange(aFrontGenes);
                gl1.AddRange(bTailGenes);
                cl1.Add(new Chromosome<T>(gl1));

                List<T> gl2 = new List<T>();
                gl2.AddRange(bFrontGenes);
                gl2.AddRange(aTailGenes);
                cl2.Add(new Chromosome<T>(gl2));
            }

            Solution<T> s1 = new Solution<T>(cl1);
            s1.Fitness = fitnessFunction(s1);
            Solution<T> s2 = new Solution<T>(cl2);
            s2.Fitness = fitnessFunction(s2);
            
            return new Solution<T>[] { s1, s2 };
        }
        public static Solution<T>[] DoublePointCrossover(Solution<T> a, Solution<T> b, FitnessFunction<T> fitnessFunction)
        {
            Random r = new Random();

            var ae = a.Chromosomes.GetEnumerator();
            var be = b.Chromosomes.GetEnumerator();

            List<Chromosome<T>> cl1 = new List<Chromosome<T>>();
            List<Chromosome<T>> cl2 = new List<Chromosome<T>>();

            // For each chromosome
            while (ae.MoveNext() && be.MoveNext())
            {
                Chromosome<T> ac = ae.Current;
                Chromosome<T> bc = be.Current;

                int point1 = r.Next(Math.Min(ac.GenesCount, bc.GenesCount));
                int point2 = r.Next(point1, Math.Min(ac.GenesCount, bc.GenesCount));

                // Split chromosomes into parts
                var aFrontGenes = ac.Genes.GetRange(0, point1);
                var bFrontGenes = bc.Genes.GetRange(0, point1);
                var aMiddleGenes = ac.Genes.GetRange(point1, point2 - point1);
                var bMiddleGenes = bc.Genes.GetRange(point1, point2 - point1);
                var aTailGenes = ac.Genes.GetRange(point2, ac.GenesCount - point2);
                var bTailGenes = bc.Genes.GetRange(point2, bc.GenesCount - point2);

                // Create new, crossed chromosomes
                List<T> gl1 = new List<T>();
                gl1.AddRange(aFrontGenes);
                gl1.AddRange(bMiddleGenes);
                gl1.AddRange(aTailGenes);
                cl1.Add(new Chromosome<T>(gl1));

                List<T> gl2 = new List<T>();
                gl2.AddRange(bFrontGenes);
                gl2.AddRange(aMiddleGenes);
                gl2.AddRange(bTailGenes);
                cl2.Add(new Chromosome<T>(gl2));
            }
            
            Solution<T> s1 = new Solution<T>(cl1);
            s1.Fitness = fitnessFunction(s1);
            Solution<T> s2 = new Solution<T>(cl2);
            s2.Fitness = fitnessFunction(s2);
            
            return new Solution<T>[] { s1, s2 };
        }
        public static Solution<T>[] PMXCrossover(Solution<T> a, Solution<T> b, FitnessFunction<T> fitnessFunction)
        {
            Random r = new Random();

            var ae = a.Chromosomes.GetEnumerator();
            var be = b.Chromosomes.GetEnumerator();

            List<Chromosome<T>> cl1 = new List<Chromosome<T>>();
            List<Chromosome<T>> cl2 = new List<Chromosome<T>>();

            // For each chromosome
            while (ae.MoveNext() && be.MoveNext())
            {
                Chromosome<T> ac = ae.Current;
                Chromosome<T> bc = be.Current;

                int point = r.Next(Math.Min(ac.GenesCount, bc.GenesCount));

                // Split chromosomes into parts
                var aFrontGenes = ac.Genes.GetRange(0, point);
                var bFrontGenes = bc.Genes.GetRange(0, point);
                var aTailGenes = ac.Genes.GetRange(point, ac.GenesCount - point);
                var bTailGenes = bc.Genes.GetRange(point, bc.GenesCount - point);

                // Find not genes that are unique in aFrontGenes and bFrontGenes
                Stack<T> uniqueAGenes = new Stack<T>();
                Stack<T> uniqueBGenes = new Stack<T>();
                foreach (T aGene in aFrontGenes)
                {
                    int i = bFrontGenes.FindIndex(x => x.Equals(aGene));

                    if (i < 0)
                        uniqueAGenes.Push(aGene);
                }
                foreach (T bGene in bFrontGenes)
                {
                    int i = aFrontGenes.FindIndex(x => x.Equals(bGene));

                    if (i < 0)
                        uniqueBGenes.Push(bGene);
                }

                // Swap elements in Tail parts
                while (uniqueAGenes.Count > 0 && uniqueBGenes.Count > 0)
                {
                    T aEl = uniqueAGenes.Pop();
                    T bEl = uniqueBGenes.Pop();

                    int pos;

                    pos = aTailGenes.FindIndex(x => x.Equals(bEl));
                    aTailGenes[pos] = aEl;

                    pos = bTailGenes.FindIndex(x => x.Equals(aEl));
                    bTailGenes[pos] = bEl;
                }
                
                // Create new, crossed chromosomes
                List<T> gl1 = new List<T>();
                gl1.AddRange(aFrontGenes);
                gl1.AddRange(bTailGenes);
                cl1.Add(new Chromosome<T>(gl1));

                List<T> gl2 = new List<T>();
                gl2.AddRange(bFrontGenes);
                gl2.AddRange(aTailGenes);
                cl2.Add(new Chromosome<T>(gl2));
            }

            Solution<T> s1 = new Solution<T>(cl1);
            s1.Fitness = fitnessFunction(s1);
            Solution<T> s2 = new Solution<T>(cl2);
            s2.Fitness = fitnessFunction(s2);

            return new Solution<T>[] { s1, s2 };
        }
        #endregion

        #region Mutation_methods
        public static void SwapMutation(Solution<T> a, CreateNewSolution<T> createSolution, FitnessFunction<T> fitnessFunction, double mutationProbability)
        {
            Random r = new Random();
            
            foreach (var c in a.Chromosomes)
            {
                for (int i = 0; i < c.GenesCount; i++)
                {
                    // Probability od mutatnion / 2 - 2 genes affected for one mutation
                    if (mutationProbability / 2 < r.NextDouble())
                        continue;

                    int pos1 = r.Next(c.GenesCount);
                    int pos2 = r.Next(c.GenesCount);

                    T tmp = c.Genes[pos1];
                    c.Genes[pos1] = c.Genes[pos2];
                    c.Genes[pos2] = tmp;
                }
            }
            
            a.Fitness = fitnessFunction(a);
        }
        public static void ReplaceMutation(Solution<T> a, CreateNewSolution<T> createSolution, FitnessFunction<T> fitnessFunction, double mutationProbability)
        {
            Random r = new Random();
            Solution<T> n = createSolution();
            
            var ae = a.Chromosomes.GetEnumerator();
            var ne = n.Chromosomes.GetEnumerator();

            while (ae.MoveNext() && ne.MoveNext())
            {
                var ac = ae.Current.Genes;
                var nc = ne.Current.Genes;

                for (int i = 0; i < ac.Count; i++)
                {
                    // Probability od mutatnion
                    if (mutationProbability < r.NextDouble())
                        break;

                    ac[i] = nc[i];
                }
            }
            
            a.Fitness = fitnessFunction(a);
        }
        #endregion
    }

    class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
    {
        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            return result == 0 ? -1 : result;
        }
    }
}
