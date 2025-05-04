using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class Board : IBoard
    {
        private const int boardSize = 10;
        private readonly Tile[,] tiles = new Tile[boardSize, boardSize];
        private readonly List<IShip> ships;

        public Board()
        {
            ships = new List<IShip>();
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    tiles[i, j] = new Tile();
                }
            }
        }

        public void PlaceShip(IShip ship, int x, int y, Direction direction)
        {
            // TODO: Implement placement
        }

        public ShotResult Shoot(int x, int y)
        {
            // TODO: Implement shooting logic
            return ShotResult.Miss;
        }

        public void DisplayBoard(bool revealShips)
        {
            AnsiConsole.Write(GetBoardRenderable(revealShips));
        }

        public Table GetBoardRenderable(bool revealShips)
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
                    if (revealShips)
                        rowData[j + 1] = tiles[i, j].HasShip ? "[green]^[/]" : "~";
                    else
                        rowData[j + 1] = "~";
                }

                board.AddRow(rowData);
            }

            return board;
        }
    }
}
