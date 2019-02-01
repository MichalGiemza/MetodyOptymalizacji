using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_1
{
    class Graf
    {
        protected bool[,] macierzKrawedzi;
        protected int iloscWierzcholkow;
        protected int[,] macierzWag = null;
        protected int[] tablicaSkojarzen = null;

        protected const int nieskojarzony = -1;

        public bool[,] MacierzKrawedzi { get { return (bool[,])macierzKrawedzi.Clone(); } }
        public int IloscWierzcholkow { get { return iloscWierzcholkow; } }
        public int[,] MacierzWag { get { return macierzWag != null ? (int[,])macierzWag.Clone() : null; } }
        public int[] TablicaSkojarzen { get { return tablicaSkojarzen != null ? (int[])tablicaSkojarzen.Clone() : null; } }

        public Graf(int iloscWierzcholkow, bool zawieraWagi = false)
        {
            this.iloscWierzcholkow = iloscWierzcholkow;
            macierzKrawedzi = new bool[iloscWierzcholkow, iloscWierzcholkow];
            UsunKrawedzie();

            if (zawieraWagi)
                WyczyscWagi();
        }
        public Graf(Graf g)
        {
            iloscWierzcholkow = g.iloscWierzcholkow;
            macierzKrawedzi = g.MacierzKrawedzi;
            macierzWag = g.MacierzWag;
            tablicaSkojarzen = g.TablicaSkojarzen;
        }

        public void UsunKrawedzie()
        {
            for (int i = 0; i < iloscWierzcholkow; i++)
            {
                for (int j = 0; j < iloscWierzcholkow; j++)
                {
                    macierzKrawedzi[i, j] = false;
                }
            }
        }
        public virtual bool Krawedz(int wierzcholekStartowy, int wierzcholekDocelowy)
        {
            return macierzKrawedzi[wierzcholekStartowy, wierzcholekDocelowy];
        }
        private void UstawKrawedzSkierowana(int wierzcholekStartowy, int wierzcholekDocelowy, bool polacz, int waga = int.MaxValue)
        {
            macierzKrawedzi[wierzcholekStartowy, wierzcholekDocelowy] = polacz;

            if (waga != int.MaxValue)
                macierzWag[wierzcholekStartowy, wierzcholekDocelowy] = waga;
        }
        public void DodajKrawedzSkierowana(int wierzcholekStartowy, int wierzcholekDocelowy, int waga = int.MaxValue)
        {
            UstawKrawedzSkierowana(wierzcholekStartowy, wierzcholekDocelowy, true, waga);
        }
        public void DodajKrawedzNieskierowana(int wierzcholek1, int wierzcholek2, int waga = int.MaxValue)
        {
            UstawKrawedzSkierowana(wierzcholek1, wierzcholek2, true, waga);
            UstawKrawedzSkierowana(wierzcholek2, wierzcholek1, true, waga);
        }
        public void UsunKrawedzSkierowana(int wierzcholekStartowy, int wierzcholekDocelowy)
        {
            UstawKrawedzSkierowana(wierzcholekStartowy, wierzcholekDocelowy, false);
        }
        public void UsunKrawedzNieskierowana(int wierzcholek1, int wierzcholek2)
        {
            UstawKrawedzSkierowana(wierzcholek1, wierzcholek2, false);
            UstawKrawedzSkierowana(wierzcholek2, wierzcholek1, false);
        }

        public List<int> ZnajdzCyklEulera(int wierzcholekStartowy)
        {
            Graf g = new Graf(this);
            Stack<int> stos = new Stack<int>();
            List<int> lista = new List<int>();

            stos.Push(wierzcholekStartowy);

            while (stos.Count > 0)
            {
                int v = stos.First();

                int? s = g.PierwszySasiad(v);
                if (s.HasValue == false)
                {
                    stos.Pop();
                    lista.Add(v);
                }
                else
                {
                    stos.Push(s.Value);
                    g.UsunKrawedzNieskierowana(v, s.Value);
                }
            }
            return lista;
        }

        private bool ZawieraWszystkieWierzcholki(IEnumerable<int> zbior)
        {
            bool[] wierzcholekObecny = new bool[iloscWierzcholkow];

            for (int i = 0; i < iloscWierzcholkow; i++)
                wierzcholekObecny[i] = false;

            foreach (int w in zbior)
                wierzcholekObecny[w] = true;

            bool result = true;
            foreach (bool wo in wierzcholekObecny)
                result &= wo;

            return result;
        }
        private bool ZbiorZawieraWierzcholek(IEnumerable<int> zbior, int wierzcholek)
        {
            foreach (var w in zbior)
            {
                if (wierzcholek == w)
                    return true;
            }
            return false;
        }
        public int? PierwszySasiad(int wierzcholek, int pierwszyOd = 0)
        {
            for (int i = pierwszyOd; i < iloscWierzcholkow; i++)
            {
                if (macierzKrawedzi[wierzcholek, i] == true)
                    return i;
            }
            return null;
        }

        public List<int> Sasiedzi(int wierzcholek)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < iloscWierzcholkow; i++)
            {
                if (macierzKrawedzi[wierzcholek, i] == true)
                    result.Add(i);
            }
            return result;
        }

        public void WyczyscWagi()
        {
            if (macierzWag == null)
                macierzWag = new int[iloscWierzcholkow, iloscWierzcholkow];

            for (int i = 0; i < iloscWierzcholkow; i++)
            {
                for (int j = 0; j < iloscWierzcholkow; j++)
                {
                    macierzWag[i, j] = int.MaxValue;
                }
            }
        }
        public int WagaKrawedzi(int wierzcholekStartowy, int wierzcholekDocelowy)
        {
            return macierzWag[wierzcholekStartowy, wierzcholekDocelowy];
        }
        //public Drzewo<int> MinimalneDrzewoRozpinajace()
        //{
        //    throw new NotImplementedException();
        //}

        public void WyczyscSkojarzenia()
        {
            if (tablicaSkojarzen == null)
                tablicaSkojarzen = new int[iloscWierzcholkow];

            for (int i = 0; i < iloscWierzcholkow; i++)
            {
                tablicaSkojarzen[i] = nieskojarzony;
            }
        }
        protected void UstawSkojarzenie(int wierzcholek1, int wierzcholek2, bool skojarzony = true)
        {
            if (skojarzony == false)
            {
                tablicaSkojarzen[wierzcholek1] = nieskojarzony;
                tablicaSkojarzen[wierzcholek2] = nieskojarzony;
            }
            else
            {
                tablicaSkojarzen[wierzcholek1] = wierzcholek2;
                tablicaSkojarzen[wierzcholek2] = wierzcholek1;
            }

        }
        public int? Skojarzenie(int wierzcholek)
        {
            if (tablicaSkojarzen[wierzcholek] > nieskojarzony)
                return tablicaSkojarzen[wierzcholek];
            else
                return null;
        }
        public bool CzySkojarzone(int wierzcholek1, int wierzcholek2)
        {
            return tablicaSkojarzen[wierzcholek1] == wierzcholek2;
        }
    }
}