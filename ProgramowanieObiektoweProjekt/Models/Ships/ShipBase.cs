using ProgramowanieObiektoweProjekt.Interfaces;

namespace ProgramowanieObiektoweProjekt.Models.Ships
{
    abstract class ShipBase : IShip
    {
        public string Name { get; protected set; }
        public int Length { get; protected set; }
        public int NumberOfShips { get; protected set; } // Ta właściwość jest bardziej statyczna dla typu statku

        public bool IsHorizontal { get; set; } = true; // Używane w KeyControl i BotEasy

        protected int Hits;
        public bool IsSunk => Hits >= Length;

        // Nowe właściwości do śledzenia pozycji statku
        public int StartCol { get; set; }
        public int StartRow { get; set; }
        // Usunięto ShipDirection, ponieważ IsHorizontal już to pokrywa i jest używane
        // public Direction ShipDirection { get; set; } 
        public List<(int col, int row)> OccupiedTilesList { get; private set; } = new List<(int, int)>();

        public virtual void Hit()
        {
            if (!IsSunk) // Zliczaj trafienia tylko jeśli statek nie jest jeszcze zatopiony
            {
                Hits++;
            }
        }

        public void AddOccupiedTile(int col, int row)
        {
            if (!OccupiedTilesList.Contains((col, row))) // Unikaj duplikatów, choć nie powinno ich być
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