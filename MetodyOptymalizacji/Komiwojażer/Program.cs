using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komiwojażer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Metoda podziału i ograniczeń

            int[,] m = new int[,]
            {
                { int.MaxValue, 3, 93, 13, 1, 9},
                { 4, int.MaxValue, 77, 42, 21, 16},
                { 45, 17, int.MaxValue, 36, 16, 28},
                { 39, 90, 80, int.MaxValue, 56, 7},
                { 28, 46, 1, 33, int.MaxValue, 25},
                { 3, 88, 18, 46, 92, int.MaxValue}
            };
            int iloscWierzcholkow = (int)Math.Sqrt(m.Length);
            
            SortedList<int, Rozwiazanie> rozwiazywane = new SortedList<int, Rozwiazanie>(new KomparatorZDuplikatami<int>());
            SortedList<int, Rozwiazanie> ukonczone = new SortedList<int, Rozwiazanie>(new KomparatorZDuplikatami<int>());

            rozwiazywane.Add(0, new Rozwiazanie(m));

            // Warunek: Jeśli rozwiazywane zawieraja elementy;
            while (rozwiazywane.Count > 0)
            {
                //Warunek Stopu: w 'rozwiazywane' nie ma już przypadku mogącego dać lepsze roziązanie niż najlepsze 'ukonczone'
                if (ukonczone.Count > 0 && rozwiazywane.Values[0].Lb <= ukonczone.Values[0].Lb)
                    break;

                // Weź rozwiązanie z najniższym lb
                Rozwiazanie r = rozwiazywane.ElementAt(0).Value;
                rozwiazywane.RemoveAt(0);
                
                int[] pozycja = r.WybierzNajlepszeRozwiazanie();
                
                if (pozycja != null)
                {
                    Console.Write("Wybrane polaczenie: (" + pozycja[0] + "," + pozycja[1] + ")  Droga: ");
                    foreach (var w in r.Droga)
                        Console.Write(w + " ");
                    Console.WriteLine();
                    r.WypiszMacierz();
                    Console.WriteLine();

                    // Stworzenie dwoch kolejnych rozwiazan: 1 - najlepsze, 2 - pozostale
                    Rozwiazanie najlepsze = r.StworzNajlepszeRozwiazanie(pozycja);
                    rozwiazywane.Add(najlepsze.Lb, najlepsze);

                    Rozwiazanie pozostale = r.StworzPozostaleRozwiazania(pozycja);
                    rozwiazywane.Add(pozostale.Lb, pozostale);
                }
                else
                {
                    if (r.Droga.Count == iloscWierzcholkow)
                        ukonczone.Add(r.Lb ,r);
                }
            }

            // Wynik: ukonczone(0)
            var wynik = ukonczone.Values[0];
            var lb = ukonczone.Keys[0];

            // Koszt przejscia przez graf
            int koszt = 0;
            for (int i = 1; i < iloscWierzcholkow; i++)
            {
                koszt += m[wynik.Droga[i - 1], wynik.Droga[i]];
            }
            koszt += m[wynik.Droga[iloscWierzcholkow - 1], wynik.Droga[0]];

            Console.WriteLine("\n\n Wynik:");
            Console.WriteLine("  LB: " + lb);
            Console.Write("  Droga: ");
            foreach (var w in wynik.Droga)
                Console.Write(w + " ");
            Console.WriteLine("\n  Koszt: " + koszt);
            
            Console.ReadLine();
        }

    }

    public class KomparatorZDuplikatami<TKey> : IComparer<TKey> where TKey : IComparable
    {
        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                // Równość jest zwracana jako mniejszosc
                return 1;
            else
                return result;
        }
    }
}
