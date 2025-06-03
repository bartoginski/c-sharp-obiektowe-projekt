using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Utils;
using Spectre.Console;
using System; // Dla ArgumentOutOfRangeException, ArgumentException
using System.Linq; // Potrzebne dla .All(), .Any()
using System.Collections.Generic; // Potrzebne dla List

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
            // shipBase.ShipDirection = direction; // Usunięto, IsHorizontal wystarczy
            shipBase.ClearOccupiedTiles(); // Wyczyść poprzednie pozycje, jeśli statek był przemieszczany

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
                return ShotResult.Miss; // Strzał w już trafione pole (można rozważyć ShotResult.AlreadyHit)
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
                            if (!neighborTile.IsHit)
                            {
                                neighborTile.IsHit = true;
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
            if (!ships.Any()) return true; // Jeśli nie ma statków do zatopienia, uznajemy, że wszystkie są "zatopione" (lub gra nie powinna się zacząć)
            return ships.All(ship => ship.IsSunk);
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

                    if (keyControl != null && !KeyControl.placementComplete && 
                        KeyControl.currentShipIndexForPlacement < ships.Count && // Dodatkowe sprawdzenie
                        keyControl.IsShipPreviewTile(i, j))
                    {
                        // Sprawdzenie, czy można tu postawić (podglądowo)
                        bool canPlacePreview = _IsValidPlacementForPreview(
                            ships[KeyControl.currentShipIndexForPlacement], 
                            keyControl.GetCurrentX(), // Potrzebujemy metod w KeyControl do pobrania x_coor, y_coor
                            keyControl.GetCurrentY(), 
                            ships[KeyControl.currentShipIndexForPlacement].IsHorizontal ? Direction.Horizontal : Direction.Vertical,
                            i, j); // i,j to aktualnie renderowane pole
                        
                        if(canPlacePreview && keyControl.IsShipPreviewTile(i,j)) // IsShipPreviewTile określa kształt
                        {
                             tileDisplay = ships[KeyControl.currentShipIndexForPlacement].IsHorizontal 
                                ? ( _IsValidPlacementForPreview(ships[KeyControl.currentShipIndexForPlacement], keyControl.GetCurrentX(), keyControl.GetCurrentY(), Direction.Horizontal, -1, -1) ? "[green]O[/]" : "[grey]o[/]" )
                                : ( _IsValidPlacementForPreview(ships[KeyControl.currentShipIndexForPlacement], keyControl.GetCurrentX(), keyControl.GetCurrentY(), Direction.Vertical, -1, -1) ? "[green]O[/]" : "[grey]o[/]" );
                             if (keyControl.IsShipPreviewTile(i,j)) { // Jeśli to część statku
                                tileDisplay = "[yellow]O[/]"; // Standardowy podgląd
                                // Sprawdzenie, czy w ogóle można umieścić statek na aktualnej pozycji x_coor, y_coor
                                var currentShip = ships[KeyControl.currentShipIndexForPlacement];
                                Direction currentDir = currentShip.IsHorizontal ? Direction.Horizontal : Direction.Vertical;
                                if (!IsValidPlacement(currentShip, keyControl.GetCurrentX(), keyControl.GetCurrentY(), currentDir)) {
                                    tileDisplay = "[red]o[/]"; // Czerwony, jeśli nie można umieścić na pozycji kursora
                                }
                             } else {
                                tileDisplay = "[deepskyblue1]~[/]"; // Woda, jeśli to nie jest pole podglądu
                             }

                        } else if (keyControl != null && !KeyControl.placementComplete && keyControl.IsShipPreviewTile(i, j)) {
                            tileDisplay = "[red]o[/]"; // Jeśli to podgląd, ale nie można tu umieścić
                        }
                        else { // Normalne renderowanie, gdy nie ma podglądu
                           tileDisplay = GetRegularTileDisplay(currentTile, revealShips);
                        }
                    }
                    else // Normalne renderowanie, gdy nie ma podglądu
                    {
                       tileDisplay = GetRegularTileDisplay(currentTile, revealShips);
                    }
                    rowData[j + 1] = tileDisplay;
                }
                boardTable.AddRow(rowData);
            }
            return boardTable;
        }
        
        private string GetRegularTileDisplay(Tile currentTile, bool revealShips) {
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
                    return "[grey]S[/]"; // Statek (widoczny na planszy gracza)
                }
                else
                {
                    return "[deepskyblue1]~[/]"; // Woda
                }
            }
        }


        // Ta metoda jest używana tylko do podglądu w GetBoardRenderable, aby pokolorować podgląd statku
        // na czerwono, jeśli nie można go umieścić.
        // Parametry tileRow i tileCol (-1,-1 jeśli sprawdzamy ogólną możliwość umieszczenia)
        // odnoszą się do aktualnie renderowanego pola planszy, aby sprawdzić, czy jest częścią nieprawidłowego podglądu.
        private bool _IsValidPlacementForPreview(IShip ship, int startCol, int startRow, Direction direction, int tileRowToRender, int tileColToRender) {
            // Szybkie sprawdzenie, czy statek w ogóle mieści się na planszy
            if (direction == Direction.Horizontal) {
                if (startCol < 0 || startRow < 0 || startRow >= boardSize || startCol + ship.Length > boardSize) return false;
            } else {
                if (startCol < 0 || startRow < 0 || startCol >= boardSize || startRow + ship.Length > boardSize) return false;
            }
            // Pełna walidacja za pomocą głównej metody
            return IsValidPlacement(ship, startCol, startRow, direction);
        }


        public bool IsValidPlacement(IShip ship, int startCol, int startRow, Direction direction)
        {
            var shipToCheck = ship as ShipBase;
            if (shipToCheck == null) return false;

            // 1. Sprawdzenie, czy statek mieści się na planszy
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

            // 2. Sprawdzenie każdego pola, które statek ma zająć, oraz jego bezpośredniego otoczenia (bufor 1-polowy)
            for (int i = 0; i < ship.Length; i++)
            {
                int currentSegmentRow, currentSegmentCol;
                if (direction == Direction.Horizontal)
                {
                    currentSegmentRow = startRow;
                    currentSegmentCol = startCol + i;
                }
                else // Vertical
                {
                    currentSegmentRow = startRow + i;
                    currentSegmentCol = startCol;
                }

                // Sprawdź pole segmentu i jego 8 sąsiadów
                for (int rOffset = -1; rOffset <= 1; rOffset++)
                {
                    for (int cOffset = -1; cOffset <= 1; cOffset++)
                    {
                        int checkRow = currentSegmentRow + rOffset;
                        int checkCol = currentSegmentCol + cOffset;

                        if (checkRow >= 0 && checkRow < boardSize && checkCol >= 0 && checkCol < boardSize)
                        {
                            Tile tileToVerify = GetTile(checkRow, checkCol);
                            if (tileToVerify.OccupyingShip != null && tileToVerify.OccupyingShip != shipToCheck) // Jeśli jest inny statek
                            {
                                // Czy to pole jest częścią aktualnie umieszczanego statku? NIE - bo sprawdzamy OTOCZENIE
                                // Jeśli OccupyingShip nie jest nullem I NIE jest tym samym statkiem, który sprawdzamy (ważne przy edycji)
                                // Dla nowego umieszczenia, po prostu OccupyingShip != null jest problemem w buforze.
                                bool isPartOfCurrentShipPlacement = false;
                                for(int k=0; k<shipToCheck.Length; ++k) {
                                    if(direction == Direction.Horizontal && checkRow == startRow && checkCol == startCol + k) {
                                        isPartOfCurrentShipPlacement = true;
                                        break;
                                    }
                                    if(direction == Direction.Vertical && checkCol == startCol && checkRow == startRow + k) {
                                        isPartOfCurrentShipPlacement = true;
                                        break;
                                    }
                                }
                                
                                if (!isPartOfCurrentShipPlacement) { // Jeśli to pole buforowe i jest na nim statek
                                    return false;
                                }
                            }
                        }
                    }
                }
                // Sprawdzenie samego pola, na którym ma być segment (czy nie ma tam INNEGO statku)
                if (GetTile(currentSegmentRow, currentSegmentCol).OccupyingShip != null && GetTile(currentSegmentRow, currentSegmentCol).OccupyingShip != shipToCheck) {
                    return false;
                }
            }
            return true;
        }
    }
}