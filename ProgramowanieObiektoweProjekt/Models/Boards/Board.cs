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
            for (int i = 0; i < boardSize; i++) // i to wiersz
            {
                for (int j = 0; j < boardSize; j++) // j to kolumna
                {
                    tiles[i, j] = new Tile();
                }
            }
        }

        public Tile GetTile(int row, int col)
        {
            if (row >= 0 && row < boardSize && col >= 0 && col < boardSize)
            {
                return tiles[row, col];
            }
            throw new ArgumentOutOfRangeException($"Koordynaty ({row},{col}) są poza planszą.");
        }

        public void PlaceShip(IShip ship, int startCol, int startRow, Direction direction)
        {
            var shipBase = ship as ShipBase;
            if (shipBase == null) throw new ArgumentException("Statek musi być typu ShipBase", nameof(ship));

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

        public ShotResult Shoot(int col, int row) // col to kolumna, row to wiersz
        {
            Tile targetTile = GetTile(row, col);

            if (targetTile.IsHit)
            {
                // Zwracamy Miss, aby uniknąć podwójnego karania gracza lub bota,
                // logika sprawdzania IsHit przed strzałem jest w Menu.cs / BotEasy.cs
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
            if (sunkShip == null || !sunkShip.IsSunk) return newlyMarkedCells;

            foreach (var (shipCol, shipRow) in sunkShip.OccupiedTilesList)
            {
                for (int rOffset = -1; rOffset <= 1; rOffset++)
                {
                    for (int cOffset = -1; cOffset <= 1; cOffset++)
                    {
                        int adjacentRow = shipRow + rOffset;
                        int adjacentCol = shipCol + cOffset;

                        if (adjacentRow >= 0 && adjacentRow < boardSize &&
                            adjacentCol >= 0 && adjacentCol < boardSize)
                        {
                            Tile neighborTile = GetTile(adjacentRow, adjacentCol);
                            if (!neighborTile.IsHit) // Oznaczaj tylko nietrafione wcześniej pola
                            {
                                neighborTile.IsHit = true; // Oznacz jako "trafione" (w sensie wyłączone z gry)
                                newlyMarkedCells.Add((adjacentCol, adjacentRow));
                            }
                        }
                    }
                }
            }
            return newlyMarkedCells;
        }

        public bool AreAllShipsSunk()
        {
            if (!ships.Any()) return true; 
            return ships.All(ship => ship.IsSunk);
        }

        public void DisplayBoard(bool revealShips = true, KeyControl? keyControl = null)
        {
            // Domyślnie bez trybu strzelania i bez kursora dla tej uproszczonej metody
            AnsiConsole.Write(GetBoardRenderable(revealShips, keyControl, false, -1, -1));
        }

        // Zmodyfikowana sygnatura metody
        public Table GetBoardRenderable(bool revealShips, KeyControl? keyControl = null, bool isShootingCursorActive = false, int shootCursorCol = -1, int shootCursorRow = -1)
        {
            string[] columnHeaders = Enumerable.Range(1, boardSize)
                .Select(i => i.ToString())
                .ToArray();
            string[] rowHeaders = Enumerable.Range(0, boardSize).Select(i => ((char)('A' + i)).ToString()).ToArray();

            var boardTable = new Table()
                .Border(TableBorder.Rounded)
                .Expand()
                .ShowRowSeparators();

            boardTable.AddColumn(new TableColumn(""));
            foreach (var header in columnHeaders)
            {
                boardTable.AddColumn(new TableColumn(header).Centered());
            }

            for (int i = 0; i < boardSize; i++) // i to indeks wiersza
            {
                var rowData = new string[boardSize + 1];
                rowData[0] = rowHeaders[i];
                for (int j = 0; j < boardSize; j++) // j to indeks kolumny
                {
                    Tile currentTile = GetTile(i, j);
                    string tileDisplay;

                    // Logika dla podglądu umieszczania statku (z KeyControl)
                    if (keyControl != null && !KeyControl.placementComplete &&
                        KeyControl.currentShipIndexForPlacement < ships.Count && // Dodatkowe sprawdzenie
                        keyControl.IsShipPreviewTile(i, j)) // i = wiersz, j = kolumna
                    {
                        var currentShipForPlacement = ships[KeyControl.currentShipIndexForPlacement];
                        Direction currentPlacementDir = currentShipForPlacement.IsHorizontal ? Direction.Horizontal : Direction.Vertical;
                        if (!IsValidPlacement(currentShipForPlacement, keyControl.GetCurrentX(), keyControl.GetCurrentY(), currentPlacementDir))
                        {
                            tileDisplay = "[red]o[/]"; // Nakładanie się lub zła pozycja
                        }
                        else
                        {
                           tileDisplay = "[yellow]O[/]"; // Poprawny podgląd
                        }
                    }
                    // Logika dla kursora strzelania
                    else if (isShootingCursorActive && j == shootCursorCol && i == shootCursorRow) // j = kolumna, i = wiersz
                    {
                        if (currentTile.IsHit)
                        {
                            tileDisplay = currentTile.OccupyingShip != null ? "[black on red]X[/]" : "[black on blue]M[/]";
                        }
                        else 
                        {
                            if (revealShips && currentTile.OccupyingShip != null) // Używane dla DevMode na planszy przeciwnika
                            {
                                tileDisplay = "[black on yellow]S[/]"; 
                            }
                            else
                            {
                                tileDisplay = "[black on yellow]~[/]"; 
                            }
                        }
                    }
                    // Normalne renderowanie komórki
                    else
                    {
                        tileDisplay = GetRegularTileDisplay(currentTile, revealShips);
                    }
                    rowData[j + 1] = tileDisplay;
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
                    return "[red]X[/]"; // Trafiony statek
                }
                else
                {
                    return "[blue]M[/]"; // Pudło (Miss)
                }
            }
            else // Pole nie zostało jeszcze ostrzelane
            {
                if (revealShips && currentTile.OccupyingShip != null)
                {
                    return "[grey]█[/]"; // Statek (widoczny na planszy gracza lub DevMode)
                }
                else
                {
                    return "[deepskyblue1]░[/]"; // Woda
                }
            }
        }

        public bool IsValidPlacement(IShip ship, int startCol, int startRow, Direction direction)
        {
            var shipToCheck = ship as ShipBase;
            if (shipToCheck == null) return false;

            // Sprawdzenie, czy statek mieści się w granicach planszy
            if (direction == Direction.Horizontal)
            {
                if (startCol < 0 || startRow < 0 || startRow >= boardSize || startCol + ship.Length > boardSize)
                    return false;
            }
            else // Vertical
            {
                if (startCol < 0 || startRow < 0 || startCol >= boardSize || startRow + ship.Length > boardSize)
                    return false;
            }
            
            // Sprawdzenie, czy statek lub jego otoczenie nie koliduje z innym statkiem
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

                // Sprawdzenie pola i 8 pól dookoła
                for (int rOffset = -1; rOffset <= 1; rOffset++)
                {
                    for (int cOffset = -1; cOffset <= 1; cOffset++)
                    {
                        int checkRow = currentSegmentRow + rOffset;
                        int checkCol = currentSegmentCol + cOffset;

                        if (checkRow >= 0 && checkRow < boardSize && checkCol >= 0 && checkCol < boardSize)
                        {
                            Tile tileToVerify = GetTile(checkRow, checkCol);
                            // Jeśli na sprawdzanym polu jest statek i nie jest to ten sam statek, który właśnie umieszczamy
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

        // --- USUNIĘTA ZDUPLIKOWANA METODA ISVALIDPLACEMENT ---

    }
}