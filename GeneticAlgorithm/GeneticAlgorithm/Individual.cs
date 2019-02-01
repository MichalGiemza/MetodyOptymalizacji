using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    public class Solution<T>
    {
        List<Chromosome<T>> chromosomes;
        IComparable fitness;

        public List<Chromosome<T>> Chromosomes { get { return chromosomes; } }
        public int ChromosomesCount { get { return chromosomes.Count; } }
        public IComparable Fitness { get { return fitness; } set { fitness = value; } }

        public Solution(List<Chromosome<T>> chromosomes, IComparable fitness)
        {
            this.chromosomes = chromosomes;
            this.fitness = fitness;
        }

        public Solution(List<Chromosome<T>> chromosomes)
        {
            this.chromosomes = chromosomes;
            fitness = null;
        }

        public int CompareTo(object obj)
        {
            return fitness.CompareTo(((Solution<T>)obj).Fitness);
        }

        public Solution<T> Clone()
        {
            List<Chromosome<T>> chromosomes = new List<Chromosome<T>>(this.chromosomes); // ref czy kopia?

            return new Solution<T>(chromosomes, Fitness);
        }
    }
}
