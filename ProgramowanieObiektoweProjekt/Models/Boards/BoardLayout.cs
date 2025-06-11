using ProgramowanieObiektoweProjekt.Models.Boards;
using Spectre.Console;
using System;
using ProgramowanieObiektoweProjekt.Utils; // Dla Constants

internal class BoardLayout
{
    private const int TerminalWidth = 120;  // Ustaw stałą szerokość terminala
    private const int TerminalHeight = 50;  // Ustaw stałą wysokość terminala

    // Zmodyfikowany konstruktor
    public BoardLayout(Board playerBoard, Board enemyBoard, HistoryTab history, bool isPlayerShootingTurn, int cursorCol, int cursorRow)
    {
        // Ustawienie rozmiaru terminala - można rozważyć wywołanie tego rzadziej, np. raz na starcie gry
         if (Console.WindowWidth != TerminalWidth || Console.WindowHeight != TerminalHeight)
         {
             try { Console.SetWindowSize(TerminalWidth, TerminalHeight); } catch (Exception) { /* Ignoruj błędy, jeśli nie można ustawić */ }
         }


        var rightPanelContent = new Rows(
            new Markup("[bold underline]Historia[/]"),
            history.GetHistoryRenderable(),
            new Markup("\n[bold underline]Instrukcja:[/]\n- Strzałki: poruszanie kursorem/statkiem\n- Enter: strzał/postawienie statku\n- Spacja: obrót statku (podczas rozmieszczania)\n- Wygrywasz, gdy zatopisz wszystkie statki przeciwnika\n- Powodzenia! \n- Ilosc mozliwych statkow do rozmieszczenia:")
        );

        var layout = new Layout("Root")
            .SplitColumns(
                new Layout("Boards")
                    .SplitRows(
                        new Layout("PlayerBoard").Size(25),
                        new Layout("EnemyBoard").Size(25)
                    ),
                new Layout("InfoPanel")
                    .Update(new Panel(rightPanelContent)
                        .Header("Panel Informacyjny")
                        .Border(BoxBorder.Rounded)
                        .Expand())
            );

        layout["PlayerBoard"].Update(new Panel(playerBoard.GetBoardRenderable(true, KeyControl.placementComplete ? null : new KeyControl(playerBoard))) // Przekazuj KeyControl tylko jeśli jest potrzebny
            .Header("Twoja plansza")
            .Border(BoxBorder.Rounded)
            .Expand());

        // Przekaż parametry kursora do GetBoardRenderable dla planszy przeciwnika
        layout["EnemyBoard"].Update(new Panel(enemyBoard.GetBoardRenderable(Constants.DevMode, null, isPlayerShootingTurn, cursorCol, cursorRow))
            .Header("Plansza przeciwnika")
            .Border(BoxBorder.Rounded)
            .Expand());

        // AnsiConsole.Clear(); // Wyczyszczenie konsoli jest teraz w pętli StartGame
        AnsiConsole.Write(layout);
    }
}