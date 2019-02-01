using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_1
{
    class Drzewo<T>
    {
        public Wierzcholek<T> korzen = null;
        
        public void DodajKorzen(T wartosc)
        {
            korzen = new Wierzcholek<T>(wartosc);
        }

        public List<T> ToList()
        {
            List<T> l = new List<T>();

            if (korzen != null)
                DodajElementy(l, korzen);

            return l;
        }

        private void DodajElementy(List<T> l, Wierzcholek<T> w)
        {
            l.Add(w.Wartosc);

            foreach (var k in w.Dzieci)
                DodajElementy(l, k);
        }

        public class Wierzcholek<T>
        {
            private T wartosc;
            private Wierzcholek<T> rodzic;
            private List<Wierzcholek<T>> dzieci;

            public T Wartosc { get { return wartosc; } }
            public Wierzcholek<T> Rodzic { get { return rodzic; } }
            public List<Wierzcholek<T>> Dzieci { get { return dzieci; } }
            
            public Wierzcholek(T wartosc, Wierzcholek<T> rodzic = null)
            {
                this.wartosc = wartosc;
                this.rodzic = rodzic;
                dzieci = new List<Wierzcholek<T>>();
            }

            public void DodajDziecko(T wartosc)
            {
                dzieci.Add(new Wierzcholek<T>(wartosc, this));
            }

            public bool SciezkaZawieraElement(T element)
            {
                if (wartosc.Equals(element))
                    return true;

                if (rodzic == null)
                    return false;

                return rodzic.SciezkaZawieraElement(element);
            }

            public bool SciezkaZawieraWierzcholek(Drzewo<T>.Wierzcholek<T> w)
            {
                if (w.Equals(this))
                    return true;

                if (rodzic == null)
                    return false;

                return rodzic.SciezkaZawieraWierzcholek(w);

            }

            public List<T> SciezkaDoKorzenia()
            {
                return SciezkaDoKorzeniaPom(new List<T>());
            }

            private List<T> SciezkaDoKorzeniaPom(List<T> sciezka)
            {
                sciezka.Add(wartosc);

                if (rodzic == null)
                    return sciezka;
                else
                    return rodzic.SciezkaDoKorzeniaPom(sciezka);
            }
        }
    }
}
