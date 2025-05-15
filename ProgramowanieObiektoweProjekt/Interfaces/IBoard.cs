using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;
namespace ProgramowanieObiektoweProjekt.Interfaces
{
    internal interface IBoard
    {
        /// <summary>
        /// Place a ship on the board in a specified position and direction.
        /// </summary>
        /// <param name="ship">Ship to place</param>
        /// <param name="x">Starting position X on the board.</param>
        /// <param name="y">Starting position Y on the board.</param>
        /// <param name="direction">Direction of the ship (vertical or horizontal)</param>
        void PlaceShip(IShip ship, int x, int y, Direction direction);

        /// <summary>
        /// Fires shot at the given coordiantes on the board.
        /// </summary>
        /// <param name="x">X coordinate of the shot.</param>
        /// <param name="y">Y coordinate of the shot.</param>
        /// <returns>Result of the shot (hit, miss or sink).</returns>
        ShotResult Shoot(int x, int y);

        /// <summary>
        /// Displays the borad in the terminal.
        /// </summary>
        /// <param name="revealShips">Whether to show ship positions (true) or hide (false)</param>
        /// <param name="keyControl">KeyControl instance for showing ship placement preview, or null if not in placement mode</param>
        void DisplayBoard(bool revealShips, KeyControl keyControl);
    }
}
