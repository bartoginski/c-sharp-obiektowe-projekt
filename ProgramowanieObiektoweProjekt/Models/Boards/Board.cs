using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Ships;
using Spectre.Console;
using System;
using System.Linq;

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

        public void PlaceShip(IShip ship, int x, int y, Direction direction)
        {
            if (direction == Direction.Horizontal)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    tiles[y, x + i].HasShip = true;
                    tiles[y, x + i].ShipReference = ship;
                }
            }
            else
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    tiles[y + i, x].HasShip = true;
                    tiles[y + i, x].ShipReference = ship;
                }
            }
        }

        public bool TryPlaceShip(IShip ship, int x, int y, Direction direction)
        {
            if (direction == Direction.Horizontal)
            {
                if (x < 0 || x + ship.Length > boardSize || y < 0 || y >= boardSize)
                {
                    return false;
                }
            }
            else
            {
                if (y < 0 || y + ship.Length > boardSize || x < 0 || x >= boardSize)
                {
                    return false;
                }
            }

            
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
                            return false;
                        }
                    }
                }
            }

            PlaceShip(ship, x, y, direction);
            return true;
        }


        public ShotResult Shoot(int x, int y)
        {
            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
            {
                return ShotResult.Miss;
            }

            Tile targetTile = tiles[y, x];

            if (targetTile.IsHit)
            {
                return ShotResult.Miss;
            }

            targetTile.IsHit = true;

            if (targetTile.HasShip)
            {
                targetTile.ShipReference.TakeHit();
                if (targetTile.ShipReference.IsSunk())
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
                    if (keyControl != null && keyControl.IsShipPreviewTile(i, j) && !keyControl.placementComplete)
                    {
                        rowData[j + 1] = "[yellow]S[/]";
                    }
                    else if (tiles[i, j].IsHit && tiles[i, j].HasShip)
                    {
                        rowData[j + 1] = "[red]X[/]";
                    }
                    else if (tiles[i, j].IsHit && !tiles[i, j].HasShip)
                    {
                        rowData[j + 1] = "[blue]O[/]";
                    }
                    else if (revealShips && tiles[i, j].HasShip)
                    {
                        rowData[j + 1] = "[green]#[/]";
                    }
                    else
                    {
                        rowData[j + 1] = " ";
                    }
                }
                board.AddRow(rowData);
            }
            AnsiConsole.Write(board);
        }
    }
}