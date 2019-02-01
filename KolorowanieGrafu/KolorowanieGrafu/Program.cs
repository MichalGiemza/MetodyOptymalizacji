using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt_1;

namespace KolorowanieGrafu
{
    class Program
    {
        const int niepokolorowany = 0;

        static int[] KolorujGraf(int iloscWierzcholkow, Graf g, IEnumerable<int> kolejnosc)
        {
            int[] pokolorowanie = new int[iloscWierzcholkow];

            int aktualnaIloscKolorow = 0;

            foreach (int w in kolejnosc)
            {
                List<int> koloryDopuszczalne = ZnajdzKoloryDopuszczalne(aktualnaIloscKolorow, g.Sasiedzi(w), pokolorowanie);
                pokolorowanie[w] = koloryDopuszczalne[0];
                aktualnaIloscKolorow = Math.Max(aktualnaIloscKolorow, koloryDopuszczalne[0]);
            }

            return pokolorowanie;
        }

        static List<int> ZnajdzKoloryDopuszczalne(int aktualnaIloscKolorow, IEnumerable<int> sasiedzi, int[] pokolorowanie)
        {
            List<int> kolory = new List<int>();

            for (int i = 1; i <= aktualnaIloscKolorow + 1; i++)
                kolory.Add(i);

            foreach (var s in sasiedzi)
                kolory.Remove(pokolorowanie[s]);

            return kolory;
        }

        static void Main(string[] args)
        {
            const int iloscWierzcholkow = 6;
            Graf g = new Graf(iloscWierzcholkow);

            //g.DodajKrawedzNieskierowana(0, 1);
            //g.DodajKrawedzNieskierowana(1, 2);
            //g.DodajKrawedzNieskierowana(2, 3);
            //g.DodajKrawedzNieskierowana(1, 4);
            //g.DodajKrawedzNieskierowana(4, 5);
            //g.DodajKrawedzNieskierowana(5, 2);

            g.DodajKrawedzNieskierowana(0, 2);
            g.DodajKrawedzNieskierowana(0, 4);

            g.DodajKrawedzNieskierowana(1, 3);
            g.DodajKrawedzNieskierowana(1, 5);

            g.DodajKrawedzNieskierowana(2, 5);

            g.DodajKrawedzNieskierowana(3, 4);

            List<int> stopien = new List<int>();
            for (int i = 0; i < iloscWierzcholkow; i++)
                stopien.Add(g.Sasiedzi(i).Count);

            Queue<int> kolejnosc = new Queue<int>();
            while (true)
            {
                int maxIndex = stopien.IndexOf(stopien.Max());

                if (stopien[maxIndex] < 0)
                    break;

                kolejnosc.Enqueue(maxIndex);
                stopien[maxIndex] = -1;
            }

            var pokolorowanie = KolorujGraf(iloscWierzcholkow, g, kolejnosc);

            Console.ReadLine();
        }
    }
}
