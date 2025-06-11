using ProgramowanieObiektoweProjekt.Models.Ships;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class Tile
    {
        public ShipBase? OccupyingShip { get; set; } = null; // Ship occupying this tile, null if empty
        public bool IsHit { get; set; } = false; // Whether the tile has been hit

        // Helper property for checking if tile has a ship
        public bool HasShip => OccupyingShip != null;
    }
}