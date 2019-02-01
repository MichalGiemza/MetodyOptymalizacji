using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_1
{
    class GrafNDzielny : Graf
    {
        private int[] zbioryWierzcholkow;

        public int[] ZbioryWierzcholkow { get { return (int[])zbioryWierzcholkow.Clone(); } }

        public GrafNDzielny(Graf g) : base(g)
        {
            zbioryWierzcholkow = new int[IloscWierzcholkow];
        }

        public GrafNDzielny(GrafNDzielny g) : base(g)
        {
            zbioryWierzcholkow = g.ZbioryWierzcholkow;
        }

        public void UmiescWZbiorze(int wierzcholek, int zbior)
        {
            zbioryWierzcholkow[wierzcholek] = zbior;
        }
        public int ZbiorWierzcholka(int wierzcholek)
        {
            return zbioryWierzcholkow[wierzcholek];
        }
        public bool NalezyDoZbioru(int wierzcholek, int zbior)
        {
            return ZbiorWierzcholka(wierzcholek) == zbior;
        }
        public List<int> WierzcholkiZbioru(int zbior)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < IloscWierzcholkow; i++)
            {
                if (NalezyDoZbioru(i, zbior))
                    result.Add(i);
            }
            return result;
        }

        public void SkojarzeniePoczatkowe(int zbior1, int zbior2)
        {
            WyczyscSkojarzenia();
            
            Queue<int> wierzcholki = new Queue<int>();
            
            foreach (int w in WierzcholkiZbioru(zbior1))
                wierzcholki.Enqueue(w);
            
            while (wierzcholki.Count > 0)
            {
                int wierzcholek = wierzcholki.Dequeue();

                if (tablicaSkojarzen[wierzcholek] != nieskojarzony)
                    continue;
                
                foreach (int sasiad in Sasiedzi(wierzcholek))
                {
                    if (tablicaSkojarzen[sasiad] == nieskojarzony && NalezyDoZbioru(sasiad, zbior2))
                    {
                        UstawSkojarzenie(wierzcholek, sasiad);
                        break;
                    }
                }
            }
        }
        public void SkojarzenieMaksymalne(int zbior1, int zbior2)
        {
            if (tablicaSkojarzen == null)
                SkojarzeniePoczatkowe(zbior1, zbior2);

            do
            {
                var sciezka = ZnajdzDrogeRozszerzajaca(zbior1, zbior2);

                if (sciezka == null)
                    break;
                
                GrafNDzielny g = new GrafNDzielny(this);
                g.UsunKrawedzie();
                
                bool[,] sk = new bool[iloscWierzcholkow, iloscWierzcholkow];
                bool[,] sc = new bool[iloscWierzcholkow, iloscWierzcholkow];
                
                for (int i = 1; i < sciezka.Count; i++)
                {
                    sc[sciezka[i], sciezka[i - 1]] = true;
                    sc[sciezka[i - 1], sciezka[i]] = true;
                }
                for (int i = 0; i < tablicaSkojarzen.Length; i++)
                {
                    if (tablicaSkojarzen[i] != nieskojarzony)
                    {
                        sk[i, tablicaSkojarzen[i]] = true;
                        sk[tablicaSkojarzen[i], i] = true;
                    }
                }

                for (int i = 0; i < iloscWierzcholkow; i++)
                {
                    for (int j = 0; j < iloscWierzcholkow; j++)
                    {
                        if (sc[i, j] ^ sk[i, j])
                            g.DodajKrawedzSkierowana(i, j);
                    }
                }

                g.SkojarzeniePoczatkowe(zbior1, zbior2);

                tablicaSkojarzen = g.TablicaSkojarzen;
                
            } while (true);
        }

        public List<int> ZnajdzDrogeRozszerzajaca(int zbior1, int zbior2)
        {
            Drzewo<int> d;
            GrafNDzielny g = new GrafNDzielny(this);

            //g.UsunKrawedzieWychodzaceZeZbioru(zbior2);

            Drzewo<int>.Wierzcholek<int> w = null;
            foreach (int v1 in WierzcholkiZbioru(zbior1))
            {
                if (g.Skojarzenie(v1).HasValue)
                    continue;

                d = new Drzewo<int>();
                d.DodajKorzen(v1);

                w = g.SzukajNieskojarzonego(d.korzen, zbior2);

                if (w != null)
                    break;
            }
            if (w != null)
                return w.SciezkaDoKorzenia();
            else
                return null;
        }

        private Drzewo<int>.Wierzcholek<int> SzukajNieskojarzonego(Drzewo<int>.Wierzcholek<int> wierzcholekStartowy, int wZbiorze)
        {
            Drzewo<int>.Wierzcholek<int> result = null;

            foreach (int w in Sasiedzi(wierzcholekStartowy.Wartosc))
                if (wierzcholekStartowy.SciezkaZawieraElement(w) == false)
                    wierzcholekStartowy.DodajDziecko(w);

            foreach (var d in wierzcholekStartowy.Dzieci)
            {
                if (Skojarzenie(d.Wartosc).HasValue == false && NalezyDoZbioru(d.Wartosc, wZbiorze))
                {
                    result = d;
                    break;
                }
                else
                {
                    // Szukanie dalej po scieżce naprzemiennej skojarzony-nieskojarzony

                    // Sprawdzanie czy ścieżka jest naprzemienna na podstawie 3 węzłów: wierzcholekStartowy.rodzic, wierzcholekStartowy oraz d
                    // Jeśli rodzic == null wtedy wszystkie d dozwolone
                    if (wierzcholekStartowy.Rodzic == null)
                    {
                        result = SzukajNieskojarzonego(d, wZbiorze);
                        if (result != null)
                            break;
                    }
                    else
                    {
                        // Każde d dozwolone jeżeli: startowy skojarzony z: rodzicem lub d
                        if (CzySkojarzone(wierzcholekStartowy.Wartosc, wierzcholekStartowy.Rodzic.Wartosc) || CzySkojarzone(wierzcholekStartowy.Wartosc, d.Wartosc))
                            result = SzukajNieskojarzonego(d, wZbiorze);
                        if (result != null)
                            break;
                    }
                }
            }
            return result;
        }

        public void UsunKrawedzieWychodzaceZeZbioru(int zbior)
        {
            foreach (int w in WierzcholkiZbioru(zbior))
            {
                for (int i = 0; i < IloscWierzcholkow; i++)
                    UsunKrawedzSkierowana(w, i);
            }
        }
    }
}
