using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Utils;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class KeyControl
    {
        private readonly Board _board;

        public static int CurrentShipIndexForPlacement = 0; // Index of currently placed ship
        public static bool PlacementComplete = false;

        // Current cursor coordinates on board (top-left corner of ship)
        private int _xCoor = 0;
        private int _yCoor = 0;

        public KeyControl(Board board)
        {
            _board = board;
            // Reset state when creating new KeyControl
            CurrentShipIndexForPlacement = 0;
            PlacementComplete = false;
            _xCoor = 0;
            _yCoor = 0;
        }

        public int GetCurrentX() => _xCoor;
        public int GetCurrentY() => _yCoor;

        public void HandleKeyPress()
        {
            if (PlacementComplete || CurrentShipIndexForPlacement >= _board.Ships.Count)
            {
                PlacementComplete = true;
                return;
            }

            var currentShip = _board.Ships[CurrentShipIndexForPlacement];
            var key = Console.ReadKey(true).Key;
            
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (_xCoor > 0) _xCoor--;
                    break;
                case ConsoleKey.RightArrow:
                    // Limit right movement so ship doesn't go off board
                    int maxX = Constants.BoardSize - (currentShip.IsHorizontal ? currentShip.Length : 1);
                    if (_xCoor < maxX) _xCoor++;
                    break;
                case ConsoleKey.UpArrow:
                    if (_yCoor > 0) _yCoor--;
                    break;
                case ConsoleKey.DownArrow:
                    // Limit down movement
                    int maxY = Constants.BoardSize - (currentShip.IsHorizontal ? 1 : currentShip.Length);
                    if (_yCoor < maxY) _yCoor++;
                    break;
                case ConsoleKey.Spacebar:
                    // Try to rotate ship
                    currentShip.IsHorizontal = !currentShip.IsHorizontal;
                    
                    // Check if ship still fits on board after rotation
                    int newMaxXAfterRotate = Constants.BoardSize - (currentShip.IsHorizontal ? currentShip.Length : 1);
                    int newMaxYAfterRotate = Constants.BoardSize - (currentShip.IsHorizontal ? 1 : currentShip.Length);

                    if (_xCoor > newMaxXAfterRotate) _xCoor = newMaxXAfterRotate;
                    if (_yCoor > newMaxYAfterRotate) _yCoor = newMaxYAfterRotate;
                    
                    // Full validation happens on Enter press
                    Direction prospectiveDir = currentShip.IsHorizontal ? Direction.Horizontal : Direction.Vertical;
                    break;
                case ConsoleKey.Enter:
                    Direction dir = currentShip.IsHorizontal ? Direction.Horizontal : Direction.Vertical;
                    if (_board.IsValidPlacement(currentShip, _xCoor, _yCoor, dir))
                    {
                        _board.PlaceShip(currentShip, _xCoor, _yCoor, dir);
                        CurrentShipIndexForPlacement++;
                        _xCoor = 0; // Reset position for next ship
                        _yCoor = 0;
                        if (CurrentShipIndexForPlacement >= _board.Ships.Count)
                        {
                            PlacementComplete = true;
                        }
                    }
                    else
                    {
                        // Optional: sound or error message
                        // AnsiConsole.MarkupLine("[red]Cannot place ship here![/]");
                        // Thread.Sleep(200);
                    }
                    break;
                case ConsoleKey.Escape: // Allow early completion of placement
                    PlacementComplete = true;
                    break;
            }
        }

        public bool IsShipPreviewTile(int row, int col)
        {
            if (PlacementComplete || CurrentShipIndexForPlacement >= _board.Ships.Count)
                return false;

            var currentShip = _board.Ships[CurrentShipIndexForPlacement];
            if (currentShip.IsHorizontal)
            {
                return row == _yCoor && col >= _xCoor && col < _xCoor + currentShip.Length;
            }

            // Vertical placement
            return col == _xCoor && row >= _yCoor && row < _yCoor + currentShip.Length;
        }
    }
}