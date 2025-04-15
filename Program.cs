using System;
using Spectre.Console;
using System.Collections.Generic;
using Spectre.Console.Rendering;

public class BattleshipBoardSpectre
{
    public static void Main(string[] args)
    {
        // Tworzenie głównego układu
        var mainLayout = new Layout("Root").SplitColumns(
            new Layout("Sidebar").Size(30), // 👈 Lewy panel - węższy
            new Layout("Boards")
                .SplitRows(
                    new Layout("EnemyBoardPanel"),
                    new Layout("PlayerBoardPanel")
                )
        );

        // Lewy panel - krótkie komunikaty
        mainLayout["Sidebar"].Update(
            new Panel(
                Align.Center(
                    new Markup("[green]Gra rozpoczęta![/]\n[blue]Rozmieść swoje statki.[/]"),
                    VerticalAlignment.Middle))
                .BorderColor(Color.Grey)
                .Border(BoxBorder.Double)
                .Expand()
        );

        // Górny panel - plansza przeciwnika (czerwona ramka)
        var enemyBoardTable = CreateGameBoard();
        var enemyBoardPanel = new Panel(enemyBoardTable)
            .Border(BoxBorder.Square)
            .BorderColor(Color.Red)
            .Header("[bold green]Plansza przeciwnika[/]")
            .Expand();
        mainLayout["Boards"]["EnemyBoardPanel"].Update(enemyBoardPanel);

        // Dolny panel - plansza gracza (niebieska ramka)
        var playerBoardTable = CreateGameBoard();
        var playerBoardPanel = new Panel(playerBoardTable)
            .Border(BoxBorder.Square)
            .BorderColor(Color.Blue)
            .Header("[bold yellow]Twoja plansza[/]")
            .Expand();
        mainLayout["Boards"]["PlayerBoardPanel"].Update(playerBoardPanel);

        // Wyświetlenie layoutu
        AnsiConsole.Write(mainLayout);
    }

    // Tworzy kolorową planszę 10x10 z nagłówkami
    private static Table CreateGameBoard()
    {
        var boardTable = new Table();
        boardTable.Border = TableBorder.Rounded;
        boardTable.Expand();

        // Nagłówki kolumn: pusta + A-J
        boardTable.AddColumn(new TableColumn(new Markup("[grey]#[/]"))); // Kolumna numerów
        for (int i = 0; i < 10; i++)
        {
            string columnLetter = ((char)('A' + i)).ToString();
            boardTable.AddColumn(new TableColumn(new Markup($"[grey]{columnLetter}[/]")));
        }

        // Dodaj wiersze z numeracją + kolorowe pola
        for (int row = 0; row < 10; row++)
        {
            var rowData = new List<IRenderable>
            {
                new Markup($"[grey]{row + 1}[/]") // numer wiersza
            };

            for (int col = 0; col < 10; col++)
            {
                string cellColor = GetCellColor(row, col);
                rowData.Add(new Markup($"[{cellColor}]■[/]"));
            }

            boardTable.AddRow(rowData.ToArray());
        }

        return boardTable;
    }

    // Kolorowanie pól – szachownica
    private static string GetCellColor(int row, int col)
    {
        return (row + col) % 2 == 0 ? "deepskyblue1" : "blue";
    }
}
