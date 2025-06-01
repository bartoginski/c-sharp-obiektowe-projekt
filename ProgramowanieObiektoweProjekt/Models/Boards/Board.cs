using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Utils;
using Spectre.Console;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    public class Board
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

        // CHANGED: Implement shooting logic using Tile.IsHit and Tile.HasShip
        public ShotResult Shoot(int x, int y)
        {
            var tile = GetTile(y, x); // y=row, x=column (matches rest of code)
            if (tile.IsHit)
            {
                // Already shot here; you might want to handle this differently
                return ShotResult.Miss;
            }
            tile.IsHit = true;
            if (tile.HasShip)
            {
                // (Optional) TODO: Add logic to check if a ship is sunk
                return ShotResult.Hit;
            }
            return ShotResult.Miss;
        }

        public void DisplayBoard(bool revealShips = true, KeyControl? keyControl = null)
        {
            AnsiConsole.Write(GetBoardRenderable(revealShips, keyControl));
        }

        public Table GetBoardRenderable(bool revealShips, KeyControl? keyControl = null)
        {
            string[] columnHeaders = Enumerable.Range(1, boardSize)
                .Select(i => i.ToString())
                .ToArray();
            string[] rowHeaders = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

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
                var rowData = new string[boardSize + 1];
                rowData[0] = rowHeaders[i];
                for (int j = 0; j < boardSize; j++)
                {
                    // TODO: handle shoots
                    if (keyControl != null && !KeyControl.placementComplete && keyControl.IsShipPreviewTile(i, j))
                    {
                        rowData[j + 1] = "O";
                    }
                    else if (tiles[i, j]?.HasShip != null && tiles[i, j].HasShip && revealShips)
                    {
                        rowData[j + 1] = "\u25a0";
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

        public bool IsValidPlacement(IShip ship, int startX, int startY, Direction direction)
        {
            bool IsSpaceAvailableAroundTile(int row, int col)
            {
                if (row < 0 || row >= boardSize || col < 0 || col >= boardSize)
                    return false;

                if (GetTile(row, col).HasShip)
                    return false;

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
                            return false;
                        }
                    }
                }
                return true;
            }

            if (direction == Direction.Horizontal)
            {
                if (startX + ship.Length > boardSize)
                {
                    return false;
                }

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
                if (startY + ship.Length > boardSize)
                {
                    return false;
                }

                for (int row = startY; row < startY + ship.Length; row++)
                {
                    if (!IsSpaceAvailableAroundTile(row, startX))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}