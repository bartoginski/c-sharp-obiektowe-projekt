using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Utils;
using Spectre.Console;


namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class Board : IBoard
    {
        private const int boardSize = Constants.BoardSize;
        private Tile[,] tiles = new Tile[boardSize, boardSize];
        public List<ShipBase> ships = new List<ShipBase>
        {
            new BattleShip(),
            new Cruiser(),
            new Cruiser(),
            new Destroyer(),
            new Destroyer(),
            new Destroyer(),
            new Submarine(),
            new Submarine(),
            new Submarine(),
            new Submarine()
        };

        public Board()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    tiles[i, j] = new Tile();
                }
            }
        }


        public Tile GetTile(int x, int y)
        {
            return tiles[x, y];
        }

        // Implementation of PlaceShip
        public void PlaceShip(IShip ship, int x, int y, Direction direction)
        {
            if (direction == Direction.Horizontal)
            {
                for (int col = x; col < x + ship.Length; col++)
                {
                    tiles[y, col].HasShip = true;
                }
            }
            else // Vertical
            {
                for (int row = y; row < y + ship.Length; row++)
                {
                    tiles[row, x].HasShip = true;
                }
            }
        }

        public ShotResult Shoot(int x, int y)
        {
            // TODO: Implement shooting logic
            return ShotResult.Miss;
        }

        public void DisplayBoard(bool revealShips = true, KeyControl keyControl = null)
        {
            AnsiConsole.Write(GetBoardRenderable(revealShips, keyControl));
        }

        public Table GetBoardRenderable(bool revealShips, KeyControl keyControl = null)
        {
            string[] columnHeaders = Enumerable.Range(1, boardSize)
                .Select(i => i.ToString())
                .ToArray();
            string[] rowHeaders = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];

            var board = new Table()
                .Border(TableBorder.Rounded)
                .Expand()
                .ShowRowSeparators();
            // Add the empty corner cell
            board.AddColumn(new TableColumn(""));
            foreach (var header in columnHeaders)
            {
                board.AddColumn(new TableColumn(header).Centered());
            }
            for (int i = 0; i < boardSize; i++)
            {
                // Index column + 10 game columns == 11 columns in each row
                var rowData = new string[boardSize + 1];
                // Add index to first cell in each row
                rowData[0] = rowHeaders[i];
                for (int j = 0; j < boardSize; j++)
                {
                    // TODO: handle shoots 
                    // Check if this is a preview tile
                    if (keyControl != null && !KeyControl.placementComplete && keyControl.IsShipPreviewTile(i, j))
                    {
                        rowData[j + 1] = "O"; // Use a different symbol for preview
                    }
                    // Check if tile exists and has ship and isn't hidden
                    else if (tiles[i, j]?.HasShip != null && tiles[i, j].HasShip && revealShips)
                    {
                        rowData[j + 1] = "■";
                    }
                    else
                    {
                        rowData[j + 1] = "~";
                    }
                }
                board.AddRow(rowData);
            }
            return board;
        }

        public bool IsValidPlacement(IShip ship, int startX, int startY, Direction direction) // Renamed parameters to match x, y (col, row)
        {
            bool IsSpaceAvailableAroundTile(int row, int col)
            {
                if (row < 0 || row >= boardSize || col < 0 || col >= boardSize)
                    return false; // Out of bounds check is actually handled by the main loop

                if (GetTile(row, col).HasShip)
                    return false; // Tile already has a ship

                // Check 8 surrounding tiles
                for (int deltaRow = -1; deltaRow <= 1; deltaRow++)
                {
                    for (int deltaCol = -1; deltaCol <= 1; deltaCol++)
                    {
                        int neighborRow = row + deltaRow;
                        int neighborCol = col + deltaCol;

                        if (neighborRow >= 0 && neighborRow < boardSize &&
                            neighborCol >= 0 && neighborCol < boardSize &&
                            GetTile(neighborRow, neighborCol).HasShip)
                        {
                            return false; // Adjacent tile has a ship
                        }
                    }
                }
                return true;
            }

            if (direction == Direction.Horizontal)
            {
                // Check if ship fits horizontally within the board boundaries
                if (startX + ship.Length > boardSize)
                {
                    return false;
                }

                // Check each tile for availability before placing
                for (int col = startX; col < startX + ship.Length; col++)
                {
                    if (!IsSpaceAvailableAroundTile(startY, col))
                    {
                        return false;
                    }
                }
            }
            else // Vertical
            {
                // Check if ship fits vertically within the board boundaries
                if (startY + ship.Length > boardSize)
                {
                    return false;
                }

                // Check each tile for availability before placing
                for (int row = startY; row < startY + ship.Length; row++)
                {
                    if (!IsSpaceAvailableAroundTile(row, startX))
                    {
                        return false;
                    }
                }
            }

            return true; // Placement is valid
        }
    }
}