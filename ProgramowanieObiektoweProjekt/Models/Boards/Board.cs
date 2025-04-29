using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using Spectre.Console;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class Board : IBoard
    {
        // this should represent one tile
        // class on the bottom
        // private Tile[,] tiles;
        private List<IShip> ships;

        public void PlaceShip(IShip ship, int x, int y, Direction direction)
        {
            // code...
        }

        public ShotResult Shoot(int x, int y)
        {
            return ShotResult.Miss;
        }

        public void DisplayBoard(bool revealShips)
        {

            const int boardSize = 10;
            string[] columnHeaders = Enumerable.Range(1, boardSize)
                .Select(i => i.ToString())
                .ToArray();
            string[] rowHeaders = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];

            var matrix = new int[boardSize, boardSize];

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
                    if (matrix[i, j] == 0)
                    {
                        rowData[j + 1] = "~";
                    }
                }

                board.AddRow(rowData);
            }

            AnsiConsole.Write(board);
        }
    }


    

    //class Tile
    //{
    //    public bool HasSHip { get; set; }
    //    public bool IsHit { get; set; }
    //}
}
