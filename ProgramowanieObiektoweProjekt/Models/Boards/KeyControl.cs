namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class KeyControl
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

        public bool IsShipPreviewTile(int row, int col)
        {
            if (_board.ships[currentShip].IsHorizontal)
            {
                return row == x_coor && col >= y_coor && col < y_coor + _board.ships[currentShip].Length && col < 10;
            }
            else
            {
                return col == y_coor && row >= x_coor && row < x_coor + _board.ships[currentShip].Length && row < 10;
            }
        }

        // Check if tiles aroud are avaiable
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
            if (_board.ships[currentShip].IsHorizontal)
            {
                if (x_coor + _board.ships[currentShip].Length > 10)
                {
                    return false;
                }
                // Check if there is no ships on the way in the row
                for (int row = x_coor; row < x_coor + _board.ships[currentShip].Length; row++)
                {
                    if (_board.GetTile(row,y_coor).HasShip || !IsTileAvaiable(row, y_coor))
                    {
                        return false;
                    }
                }
            }

            else
            {
                if (y_coor + _board.ships[currentShip].Length > 10)
                {
                    return false;
                }
                // Check if there is no ships on the way in the col
                for (int col = y_coor; col < y_coor + _board.ships[currentShip].Length; col++)
                {
                    if (_board.GetTile(x_coor,col).HasShip || !IsTileAvaiable(x_coor, col))
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
                if (y_coor + _board.ships[currentShip].Length > 10)
                {
                    return false;
                }

                for (int col = y_coor; col < y_coor + _board.ships[currentShip].Length; col++)
                {
                    if (_board.GetTile(x_coor, col).HasShip || !IsTileAvaiable(x_coor, col))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (x_coor + _board.ships[currentShip].Length > 10)
                {
                    return false;
                }

                for (int row = x_coor; row < x_coor + _board.ships[currentShip].Length; row++)
                {
                    if (_board.GetTile(row, y_coor).HasShip || !IsTileAvaiable(row, y_coor))
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
                for (int col = y_coor; col < y_coor + _board.ships[currentShip].Length; col++)
                {
                    _board.GetTile(x_coor, col).HasShip = true;
                }
            }
            else
            {
                for (int row = x_coor; row < x_coor + _board.ships[currentShip].Length; row++)
                {
                    _board.GetTile(row, y_coor).HasShip = true;
                }
            }
        }



    }
}
