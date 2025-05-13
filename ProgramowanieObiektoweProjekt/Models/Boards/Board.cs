using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Ships;
using Spectre.Console;

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
        
        
        public Tile GetTile(int x, int y)
        {
            return tiles[x, y];
        }

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
                    // Check if tile exists and has ship
                    if (tiles[i, j]?.HasShip != null && tiles[i, j].HasShip)
                    {
                        rowData[j + 1] = "^";
                    }
                    else
                    {
                        rowData[j + 1] = "~";
                    }
                }

                board.AddRow(rowData);
            }

            AnsiConsole.Write(board);
        }
    }
}
