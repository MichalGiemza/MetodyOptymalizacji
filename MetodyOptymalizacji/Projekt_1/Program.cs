using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_1
{
    class Program
    {
        static void Main(string[] args)
        {
            MaksymalneSkojarzenie();

            Console.ReadLine();
        }

        static void MaksymalneSkojarzenie()
        {
            //GrafNDzielny g = new GrafNDzielny(new Graf(6));

            //g.DodajKrawedzNieskierowana(0, 3);
            //g.DodajKrawedzNieskierowana(0, 4);
            //g.DodajKrawedzNieskierowana(1, 3);
            //g.DodajKrawedzNieskierowana(1, 4);
            //g.DodajKrawedzNieskierowana(1, 5);
            //g.DodajKrawedzNieskierowana(2, 3);

            //g.UmiescWZbiorze(3, 1);
            //g.UmiescWZbiorze(4, 1);
            //g.UmiescWZbiorze(5, 1);


            GrafNDzielny g = new GrafNDzielny(new Graf(10));

            g.DodajKrawedzNieskierowana(0, 5);
            g.DodajKrawedzNieskierowana(0, 6);

            g.DodajKrawedzNieskierowana(1, 5);
            g.DodajKrawedzNieskierowana(1, 6);
            g.DodajKrawedzNieskierowana(1, 7);

            g.DodajKrawedzNieskierowana(2, 6);
            g.DodajKrawedzNieskierowana(2, 7);
            g.DodajKrawedzNieskierowana(2, 8);

            g.DodajKrawedzNieskierowana(3, 7);

            g.DodajKrawedzNieskierowana(4, 5);
            g.DodajKrawedzNieskierowana(4, 7);


            g.DodajKrawedzNieskierowana(3, 9);

            g.UmiescWZbiorze(5, 1);
            g.UmiescWZbiorze(6, 1);
            g.UmiescWZbiorze(7, 1);
            g.UmiescWZbiorze(8, 1);
            g.UmiescWZbiorze(9, 1);


            g.SkojarzeniePoczatkowe(0, 1);
            Console.WriteLine("Skojarzenie poczatkowe:");
            WypiszSkojarzenia(g.TablicaSkojarzen);
            Console.WriteLine();

            g.SkojarzenieMaksymalne(0, 1);
            Console.WriteLine("Skojarzenie maksymalne:");
            WypiszSkojarzenia(g.TablicaSkojarzen);
        }

        static void WypiszSkojarzenia(int[] tabSKojarzen)
        {
            for (int i = 0; i < tabSKojarzen.Length; i++)
            {
                if (tabSKojarzen[i] >= 0)
                    Console.Write("(" + i + ',' + tabSKojarzen[i] + ") ");
            }
            Console.WriteLine();
        }
    }
}
