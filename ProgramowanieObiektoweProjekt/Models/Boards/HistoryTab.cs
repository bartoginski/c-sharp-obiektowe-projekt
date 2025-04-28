using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    /// <summary>
    /// A class representing the game history, 
    /// storing information about shots fired, hits and misses.
    /// </summary>
    internal class HistoryTab
    {
        /// <summary>
        /// Number of shots fired.
        /// </summary>
        public int ShotsFired { get; private set; }

        /// <summary>
        /// Number of hits.
        /// </summary>
        public int Hits { get; private set; }

        /// <summary>
        /// Number of misses (difference between shots fired and hits).
        /// </summary>
        public int Misses => ShotsFired - Hits;

        /// <summary>
        /// Registers every shot fired and updates hit and miss statistics.
        /// </summary>
        /// <param name="wasHit">A logical value specifying whether the shot was a hit (true) or a miss (false).</param>
        public void RecordShot(bool wasHit)
        {
            // code...
        }

        /// <summary>
        /// Displays history of shots
        /// </summary>
        public void DisplayHistoryTab()
        {
            // code...
        }
    }
}
