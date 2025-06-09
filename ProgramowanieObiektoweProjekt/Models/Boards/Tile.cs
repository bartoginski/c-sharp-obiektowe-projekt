using ProgramowanieObiektoweProjekt.Models.Ships; // Potrzebne dla ShipBase

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class Tile
    {
        public ShipBase? OccupyingShip { get; set; } = null; // Statek zajmujący to pole, null jeśli brak
        public bool IsHit { get; set; } = false; // Czy pole zostało trafione

        // Właściwość pomocnicza, jeśli HasShip jest często używane w innych częściach kodu
        public bool HasShip => OccupyingShip != null;
    }
}