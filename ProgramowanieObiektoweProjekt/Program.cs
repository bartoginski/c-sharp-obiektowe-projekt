namespace ProgramowanieObiektoweProjekt;
using ProgramowanieObiektoweProjekt.Models.Boards;

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
        var board = new Board();

        board.DisplayBoard(false);
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




