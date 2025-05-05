using ProgramowanieObiektoweProjekt.Models.Boards;
using Spectre.Console;
using System;

internal class BoardLayout
{
   private const int TerminalWidth = 120;  // Ustaw stałą szerokość terminala
   private const int TerminalHeight = 50;  // Ustaw stałą wysokość terminala

    public BoardLayout(Board playerBoard, Board enemyBoard, HistoryTab history)
    {
        // Ustawienie rozmiaru terminala
      Console.SetWindowSize(TerminalWidth, TerminalHeight);

        // Stała wielkość layout
        // Połącz historia + instrukcja
        var rightPanelContent = new Rows(
            new Markup("[bold underline]Historia[/]"),
            history.GetHistoryRenderable(),
            new Markup("\n[bold underline]Instrukcja:[/]\n- Strzelaj: \n- Wygrywasz, gdy zatopisz wszystkie statki przeciwnika\n- Powodzenia! \n- Ilosc mozliwych statkow do rozmieszczenia:")
        );

        // Layout
        var layout = new Layout("Root")
            .SplitColumns(
                new Layout("Boards")
                    .SplitRows(
                        new Layout("PlayerBoard").Size(25),  // Wysokość planszy gracza
                        new Layout("EnemyBoard").Size(25)    // Wysokość planszy przeciwnika
                    ),
                new Layout("InfoPanel")
                    .Update(new Panel(rightPanelContent)
                        .Header("Panel Informacyjny")
                        .Border(BoxBorder.Rounded)
                        .Expand())
            );

        // Uzupełnij plansze
        layout["PlayerBoard"].Update(new Panel(playerBoard.GetBoardRenderable(true))
            .Header("Twoja plansza")
            .Border(BoxBorder.Rounded)
            .Expand());

        layout["EnemyBoard"].Update(new Panel(enemyBoard.GetBoardRenderable(false))
            .Header("Plansza przeciwnika")
            .Border(BoxBorder.Rounded)
            .Expand());

        // Wyświetl
        AnsiConsole.Clear();
        AnsiConsole.Write(layout);
    }

    
}
