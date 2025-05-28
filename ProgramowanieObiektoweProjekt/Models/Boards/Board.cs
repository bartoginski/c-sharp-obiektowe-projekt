using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces; // Zakładam istnienie IBoard i IShip
using ProgramowanieObiektoweProjekt.Models.Ships;
using Spectre.Console;
using System; // Dodane dla Tuple
using System.Linq; // Dodane dla Enumerable.Range i Select

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class Board : IBoard
    {
        private const int boardSize = 10;
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

        // ZMIENIONE: Implementacja metody PlaceShip
        public void PlaceShip(IShip ship, int x, int y, Direction direction)
        {
            if (direction == Direction.Horizontal)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    tiles[y, x + i].HasShip = true;
                    tiles[y, x + i].ShipReference = ship; // Ustaw referencję do statku
                }
            }
            else // Direction.Vertical
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    tiles[y + i, x].HasShip = true;
                    tiles[y + i, x].ShipReference = ship; // Ustaw referencję do statku
                }
            }
        }

        // DODANE: Metoda TryPlaceShip
        public bool TryPlaceShip(IShip ship, int x, int y, Direction direction)
        {
            // Sprawdzenie, czy statek mieści się na planszy
            if (direction == Direction.Horizontal)
            {
                if (x < 0 || x + ship.Length > boardSize || y < 0 || y >= boardSize)
                {
                    return false;
                }
            }
            else // Direction.Vertical
            {
                if (y < 0 || y + ship.Length > boardSize || x < 0 || x >= boardSize)
                {
                    return false;
                }
            }

            // Sprawdzenie kolizji z innymi statkami i zasadą "jednego pola odstępu"
            // Ta logika jest już częściowo zaimplementowana w KeyControl.IsTileAvaiable,
            // ale dla pewności zaimplementujemy ją tutaj, aby Board sam weryfikował
            // poprawność położenia.

            int startRow = y;
            int endRow = direction == Direction.Vertical ? y + ship.Length - 1 : y;
            int startCol = x;
            int endCol = direction == Direction.Horizontal ? x + ship.Length - 1 : x;

            for (int r = startRow - 1; r <= endRow + 1; r++)
            {
                for (int c = startCol - 1; c <= endCol + 1; c++)
                {
                    if (r >= 0 && r < boardSize && c >= 0 && c < boardSize)
                    {
                        if (tiles[r, c].HasShip)
                        {
                            return false; // Kolizja z istniejącym statkiem lub zbyt blisko
                        }
                    }
                }
            }

            // Jeśli wszystkie sprawdzenia przeszły, to umieść statek
            PlaceShip(ship, x, y, direction);
            return true;
        }


        public ShotResult Shoot(int x, int y)
        {
            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
            {
                return ShotResult.Miss; // Strzał poza planszę
            }

            Tile targetTile = tiles[y, x]; // Używamy y jako indeksu wiersza, x jako kolumny

            if (targetTile.IsHit)
            {
                return ShotResult.Miss; // Już trafiono w to miejsce
            }

            targetTile.IsHit = true;

            if (targetTile.HasShip)
            {
                targetTile.ShipReference.TakeHit(); // Zakładam, że ShipBase ma metodę TakeHit()
                if (targetTile.ShipReference.IsSunk()) // Zakładam, że ShipBase ma metodę IsSunk()
                {
                    return ShotResult.Sunk;
                }
                return ShotResult.Hit;
            }
            return ShotResult.Miss;
        }

        public void DisplayBoard(bool revealShips, KeyControl keyControl = null)
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
                    // Logika wyświetlania stanu pola
                    if (keyControl != null && keyControl.IsShipPreviewTile(i, j) && !keyControl.placementComplete)
                    {
                        rowData[j + 1] = "[yellow]S[/]"; // Podgląd statku podczas rozmieszczania
                    }
                    else if (tiles[i, j].IsHit && tiles[i, j].HasShip)
                    {
                        rowData[j + 1] = "[red]X[/]"; // Trafiony statek
                    }
                    else if (tiles[i, j].IsHit && !tiles[i, j].HasShip)
                    {
                        rowData[j + 1] = "[blue]O[/]"; // Pudło
                    }
                    else if (revealShips && tiles[i, j].HasShip)
                    {
                        rowData[j + 1] = "[green]#[/]"; // Statek (widoczny tylko w trybie revealShips)
                    }
                    else
                    {
                        rowData[j + 1] = " "; // Pusta woda
                    }
                }
                board.AddRow(rowData);
            }
            AnsiConsole.Write(board);
        }
        private bool IsValidPlacement(IShip ship, int startRow, int startCol, Direction direction)
        {
            if (direction == Direction.Horizontal)
            {
                if (startCol + ship.Length - 1 > boardSize) return false;
                for (int i = 0; i < ship.Length; i++)
                {
                    if (tiles[startRow - 1, startCol + i - 1].HasShip)
                    {
                        return false;
                    }
                }
            }
            else // Vertical
            {
                if (startRow + ship.Length - 1 > boardSize) return false;
                for (int i = 0; i < ship.Length; i++)
                {
                    if (tiles[startRow + i - 1, startCol - 1].HasShip)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}