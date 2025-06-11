using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Utils;
using Spectre.Console;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class Board : IBoard
    {
        private const int BoardSize = Constants.BoardSize;
        private Tile[,] _tiles = new Tile[BoardSize, BoardSize];

        public List<ShipBase> Ships =
        [
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
        ];

        public Board()
        {
            PopulateTiles();
        }

        private void PopulateTiles()
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    _tiles[row, col] = new Tile();
                }
            }
        }

        public Tile GetTile(int row, int col)
        {
            if (row >= 0 && row < BoardSize && col >= 0 && col < BoardSize)
            {
                return _tiles[row, col];
            }

            throw new ArgumentOutOfRangeException($"Coordinates ({row},{col}) are outside the board.");
        }

        public void PlaceShip(IShip ship, int startCol, int startRow, Direction direction)
        {
            var shipBase = ship as ShipBase;
            if (shipBase == null) throw new ArgumentException("Ship must be of type ShipBase", nameof(ship));

            shipBase.StartCol = startCol;
            shipBase.StartRow = startRow;
            shipBase.IsHorizontal = (direction == Direction.Horizontal);
            shipBase.ClearOccupiedTiles();

            if (direction == Direction.Horizontal)
            {
                for (int col = startCol; col < startCol + ship.Length; col++)
                {
                    GetTile(startRow, col).OccupyingShip = shipBase;
                    shipBase.AddOccupiedTile(col, startRow);
                }
            }
            else // Vertical
            {
                for (int row = startRow; row < startRow + ship.Length; row++)
                {
                    GetTile(row, startCol).OccupyingShip = shipBase;
                    shipBase.AddOccupiedTile(startCol, row);
                }
            }
        }

        public ShotResult Shoot(int col, int row)
        {
            Tile targetTile = GetTile(row, col);

            if (targetTile.IsHit)
            {
                // Return Miss to avoid double-penalizing player or bot
                // Hit checking logic is handled in Menu.cs / BotEasy.cs
                return ShotResult.Miss;
            }

            targetTile.IsHit = true;

            if (targetTile.OccupyingShip != null)
            {
                targetTile.OccupyingShip.Hit();
                if (targetTile.OccupyingShip.IsSunk)
                {
                    return ShotResult.Sunk;
                }

                return ShotResult.Hit;
            }

            return ShotResult.Miss;
        }

        public List<(int col, int row)> MarkAroundSunkShip(ShipBase sunkShip)
        {
            List<(int col, int row)> newlyMarkedCells = new List<(int, int)>();
            if (!sunkShip.IsSunk) return newlyMarkedCells;

            foreach (var (shipCol, shipRow) in sunkShip.OccupiedTilesList)
            {
                // Mark all 8 surrounding tiles
                for (int rOffset = -1; rOffset <= 1; rOffset++)
                {
                    for (int cOffset = -1; cOffset <= 1; cOffset++)
                    {
                        int adjacentRow = shipRow + rOffset;
                        int adjacentCol = shipCol + cOffset;

                        if (adjacentRow >= 0 && adjacentRow < BoardSize &&
                            adjacentCol >= 0 && adjacentCol < BoardSize)
                        {
                            Tile neighborTile = GetTile(adjacentRow, adjacentCol);
                            if (neighborTile.IsHit) continue; // Only mark previously unhit tiles
                            neighborTile.IsHit = true; // Mark as "hit" (disabled from game)
                            newlyMarkedCells.Add((adjacentCol, adjacentRow));
                        }
                    }
                }
            }

            return newlyMarkedCells;
        }

        public bool AreAllShipsSunk()
        {
            return Ships.Count == 0 || Ships.All(ship => ship.IsSunk);
        }

        public void DisplayBoard(bool revealShips = true, KeyControl? keyControl = null)
        {
            // Default without shooting mode and cursor for this simplified method
            AnsiConsole.Write(GetBoardRenderable(revealShips, keyControl, false, -1, -1));
        }

        public Table GetBoardRenderable(bool revealShips, KeyControl? keyControl = null,
            bool isShootingCursorActive = false, int shootCursorCol = -1, int shootCursorRow = -1)
        {
            string[] columnHeaders = Enumerable.Range(1, BoardSize)
                .Select(i => i.ToString())
                .ToArray();
            string[] rowHeaders = Enumerable.Range(0, BoardSize).Select(i => ((char)('A' + i)).ToString()).ToArray();

            var boardTable = new Table()
                .Border(TableBorder.Rounded)
                .Expand()
                .ShowRowSeparators();

            boardTable.AddColumn(new TableColumn(""));
            foreach (var header in columnHeaders)
            {
                boardTable.AddColumn(new TableColumn(header).Centered());
            }

            for (int row = 0; row < BoardSize; row++)
            {
                var rowData = new string[BoardSize + 1];
                rowData[0] = rowHeaders[row];
                for (int col = 0; col < BoardSize; col++)
                {
                    Tile currentTile = GetTile(row, col);
                    string tileDisplay;

                    // Ship placement preview logic (with KeyControl)
                    if (keyControl != null && !KeyControl.PlacementComplete &&
                        KeyControl.CurrentShipIndexForPlacement < Ships.Count &&
                        keyControl.IsShipPreviewTile(row, col)) // i = row, j = column
                    {
                        var currentShipForPlacement = Ships[KeyControl.CurrentShipIndexForPlacement];
                        Direction currentPlacementDir = currentShipForPlacement.IsHorizontal
                            ? Direction.Horizontal
                            : Direction.Vertical;
                        if (!IsValidPlacement(currentShipForPlacement, keyControl.GetCurrentX(),
                                keyControl.GetCurrentY(), currentPlacementDir))
                        {
                            tileDisplay = "[red]o[/]"; // Overlapping or invalid position
                        }
                        else
                        {
                            tileDisplay = "[yellow]O[/]"; // Valid preview
                        }
                    }
                    // Shooting cursor logic
                    else if (isShootingCursorActive && col == shootCursorCol &&
                             row == shootCursorRow)
                    {
                        if (currentTile.IsHit)
                        {
                            tileDisplay = currentTile.OccupyingShip != null
                                ? "[black on red]X[/]"
                                : "[black on blue]░[/]";
                        }
                        else
                        {
                            if (revealShips &&
                                currentTile.OccupyingShip != null) // Used for DevMode on opponent's board
                            {
                                tileDisplay = "[black on yellow]S[/]";
                            }
                            else
                            {
                                tileDisplay = "[black on yellow]~[/]";
                            }
                        }
                    }
                    // Normal tile rendering
                    else
                    {
                        tileDisplay = GetRegularTileDisplay(currentTile, revealShips);
                    }

                    rowData[col + 1] = tileDisplay;
                }

                boardTable.AddRow(rowData);
            }

            return boardTable;
        }

        private string GetRegularTileDisplay(Tile currentTile, bool revealShips)
        {
            if (currentTile.IsHit)
            {
                if (currentTile.OccupyingShip != null)
                {
                    return "[red]X[/]"; // Hit ship
                }
                else
                {
                    return "[blue]░[/]"; // Miss
                }
            }
            else // Tile has not been shot at yet
            {
                if (revealShips && currentTile.OccupyingShip != null)
                {
                    return "[grey]█[/]"; // Ship (visible on player's board or DevMode)
                }
                else
                {
                    return "[deepskyblue1][/]"; // Water
                }
            }
        }

        public bool IsValidPlacement(IShip ship, int startCol, int startRow, Direction direction)
        {
            var shipToCheck = ship as ShipBase;
            if (shipToCheck == null) return false;

            // Check if ship fits within board boundaries
            if (direction == Direction.Horizontal)
            {
                if (startCol < 0 || startRow < 0 || startRow >= BoardSize || startCol + ship.Length > BoardSize)
                    return false;
            }
            else // Vertical
            {
                if (startCol < 0 || startRow < 0 || startCol >= BoardSize || startRow + ship.Length > BoardSize)
                    return false;
            }

            // Check if ship or its surroundings collide with another ship
            for (int i = 0; i < ship.Length; i++)
            {
                int currentSegmentRow, currentSegmentCol;
                if (direction == Direction.Horizontal)
                {
                    currentSegmentRow = startRow;
                    currentSegmentCol = startCol + i;
                }
                else
                {
                    currentSegmentRow = startRow + i;
                    currentSegmentCol = startCol;
                }

                // Check the tile and 8 surrounding tiles
                for (int rOffset = -1; rOffset <= 1; rOffset++)
                {
                    for (int cOffset = -1; cOffset <= 1; cOffset++)
                    {
                        int checkRow = currentSegmentRow + rOffset;
                        int checkCol = currentSegmentCol + cOffset;

                        if (checkRow >= 0 && checkRow < BoardSize && checkCol >= 0 && checkCol < BoardSize)
                        {
                            Tile tileToVerify = GetTile(checkRow, checkCol);
                            // If there's a ship on the checked tile and it's not the same ship we're placing
                            if (tileToVerify.OccupyingShip != null && tileToVerify.OccupyingShip != shipToCheck)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}