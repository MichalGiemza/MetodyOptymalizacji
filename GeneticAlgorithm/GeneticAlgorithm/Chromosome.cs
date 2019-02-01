using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    public class Chromosome<T>
    {
        List<T> genes;

        public int GenesCount { get { return genes.Count; } }
        public List<T> Genes { get { return genes; } }

        public Chromosome(List<T> genes)
        {
            this.genes = genes;
        }

        public Chromosome<T> Clone()
        {
            List<T> genes = new List<T>(this.genes); // Czy referencja czy kopia?
            
            return new Chromosome<T>(genes);
        }
    }
}
