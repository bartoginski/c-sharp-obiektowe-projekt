﻿using ProgramowanieObiektoweProjekt.Models.Boards;
using Spectre.Console;
using ProgramowanieObiektoweProjekt.Utils;
using ProgramowanieObiektoweProjekt.Models.Ships;

internal class BoardLayout
{
    private const int TerminalWidth = 120;  // Set constant terminal width
    private const int TerminalHeight = 50;  // Set constant terminal height

    public BoardLayout(Board playerBoard, Board enemyBoard, HistoryTab history, bool isPlayerShootingTurn, int cursorCol, int cursorRow)
    {
        // Set terminal size - consider calling this less frequently, e.g. once at game start
         if (Console.WindowWidth != TerminalWidth || Console.WindowHeight != TerminalHeight)
         {
             try { Console.SetWindowSize(TerminalWidth, TerminalHeight); } catch (Exception) { /* Ignore errors if can't set size */ }
         }


        var rightPanelContent = new Rows(
            new Markup("[bold underline]Historia[/]"),
            history.GetHistoryRenderable(),
            new Markup("\n[bold underline]Instrukcja:[/]\n" +
                       "- Strzałki: poruszanie kursorem/statkiem\n" +
                       "- Enter: strzał/postawienie statku\n" +
                       "- Spacja: obrót statku (podczas rozmieszczania)\n" +
                       "- Wygrywasz, gdy zatopisz wszystkie statki przeciwnika\n" +
                       "- Powodzenia! \n\n\n\n" +
                       "Statki przeciwnika: \n\n" +
                       $"BattleShip: {enemyBoard.Ships.OfType<BattleShip>().Count(ship => !ship.IsSunk)}\n" +
                       $"Cruiser:    {enemyBoard.Ships.OfType<Cruiser>().Count(ship => !ship.IsSunk)}\n" +
                       $"Destroyer:  {enemyBoard.Ships.OfType<Destroyer>().Count(ship => !ship.IsSunk)}\n" +
                       $"Submarine:  {enemyBoard.Ships.OfType<Submarine>().Count(ship => !ship.IsSunk)}\n\n\n\n" +
                       "Twoje statki: \n\n" +
                       $"BattleShip: {playerBoard.Ships.OfType<BattleShip>().Count(ship => !ship.IsSunk)}\n" +
                       $"Cruiser:    {playerBoard.Ships.OfType<Cruiser>().Count(ship => !ship.IsSunk)}\n" +
                       $"Destroyer:  {playerBoard.Ships.OfType<Destroyer>().Count(ship => !ship.IsSunk)}\n" +
                       $"Submarine:  {playerBoard.Ships.OfType<Submarine>().Count(ship => !ship.IsSunk)}\n\n")
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

        // Pass KeyControl only when needed for ship placement
        layout["PlayerBoard"].Update(new Panel(playerBoard.GetBoardRenderable(true, KeyControl.PlacementComplete ? null : new KeyControl(playerBoard)))
            .Header("Twoja plansza")
            .Border(BoxBorder.Rounded)
            .Expand());

        // Pass cursor parameters to enemy board for shooting mode
        layout["EnemyBoard"].Update(new Panel(enemyBoard.GetBoardRenderable(Constants.DevMode, null, isPlayerShootingTurn, cursorCol, cursorRow))
            .Header("Plansza przeciwnika")
            .Border(BoxBorder.Rounded)
            .Expand());

        // Console clearing is now handled in StartGame loop
        AnsiConsole.Write(layout);
    }
}