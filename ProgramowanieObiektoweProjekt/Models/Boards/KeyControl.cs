namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    public class KeyControl
    {
        private readonly Board _board;

        public KeyControl(Board board)
        {
            _board = board;
        }
        static int currentShip = 0;
        public static bool placementComplete = false;

        int x_coor = 0;
        int y_coor = 0;

        public void HandleKeyPress()
        {
            var key = Console.ReadKey(true).Key;


            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (x_coor > 0)
                    {
                        x_coor--;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (x_coor < 9 - (_board.ships[currentShip].IsHorizontal ? _board.ships[currentShip].Length - 1 : 0))
                    {
                        x_coor++;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (y_coor > 0)
                    {
                        y_coor--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (y_coor < 9 - (_board.ships[currentShip].IsHorizontal ? 0 : _board.ships[currentShip].Length - 1))
                    {
                        y_coor++;
                    }
                    break;
                case ConsoleKey.Spacebar:
                    if (CanRotateShip())
                    {
                        _board.ships[currentShip].IsHorizontal = !_board.ships[currentShip].IsHorizontal;
                    }
                    break;
                case ConsoleKey.Enter:
                    if (CanPlaceShipHere())
                    {
                        PlaceShipOnBoard();
                        currentShip++;
                        if (currentShip >= _board.ships.Count)
                        {
                            placementComplete = true;
                        }
                    }
                    break;
                case ConsoleKey.Escape:
                    placementComplete = true;
                    break;
            }
        }

        // TODO: fix bug with rotation
        public bool IsShipPreviewTile(int row, int col)
        {
            if (_board.ships[currentShip].IsHorizontal)
            {
                // If ship is horizontal, the row stays fixed (y_coor) and columns vary
                return row == y_coor && col >= x_coor && col < x_coor + _board.ships[currentShip].Length;
            }
            // If ship is vertical, the column stays fixed (x_coor) and rows vary
            return col == x_coor && row >= y_coor && row < y_coor + _board.ships[currentShip].Length;
        }

        // Check if tiles around are available
        public bool IsTileAvaiable(int x_coor, int y_coor)
        {
            for (int deltaRow = -1; deltaRow <= 1; deltaRow++)
            {
                for (int deltaCol = -1; deltaCol <= 1; deltaCol++)
                {
                    int neighborRow = x_coor + deltaRow;
                    int neighborCol = y_coor + deltaCol;

                    if (neighborRow >= 0 && neighborRow < 10 &&
                        neighborCol >= 0 && neighborCol < 10 &&
                        _board.GetTile(neighborRow, neighborCol).HasShip)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CanRotateShip()
        {
            // If ship is currently horizontal and we want to check if it can be vertical
            if (_board.ships[currentShip].IsHorizontal)
            {
                // Check if ship would fit vertically at current position
                if (y_coor + _board.ships[currentShip].Length > 10)
                {
                    return false;
                }
        
                // Check if there are no ships in the way when placed vertically
                for (int row = y_coor; row < y_coor + _board.ships[currentShip].Length; row++)
                {
                    if (_board.GetTile(row, x_coor).HasShip || !IsTileAvaiable(row, x_coor))
                    {
                        return false;
                    }
                }
            }
            // If ship is currently vertical and we want to check if it can be horizontal
            else
            {
                // Check if ship would fit horizontally at current position
                if (x_coor + _board.ships[currentShip].Length > 10)
                {
                    return false;
                }
        
                // Check if there are no ships in the way when placed horizontally
                for (int col = x_coor; col < x_coor + _board.ships[currentShip].Length; col++)
                {
                    if (_board.GetTile(y_coor, col).HasShip || !IsTileAvaiable(y_coor, col))
                    {
                        return false;
                    }
                }
            }
    
            return true;
        }

        public bool CanPlaceShipHere()
        {
            if (_board.ships[currentShip].IsHorizontal)
            {
                // For horizontal ships, check if it fits within the board horizontally
                if (x_coor + _board.ships[currentShip].Length > 10)
                {
                    return false;
                }
        
                // Check each column in the horizontal ship's placement
                for (int col = x_coor; col < x_coor + _board.ships[currentShip].Length; col++)
                {
                    if (_board.GetTile(y_coor, col).HasShip || !IsTileAvaiable(y_coor, col))
                    {
                        return false;
                    }
                }
            }
            else
            {
                // For vertical ships, check if it fits within the board vertically
                if (y_coor + _board.ships[currentShip].Length > 10)
                {
                    return false;
                }
        
                // Check each row in the vertical ship's placement
                for (int row = y_coor; row < y_coor + _board.ships[currentShip].Length; row++)
                {
                    if (_board.GetTile(row, x_coor).HasShip || !IsTileAvaiable(row, x_coor))
                    {
                        return false;
                    }
                }
            }
    
            return true;
        }

        public void PlaceShipOnBoard()
        {
            if (_board.ships[currentShip].IsHorizontal)
            {
                // Place a horizontal ship by setting HasShip to true for each column
                for (int col = x_coor; col < x_coor + _board.ships[currentShip].Length; col++)
                {
                    _board.GetTile(y_coor, col).HasShip = true;
                }
            }
            else
            {
                // Place a vertical ship by setting HasShip to true for each row
                for (int row = y_coor; row < y_coor + _board.ships[currentShip].Length; row++)
                {
                    _board.GetTile(row, x_coor).HasShip = true;
                }
            }
        }
    }
}
