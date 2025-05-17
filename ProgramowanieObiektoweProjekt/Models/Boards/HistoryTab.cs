using Spectre.Console;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    /// <summary>
    /// Represents a tab that tracks and displays shot history statistics in the game.
    /// </summary>
    internal class HistoryTab
    {
        /// <summary>
        /// Gets the total number of shots fired by the player.
        /// </summary>
        public int ShotsFired { get; private set; }

        /// <summary>
        /// Gets the total number of successful hits.
        /// </summary>
        public int Hits { get; private set; }

        /// <summary>
        /// Gets the total number of missed shots, calculated as the difference
        /// between total shots fired and successful hits.
        /// </summary>
        public int Misses => ShotsFired - Hits;

        /// <summary>
        /// Records a new shot in the history and updates statistics.
        /// </summary>
        /// <param name="wasHit">True if the shot was a hit, false if it was a miss.</param>
        public void RecordShot(bool wasHit)
        {
            ShotsFired++;
            if (wasHit)
                Hits++;
        }

        /// <summary>
        /// Displays the shot history statistics in the console.
        /// </summary>
        public void DisplayHistoryTab()
        {
            AnsiConsole.Write(GetHistoryRenderable());
        }

        /// <summary>
        /// Creates and returns a formatted table containing shot history statistics.
        /// </summary>
        /// <returns>A Spectre.Console Table object with formatted history data.</returns>
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