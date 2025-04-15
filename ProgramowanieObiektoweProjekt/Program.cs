namespace ProgramowanieObiektoweProjekt;
using Spectre.Console;
using System;

    static void Main(string[] args);
    
       class Program
    {
        static int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
        static char[,] plansza1 = new char[10, 10];
        static char[,] plansza2 = new char[10, 10];
        static int[] statki = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        static int aktualnyStatek = 0;
        static bool poziomy = true;
        static bool gracz1 = true;
        static bool koniecUmieszczania = false;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            InicjalizujPlansze();

            while (!koniecUmieszczania)
            {
                WyswietlInterfejs();
                ObsluzWejscie();
            }

            Console.Clear();
            WyswietlObiePlansze();
            Console.WriteLine("\nKoniec umieszczania statków. Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        static void InicjalizujPlansze()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    plansza1[i, j] = '·';
                    plansza2[i, j] = '·';
                }
        }

        static void WyswietlInterfejs()
        {
            Console.Clear();
            Console.WriteLine("===============================================");
            Console.WriteLine("          GRA W STATKI - UMIEŚĆ STATKI         ");
            Console.WriteLine("===============================================");

            if (!koniecUmieszczania)
            {
                Console.WriteLine($"\nGracz {(gracz1 ? "1" : "2")}: Statek {aktualnyStatek + 1}/{statki.Length} ({statki[aktualnyStatek]}-masztowiec)");
                Console.WriteLine("Sterowanie: [Strzałki] ruch | [Spacja] obrót | [Enter] umieść");
                Console.WriteLine("===============================================\n");
            }

            WyswietlAktualnaPlansze();
        }

        static void WyswietlAktualnaPlansze()
        {
            char[,] plansza = gracz1 ? plansza1 : plansza2;
            int x = gracz1 ? x1 : x2;
            int y = gracz1 ? y1 : y2;

            Console.WriteLine($"PLANSZA GRACZA {(gracz1 ? "1" : "2")}\n");

            // Nagłówek kolumn
            Console.Write("     ");
            for (int j = 0; j < 10; j++)
            {
                Console.Write($" {(char)('A' + j)}  ");
            }
            Console.WriteLine();

            // Górna krawędź
            Console.Write("    +");
            for (int j = 0; j < 10; j++)
            {
                Console.Write("---+");
            }
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                // Numer wiersza
                Console.Write($" {i + 1,2} |");

                // Zawartość wiersza
                for (int j = 0; j < 10; j++)
                {
                    if (i == x && j == y)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.Write($" {plansza[i, j]} ");
                        Console.ResetColor();
                        Console.Write("|");
                    }
                    else if (CzyPodgladStatku(i, j))
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write(" ■ ");
                        Console.ResetColor();
                        Console.Write("|");
                    }
                    else
                    {
                        Console.Write($" {plansza[i, j]} |");
                    }
                }
                Console.WriteLine();

                // Linia oddzielająca
                Console.Write("    +");
                for (int j = 0; j < 10; j++)
                {
                    Console.Write("---+");
                }
                Console.WriteLine();
            }
        }

        static void WyswietlObiePlansze()
        {
            Console.WriteLine("===============================================");
            Console.WriteLine("           PODSUMOWANIE UMIESZCZANIA          ");
            Console.WriteLine("===============================================\n");

            Console.WriteLine("PLANSZA GRACZA 1\n");
            WyswietlGotowaPlansze(plansza1);

            Console.WriteLine("\n===============================================\n");

            Console.WriteLine("PLANSZA GRACZA 2\n");
            WyswietlGotowaPlansze(plansza2);
        }

        static void WyswietlGotowaPlansze(char[,] plansza)
        {
            // Nagłówek kolumn
            Console.Write("     ");
            for (int j = 0; j < 10; j++)
            {
                Console.Write($" {(char)('A' + j)}  ");
            }
            Console.WriteLine();

            // Górna krawędź
            Console.Write("    +");
            for (int j = 0; j < 10; j++)
            {
                Console.Write("---+");
            }
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                // Numer wiersza
                Console.Write($" {i + 1,2} |");

                // Zawartość wiersza
                for (int j = 0; j < 10; j++)
                {
                    Console.Write($" {(plansza[i, j] == '·' ? " " : "■")} |");
                }
                Console.WriteLine();

                // Linia oddzielająca
                Console.Write("    +");
                for (int j = 0; j < 10; j++)
                {
                    Console.Write("---+");
                }
                Console.WriteLine();
            }
        }

        static void ObsluzWejscie()
        {
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (gracz1) { if (y1 > 0) y1--; }
                    else { if (y2 > 0) y2--; }
                    break;
                case ConsoleKey.RightArrow:
                    if (gracz1) { if (y1 < 9 - (poziomy ? statki[aktualnyStatek] - 1 : 0)) y1++; }
                    else { if (y2 < 9 - (poziomy ? statki[aktualnyStatek] - 1 : 0)) y2++; }
                    break;
                case ConsoleKey.UpArrow:
                    if (gracz1) { if (x1 > 0) x1--; }
                    else { if (x2 > 0) x2--; }
                    break;
                case ConsoleKey.DownArrow:
                    if (gracz1) { if (x1 < 9 - (poziomy ? 0 : statki[aktualnyStatek] - 1)) x1++; }
                    else { if (x2 < 9 - (poziomy ? 0 : statki[aktualnyStatek] - 1)) x2++; }
                    break;
                case ConsoleKey.Spacebar:
                    if (CzyMoznaObrocic())
                        poziomy = !poziomy;
                    break;
                case ConsoleKey.Enter:
                    if (CzyMoznaUmiescicStatek())
                    {
                        UmiescStatek();
                        aktualnyStatek++;
                        if (aktualnyStatek >= statki.Length)
                        {
                            if (gracz1)
                            {
                                gracz1 = false;
                                aktualnyStatek = 0;
                                x2 = 0; y2 = 0;
                            }
                            else
                            {
                                koniecUmieszczania = true;
                            }
                        }
                    }
                    break;
                case ConsoleKey.Escape:
                    koniecUmieszczania = true;
                    break;
            }
        }

        static bool CzyPodgladStatku(int i, int j)
        {
            if (koniecUmieszczania) return false;

            int x = gracz1 ? x1 : x2;
            int y = gracz1 ? y1 : y2;

            if (poziomy)
                return i == x && j >= y && j < y + statki[aktualnyStatek] && j < 10;
            else
                return j == y && i >= x && i < x + statki[aktualnyStatek] && i < 10;
        }

        static bool CzyMoznaUmiescicStatek()
        {
            char[,] plansza = gracz1 ? plansza1 : plansza2;
            int x = gracz1 ? x1 : x2;
            int y = gracz1 ? y1 : y2;

            if (poziomy)
            {
                if (y + statki[aktualnyStatek] > 10) return false;
                for (int j = y; j < y + statki[aktualnyStatek]; j++)
                    if (plansza[x, j] != '·' || !CzyWolne(plansza, x, j)) return false;
            }
            else
            {
                if (x + statki[aktualnyStatek] > 10) return false;
                for (int i = x; i < x + statki[aktualnyStatek]; i++)
                    if (plansza[i, y] != '·' || !CzyWolne(plansza, i, y)) return false;
            }
            return true;
        }

        static bool CzyWolne(char[,] plansza, int i, int j)
        {
            for (int di = -1; di <= 1; di++)
                for (int dj = -1; dj <= 1; dj++)
                {
                    int ni = i + di;
                    int nj = j + dj;
                    if (ni >= 0 && ni < 10 && nj >= 0 && nj < 10 && plansza[ni, nj] != '·')
                        return false;
                }
            return true;
        }

        static bool CzyMoznaObrocic()
        {
            char[,] plansza = gracz1 ? plansza1 : plansza2;
            int x = gracz1 ? x1 : x2;
            int y = gracz1 ? y1 : y2;

            if (poziomy)
            {
                if (x + statki[aktualnyStatek] > 10) return false;
                for (int i = x; i < x + statki[aktualnyStatek]; i++)
                    if (plansza[i, y] != '·' || !CzyWolne(plansza, i, y)) return false;
            }
            else
            {
                if (y + statki[aktualnyStatek] > 10) return false;
                for (int j = y; j < y + statki[aktualnyStatek]; j++)
                    if (plansza[x, j] != '·' || !CzyWolne(plansza, x, j)) return false;
            }
            return true;
        }

        static void UmiescStatek()
        {
            char[,] plansza = gracz1 ? plansza1 : plansza2;
            int x = gracz1 ? x1 : x2;
            int y = gracz1 ? y1 : y2;

            if (poziomy)
                for (int j = y; j < y + statki[aktualnyStatek]; j++)
                    plansza[x, j] = '■';
            else
                for (int i = x; i < x + statki[aktualnyStatek]; i++)
                    plansza[i, y] = '■';
        }
    }


