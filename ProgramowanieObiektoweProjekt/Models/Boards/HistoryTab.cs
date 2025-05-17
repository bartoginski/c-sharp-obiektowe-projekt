using Spectre.Console;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class HistoryTab
    {
        public int ShotsFired { get; private set; }
        public int Hits { get; private set; }
        public int Misses => ShotsFired - Hits;

        public void RecordShot(bool wasHit)
        {
            ShotsFired++;
            if (wasHit)
                Hits++;
        }

        public void DisplayHistoryTab()
        {
            AnsiConsole.Write(GetHistoryRenderable());
        }

        public Table GetHistoryRenderable()
        {
            var table = new Table()
                .Title("Historia Strzałów")
                .AddColumns("Łącznie", "Trafienia", "Pudła");

            table.AddRow(ShotsFired.ToString(), Hits.ToString(), Misses.ToString());

            return table;
        }
    }
}