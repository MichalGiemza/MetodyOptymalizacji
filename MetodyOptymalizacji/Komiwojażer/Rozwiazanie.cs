using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komiwojażer
{
    class Rozwiazanie
    {
        int[,] m;
        int iloscWierzcholkow;
        int lb;
        Rozwiazanie rodzic;
        int[] polaczenie;
        List<int> droga;

        public int Lb { get { return lb; } }
        public List<int> Droga { get { return droga == null ? new List<int>() : droga; } }

        public Rozwiazanie(int[,] macierz, int lb = 0, Rozwiazanie rodzic = null, int[] polaczenie = null, List<int> droga = null)
        {
            m = (int[,])macierz.Clone();
            iloscWierzcholkow = (int)Math.Sqrt(m.Length); // Poprawić
            this.lb = lb;
            this.rodzic = rodzic;
            this.polaczenie = polaczenie;
            this.droga = droga;

            OdejmijMinima();
        }

        private void OdejmijMinima()
        {
            for (int i = 0; i < iloscWierzcholkow; i++)
            {
                // Szukanie indeksu el. najniższego w wierszu
                int min = int.MaxValue;
                for (int j = 0; j < iloscWierzcholkow; j++)
                    min = Math.Min(min, m[i, j]);

                // Odjęcie wartości minimalnej od wszystkich elementów w wierszu
                if (min == int.MaxValue)
                    continue;
                for (int j = 0; j < iloscWierzcholkow; j++)
                {
                    if (m[i, j] == int.MaxValue)
                        continue;

                    m[i, j] -= min;
                }

                lb += min;
            }

            for (int j = 0; j < iloscWierzcholkow; j++)
            {
                // Szukanie indeksu el. najniższego w kolumnie
                int min = int.MaxValue;
                for (int i = 0; i < iloscWierzcholkow; i++)
                    min = Math.Min(min, m[i, j]);

                // Odjęcie wartości minimalnej od wszystkich elementów w kolumnie
                if (min == int.MaxValue)
                    continue;
                for (int i = 0; i < iloscWierzcholkow; i++)
                {
                    if (m[i, j] == int.MaxValue)
                        continue;

                    m[i, j] -= min;
                }

                lb += min;
            }
        }

        public int[] WybierzNajlepszeRozwiazanie()
        {
            // Wybor rozwiazania najmniej podwyzszajacego LB
            int[] pozycja = new int[2];
            int najw_min = 0;
            bool pozycjaIstnieje = false;
            for (int i = 0; i < iloscWierzcholkow; i++)
            {
                for (int j = 0; j < iloscWierzcholkow; j++)
                {
                    // Tylko 0 (połączenie o minimalnym koszcie może zostać wybrane)
                    if (m[i, j] != 0)
                        continue;
                    pozycjaIstnieje = true;

                    // Wybranie połączenia i,j spowoduje usuniecie wiersza i oraz kolumny j,
                    // nalezy wybrac polaczenie kasujące największe minimum w kolumnie i wierszu
                    int lokal_k = int.MaxValue;
                    int lokal_w = int.MaxValue;
                    for (int ir = 0; ir < iloscWierzcholkow; ir++)
                    {
                        if (ir != i)
                            lokal_k = Math.Min(m[ir, j], lokal_k);
                    }
                    for (int jr = 0; jr < iloscWierzcholkow; jr++)
                    {
                        if (jr != j)
                            lokal_w = Math.Min(m[i, jr], lokal_w);
                    }

                    // Najwieksza wartość podnosząca lb z wiersza lub kolumny
                    int lokal_min = Math.Max(lokal_k, lokal_w);
                    if (lokal_min > najw_min)
                    {
                        najw_min = lokal_min;
                        pozycja[0] = i;
                        pozycja[1] = j;
                    }
                }
            }
            if (pozycjaIstnieje)
                return pozycja;
            else
                return null;
        }

        public Rozwiazanie StworzNajlepszeRozwiazanie(int[] najlepsze)
        {
            int[,] rm = (int[,])m.Clone();

            // Kasowanie wiersza
            for (int j = 0; j < iloscWierzcholkow; j++)
                rm[najlepsze[0], j] = int.MaxValue;

            // Kasowanie kolumny
            for (int i = 0; i < iloscWierzcholkow; i++)
                rm[i, najlepsze[1]] = int.MaxValue;

            //// Zablokowanie odwrotnego połączenia 
            //rm[najlepsze[1], najlepsze[0]] = int.MaxValue; // sprawdzić czy zbędne

            // Zablokowanie połączeń tworzących cykle
            List<int> d;
            ZablokujPolaczeniaTworzaceCykle(najlepsze, rm, out d);

            return new Rozwiazanie(rm, lb, this, najlepsze, d);
        }

        protected void ZablokujPolaczeniaTworzaceCykle(int[] pol, int[,] mac, out List<int> droga)
        {
            // Znajdz drogę zawierającą polaczenie 'pol'
            droga = DrogaRozwiazania(pol);

            // Zablokować połączenia z każdego do każdego
            foreach (var s in droga)
            {
                foreach (var c in droga)
                {
                    mac[s, c] = int.MaxValue;
                    mac[c, s] = int.MaxValue;
                }
            }
        }

        protected List<int> DrogaRozwiazania(int[] pol)
        {
            List<int> l;
                
            l = new List<int>();
            l.Add(pol[0]);
            l.Add(pol[1]);

            int tyl = pol[0];
            int przod = pol[1];

            List<int[]> polaczenia = ListaPolaczenRozwiazania();
            List<int[]> pozostale;

            while (polaczenia.Count > 0)
            {
                pozostale = new List<int[]>();
                bool postep = false;

                foreach (var p in polaczenia)
                {
                    // Przedłużenie do tyłu
                    if (tyl == p[1])
                    {
                        l.Insert(0, p[0]);
                        tyl = p[0];
                        postep = true;
                    }
                    // Przedłużenie z przodu
                    else if (przod == p[0])
                    {
                        l.Add(p[1]);
                        przod = p[1];
                        postep = true;
                    }
                    // Zapisanie nieużytego polaczenia do nastepnej iteracji
                    else
                    {
                        pozostale.Add(p);
                    }
                }

                // Uzyj nieuzytych polaczen w nastepnej iteracji
                polaczenia = pozostale;

                // Jesli droga sie nie zmienila przez cala iteracje, wtedy pozostale polaczenia nie naleza do tej drogi
                if (postep == false)
                    break;
            }

            return l;
        }
        protected List<int[]> ListaPolaczenRozwiazania()
        {
            List<int[]> pol = new List<int[]>();
            ListaPolaczenPom(pol);

            return pol;
        }
        protected void ListaPolaczenPom(List<int[]> pol)
        {
            if (polaczenie != null)
                pol.Add(polaczenie);

            if (rodzic != null)
                rodzic.ListaPolaczenPom(pol);
        }

        public Rozwiazanie StworzPozostaleRozwiazania(int[] najlepsze)
        {
            int[,] rm = (int[,])m.Clone();

            // Nieskonczonosc do najlepszego rozwiazania
            rm[najlepsze[0], najlepsze[1]] = int.MaxValue;

            return new Rozwiazanie(rm, lb, this, null, droga == null ? null : new List<int>(droga));
        }

        public void WypiszMacierz()
        {
            for (int i = 0; i < iloscWierzcholkow; i++)
            {
                for (int j = 0; j < iloscWierzcholkow; j++)
                {
                    if (m[i,j] != int.MaxValue)
                        Console.Write(m[i, j] + "\t");
                    else
                        Console.Write("-\t");
                }
                Console.WriteLine();
            }
        }
    }
}
