namespace ProgramowanieObiektoweProjekt;

using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        const int boardSize = 10;
        
        // TODO: replace it later
        AnsiConsole.Markup("[underline red]Battleships[/] Game!\n");

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
                    rowData[j + 1] = "🌊";
                }
            }

            board.AddRow(rowData);
        }

        AnsiConsole.Write(board);
    }
}