using ProgramowanieObiektoweProjekt.Interfaces;

namespace ProgramowanieObiektoweProjekt.Models.Ships
{
    abstract class ShipBase : IShip
    {
        public string Name { get; protected set; }
        public int Length { get; protected set; }
        public int NumberOfShips { get; protected set; } // Static property for ship type

        public bool IsHorizontal { get; set; } = true; // Used in KeyControl and BotEasy

        protected int Hits;
        public bool IsSunk => Hits >= Length;

        // Properties to track ship position on board
        public int StartCol { get; set; }
        public int StartRow { get; set; }
        public List<(int col, int row)> OccupiedTilesList { get; private set; } = new List<(int, int)>();

        public virtual void Hit()
        {
            if (!IsSunk) // Count hits only if ship is not already sunk
            {
                Hits++;
            }
        }

        public void AddOccupiedTile(int col, int row)
        {
            if (!OccupiedTilesList.Contains((col, row))) // Avoid duplicates
            {
                OccupiedTilesList.Add((col, row));
            }
        }

        public void ClearOccupiedTiles()
        {
            OccupiedTilesList.Clear();
        }
    }
}