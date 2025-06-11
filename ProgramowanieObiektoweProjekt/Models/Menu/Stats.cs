
namespace ProgramowanieObiektoweProjekt.Models.Menu
{
    internal class Stats
    {
        private static readonly string HistoryFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "Models", "Menu", "History.txt");
        public void saveStats(string Result, int ShotsFired, int Hits, int Misses)
        {
            File.AppendAllLines(HistoryFilePath, new[] {$"-{Result}\n" +
                                                $" Oddanych strzałów: {ShotsFired}, " +
                                                $" Trafień: {Hits}, " +
                                                $" Spudłowań: {Misses}\n" });
        }
        public void openStats()
        {
            if (File.Exists(HistoryFilePath))
            {
                string zawartosc = File.ReadAllText(HistoryFilePath);
                Console.WriteLine(zawartosc);
            }
            else
            {
                Console.WriteLine("Brak historii.");
            }
        }
    }
}
