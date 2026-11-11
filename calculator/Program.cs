using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PÉNZÜGYI KALKULÁTOR ===");
            Console.WriteLine("Adja meg az alábbi adatokat!\n");

            //bemenetek
            double kezdotoke = beKezdo("Kezdőtőke (HUF): ");
            double rendszeresBefizetes = beKezdo("Rendszeres befizetés (HUF): ");

            bool haviBefizetes = berGyakorisag("Befizetés gyakorisága (H - havi, E - éves): ");
            double indexalas = beSzazalek("Befizetés éves indexálása (%): ");
            int futamidoEv = beFutamido("Futamidő (Év): ");
            double evesHozam = beSzazalek("Várható éves hozam (%): ");

            bool haviTokesites = berGyakorisag("Tőkésítés gyakorisága (H - havi, E - éves): ");
            double inflacio = beSzazalek("Várható átlagos infláció (%): ");
            double nyeresegado = beSzazalek("Nyereségadó (%): ");

            double aktualisEgyenleg = kezdotoke;
            double osszesBefizetettToke = kezdotoke;

            int tokesitesekEvente = haviTokesites ? 12 : 1;
            double idoszakiHozamRata = evesHozam / tokesitesekEvente;

            bool haviSzimulacio = haviBefizetes || haviTokesites;
            int teljesCiklusSzam = haviSzimulacio ? (futamidoEv * 12) : futamidoEv;

            List<double> evesTokek = new List<double>();
            List<double> evesEgyenlegek = new List<double>();

        

            for (int aktualisCiklus = 1; aktualisCiklus <= teljesCiklusSzam; aktualisCiklus++)
            {
                int aktualisEv = haviSzimulacio ? ((aktualisCiklus - 1) / 12) + 1 : aktualisCiklus;
                int honapAzEvenBelul = haviSzimulacio ? ((aktualisCiklus - 1) % 12) + 1 : 1;

                double aktualisRendszeresBefizetes = rendszeresBefizetes;
                if (aktualisEv > 1 && indexalas > 0)
                {
                    aktualisRendszeresBefizetes = rendszeresBefizetes * Math.Pow(1 + indexalas, aktualisEv - 1);
                }

                if (haviBefizetes && haviSzimulacio)
                {
                    aktualisEgyenleg += aktualisRendszeresBefizetes;
                    osszesBefizetettToke += aktualisRendszeresBefizetes;
                    
                }
                else if (!haviBefizetes && (!haviSzimulacio || honapAzEvenBelul == 1))
                {
                    aktualisEgyenleg += aktualisRendszeresBefizetes;
                    osszesBefizetettToke += aktualisRendszeresBefizetes;
                    
                }

                if (haviTokesites && haviSzimulacio)
                {
                    aktualisEgyenleg *= (1 + idoszakiHozamRata);
                    
                }
                else if (!haviTokesites)
                {
                    if (!haviSzimulacio || honapAzEvenBelul == 12)
                    {
                        aktualisEgyenleg *= (1 + evesHozam);
                        
                    }
                }

                if (!haviSzimulacio || honapAzEvenBelul == 12)
                {
                    evesTokek.Add(osszesBefizetettToke);
                    evesEgyenlegek.Add(aktualisEgyenleg);
                }
            }

            double bruttoZaroEgyenleg = aktualisEgyenleg;
            double elertBruttoHozam = Math.Max(0, bruttoZaroEgyenleg - osszesBefizetettToke);

            double levonandoAdo = elertBruttoHozam * nyeresegado;
            double nettoZaroEgyenleg = bruttoZaroEgyenleg - levonandoAdo;

            double realErtek = nettoZaroEgyenleg / Math.Pow(1 + inflacio, futamidoEv);

            //kiiras
            Console.WriteLine("\n=====================================");
            Console.WriteLine("         SZÁMÍTÁSI EREDMÉNYEK        ");
            Console.WriteLine("=====================================");
            Console.WriteLine($"Befizetett tőke összesen:  {osszesBefizetettToke:N0}");
            Console.WriteLine($"Elért bruttó hozam:        {elertBruttoHozam:N0}");
            Console.WriteLine($"Bruttó záróegyenleg:       {bruttoZaroEgyenleg:N0}");
            Console.WriteLine($"Nettó záróegyenleg:        {nettoZaroEgyenleg:N0}");
            Console.WriteLine($"Reálérték (mai értéken):   {realErtek:N0}");
            Console.WriteLine("=====================================");

            Console.WriteLine("\n--- ÉVENKÉNTI BONTÁS ---");
            for (int i = 0; i < evesTokek.Count; i++)
            {
                Console.WriteLine($"{i + 1,2}. év | Tőke: {evesTokek[i],12:N0} Ft | Egyenleg: {evesEgyenlegek[i],12:N0} Ft");
            }

            Console.Write("\nSzeretné elmenteni a kalkuláció részleteit egy fájlba? (i/n): ");
            string valasz = Console.ReadLine().Trim().ToLower();

            if (valasz == "i")
            {
                using (StreamWriter sw = new StreamWriter("kalkulacio_eredmeny.txt", false, Encoding.UTF8))
                {
                    sw.WriteLine("=== MEGADOTT PARAMÉTEREK ===");
                    sw.WriteLine($"Kezdőtőke: {kezdotoke:N0} Ft");
                    sw.WriteLine($"Rendszeres befizetés: {rendszeresBefizetes:N0} Ft");
                    sw.WriteLine($"Befizetés gyakorisága: {(haviBefizetes ? "Havi" : "Éves")}");
                    sw.WriteLine($"Befizetés indexálása: {indexalas * 100}%");
                    sw.WriteLine($"Futamidő: {futamidoEv} év");
                    sw.WriteLine($"Várható éves hozam: {evesHozam * 100}%");
                    sw.WriteLine($"Tőkésítés gyakorisága: {(haviTokesites ? "Havi" : "Éves")}");
                    sw.WriteLine($"Várható infláció: {inflacio * 100}%");
                    sw.WriteLine($"Nyereségadó: {nyeresegado * 100}%\n");

                    sw.WriteLine("=== ÖSSZESÍTETT VÉGEREDMÉNYEK ===");
                    sw.WriteLine($"Befizetett tőke összesen:  {osszesBefizetettToke:N0} Ft");
                    sw.WriteLine($"Elért bruttó hozam:        {elertBruttoHozam:N0} Ft");
                    sw.WriteLine($"Bruttó záróegyenleg:       {bruttoZaroEgyenleg:N0} Ft");
                    sw.WriteLine($"Nettó záróegyenleg:        {nettoZaroEgyenleg:N0} Ft");
                    sw.WriteLine($"Reálérték (mai értéken):   {realErtek:N0} Ft\n");

                    sw.WriteLine("=== ÉVENKÉNTI BONTÁS ===");
                    for (int i = 0; i < evesTokek.Count; i++)
                    {
                        sw.WriteLine($"{i + 1,2}. év | Tőke: {evesTokek[i],12:N0} Ft | Egyenleg: {evesEgyenlegek[i],12:N0} Ft");
                    }
                }
                Console.WriteLine("Sikeresen mentve a kalkulacio_eredmeny.txt fájlba!");
            }

            Console.WriteLine("\nprogram vége");
            Console.ReadKey();
        }

        static double beKezdo(string szoveg)
        {
            double ertek;
            while (true)
            {
                Console.Write(szoveg);
                string bemenet = Console.ReadLine();
                if (double.TryParse(bemenet, out ertek) && ertek >= 0)
                {
                    return ertek;
                }
                Console.WriteLine("HIBA érvényes, nemnegatív számot adjon meg!");
            }
        }

        static int beFutamido(string szoveg)
        {
            int ertek;
            while (true)
            {
                Console.Write(szoveg);
                string bemenet = Console.ReadLine();
                if (int.TryParse(bemenet, out ertek) && ertek >= 0)
                {
                    return ertek;
                }
                Console.WriteLine("HIBA érvényes, nemnegatív egész számot adjon meg!");
            }
        }

        static double beSzazalek(string szoveg)
        {
            double ertek;
            while (true)
            {
                Console.Write(szoveg);
                string bemenet = Console.ReadLine();
                if (double.TryParse(bemenet, out ertek))
                {
                    return ertek / 100.0;
                }
                Console.WriteLine("HIBA érvényes számot adjon meg százalékos formában!");
            }
        }

        static bool berGyakorisag(string szoveg)
        {
            while (true)
            {
                Console.Write(szoveg);
                string bemenet = Console.ReadLine().Trim().ToUpper();
                if (bemenet == "H") return true;
                if (bemenet == "E") return false;
                Console.WriteLine("HIBA csak 'H' (havi) vagy 'E' (éves) karaktert adjon meg!");
            }
        }
    }
}
