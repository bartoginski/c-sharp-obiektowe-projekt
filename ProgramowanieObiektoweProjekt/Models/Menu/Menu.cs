using ProgramowanieObiektoweProjekt.Models.Player;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Utils;
using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Interfaces;
using Spectre.Console;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace ProgramowanieObiektoweProjekt.Models.Menu
{
    internal class Menu
    {
        // Zmienne do śledzenia pozycji kursora gracza podczas strzelania
        static private int playerShotCursorX = 0;
        static private int playerShotCursorY = 0;

        static public void TitleDisplay()
        {
            Console.WriteLine("  ██████╗  █████╗ ████████╗████████╗██╗      ███████╗███████╗██╗  ██╗██╗██████╗\n" +
                              "  ██╔══██╗██╔══██╗╚══██╔══╝╚══██╔══╝██║      ██╔════╝██╔════╝██║  ██║██║██╔══██╗\n" +
                              "  ██████╔╝███████║   ██║      ██║   ██║      █████╗  ███████╗███████║██║██████╔╝\n" +
                              "  ██╔══██╗██╔══██║   ██║      ██║   ██║      ██╔══╝  ╚════██║██╔══██║██║██╔═══╝ \n" +
                              "  ██████╔╝██║  ██║   ██║      ██║   ███████╗ ███████╗███████║██║  ██║██║██║     \n" +
                              "  ╚═════╝ ╚═╝  ╚═╝   ╚═╝      ╚═╝   ╚══════╝ ╚══════╝╚══════╝╚═╝  ╚═╝╚═╝╚═╝     ");
        }

        static public void MenuDisplay()
        {
            TitleDisplay();
            var choices = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .HighlightStyle(new Style(
                        background: Color.White,
                        foreground: Color.Black))
                    .AddChoices(new[] {
                        "Nowa gra", "Historia gier", "Autorzy",
                        "Wyjście",
                        }
                    ));

            switch (choices)
            {
                case "Nowa gra":
                    StartGame();
                    break;
                case "Historia gier":
                    GamesHistory();
                    break;
                case "Autorzy":
                    Autors();
                    break;
                case "Wyjście":
                    Exit();
                    break;
            }
        }

        private static string FormatCoordinate(int col, int row) // col_idx_0, row_idx_0
        {
            if (col < 0 || col >= Constants.BoardSize || row < 0 || row >= Constants.BoardSize) return "N/A";
            return $"{(char)('A' + row)}{col + 1}";
        }
        
        static public void StartGame()
        {
            Console.Clear();
            Board playersBoard = new Board();
            Board computersBoard = new Board();
            var botDifficulty = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Select bot difficulty:[/]")
                    .PageSize(3)
                    .HighlightStyle(new Style(
                        background: Color.White,
                        foreground: Color.Black))
                    .AddChoices(new[] { "Easy", "Medium", "Hard" })
            );

            IBot bot;
            switch (botDifficulty)
            {
                case "Medium":
                    bot = new BotMedium();
                    break;
                case "Hard":
                    bot = new BotHard();
                    break;
                default:
                    bot = new BotEasy();
                    break;
            }

            IPlayer player1 = new RealPlayer("Gracz 1", playersBoard);

            AnsiConsole.MarkupLine("[bold underline]Rozmieszczanie statków przez gracza:[/]");
            AnsiConsole.MarkupLine("Użyj strzałek do poruszania, Spacji do obracania, Enter do umieszczenia statku.");

            var keyControl = new KeyControl(playersBoard);
            KeyControl.placementComplete = false;
            KeyControl.currentShipIndexForPlacement = 0;

            while (!KeyControl.placementComplete)
            {
                Console.Clear();
                if (KeyControl.currentShipIndexForPlacement < playersBoard.ships.Count)
                {
                    AnsiConsole.MarkupLine($"[bold]Umieść statek: {playersBoard.ships[KeyControl.currentShipIndexForPlacement].Name} (Długość: {playersBoard.ships[KeyControl.currentShipIndexForPlacement].Length})[/]");
                    AnsiConsole.MarkupLine($"Pozycja: {FormatCoordinate(keyControl.GetCurrentX(), keyControl.GetCurrentY())}, Orientacja: {(playersBoard.ships[KeyControl.currentShipIndexForPlacement].IsHorizontal ? "Pozioma" : "Pionowa")}");
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold]Wszystkie statki rozmieszczone przez gracza![/]");
                }
                AnsiConsole.MarkupLine("Strzałki - ruch, Spacja - obrót, Enter - postaw, Esc - zakończ rozmieszczanie");
                playersBoard.DisplayBoard(true, keyControl);
                keyControl.HandleKeyPress();
            }

            AnsiConsole.MarkupLine("\n[green]Statki gracza rozmieszczone![/]");

            bot.BotShipPlacement(computersBoard);
            AnsiConsole.MarkupLine("[green]Statki komputera rozmieszczone![/]");
            Thread.Sleep(1500);

            var history = new HistoryTab();
            bool playerTurn = true;
            playerShotCursorX = 0;
            playerShotCursorY = 0;
            bool gameRunning = true;

            while (gameRunning)
            {
                Console.Clear();
                new BoardLayout(playersBoard, computersBoard, history, playerTurn, playerShotCursorX, playerShotCursorY);

                if (playerTurn)
                {
                    AnsiConsole.MarkupLine($"\n[bold steelblue]Tura gracza: {player1.Name}[/]");
                    AnsiConsole.MarkupLine($"Wybierz pole strzałkami (Cel: {FormatCoordinate(playerShotCursorX, playerShotCursorY)}). Enter by strzelić.");

                    ConsoleKeyInfo key = Console.ReadKey(true);
                    
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            if (playerShotCursorX > 0) playerShotCursorX--;
                            break;
                        case ConsoleKey.RightArrow:
                            if (playerShotCursorX < Constants.BoardSize - 1) playerShotCursorX++;
                            break;
                        case ConsoleKey.UpArrow:
                            if (playerShotCursorY > 0) playerShotCursorY--;
                            break;
                        case ConsoleKey.DownArrow:
                            if (playerShotCursorY < Constants.BoardSize - 1) playerShotCursorY++;
                            break;
                        case ConsoleKey.Enter:
                            Tile targetTile = computersBoard.GetTile(playerShotCursorY, playerShotCursorX);
                            if (targetTile.IsHit)
                            {
                                AnsiConsole.MarkupLine("[yellow]To pole już zostało ostrzelane. Wybierz inne.[/]");
                                Thread.Sleep(1200);
                            }
                            else
                            {
                                var (shotCol, shotRow) = (playerShotCursorX, playerShotCursorY);
                                ShotResult result = computersBoard.Shoot(shotCol, shotRow);
                                string formattedCoords = FormatCoordinate(shotCol, shotRow);

                                AnsiConsole.Markup($"Strzał w {formattedCoords}: ");
                                switch (result)
                                {
                                    case ShotResult.Hit:
                                        AnsiConsole.MarkupLine("[orange1]TRAFIONY![/]");
                                        history.RecordShot(true);
                                        break;
                                    case ShotResult.Sunk:
                                        AnsiConsole.MarkupLine("[red bold]TRAFIONY ZATOPIONY![/]");
                                        ShipBase sunkShipPlayerTarget = computersBoard.GetTile(shotRow, shotCol).OccupyingShip!;
                                        AnsiConsole.MarkupLine($"[red]Statek typu {sunkShipPlayerTarget.Name} został zatopiony![/]");
                                        computersBoard.MarkAroundSunkShip(sunkShipPlayerTarget);
                                        history.RecordShot(true);
                                        if (computersBoard.AreAllShipsSunk())
                                        {
                                            Console.Clear();
                                            new BoardLayout(playersBoard, computersBoard, history, false, -1, -1);
                                            AnsiConsole.MarkupLine($"\n[bold greenyellow]GRATULACJE, {player1.Name}! Wygrałeś, zatapiając wszystkie statki przeciwnika![/]");
                                            gameRunning = false;
                                        }
                                        break;
                                    case ShotResult.Miss:
                                        AnsiConsole.MarkupLine("[dodgerblue1]PUDŁO![/]");
                                        history.RecordShot(false);
                                        break;
                                }

                                if (gameRunning)
                                {
                                    if (result == ShotResult.Miss)
                                    {
                                        playerTurn = false;
                                        AnsiConsole.MarkupLine("\nNaciśnij Enter, aby rozpocząć turę komputera...");
                                        Console.ReadLine();
                                    }
                                    else
                                    {
                                        AnsiConsole.MarkupLine("\n[bold green]Trafienie! Masz kolejny strzał. Naciśnij Enter, aby kontynuować...[/]");
                                        Console.ReadLine();
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else // Tura komputera
                {
                    AnsiConsole.MarkupLine($"\n[bold indianred]Tura komputera: {bot.Name}[/]");
                    Thread.Sleep(1200);
                    var (botCol, botRow) = bot.BotShotSelection();
                    ShotResult result = playersBoard.Shoot(botCol, botRow);
                    bot.InformShotResult(Tuple.Create(botCol, botRow), result);
                    string formattedCoords = FormatCoordinate(botCol, botRow);

                    AnsiConsole.Markup($"Komputer strzela w {formattedCoords}: ");
                    switch (result)
                    {
                        case ShotResult.Hit:
                            AnsiConsole.MarkupLine("[orange1]TRAFIONY! (Twój statek)[/]");
                            break;
                        case ShotResult.Sunk:
                            AnsiConsole.MarkupLine("[red bold]TRAFIONY ZATOPIONY! (Twój statek)[/]");
                            ShipBase sunkShipBotTarget = playersBoard.GetTile(botRow, botCol).OccupyingShip!;
                            AnsiConsole.MarkupLine($"[red]Twój statek typu {sunkShipBotTarget.Name} został zatopiony przez komputer![/]");
                            List<(int col, int row)> markedCells = playersBoard.MarkAroundSunkShip(sunkShipBotTarget);
                            bot.AddCellsToAvoid(markedCells);
                            if (playersBoard.AreAllShipsSunk())
                            {
                                Console.Clear();
                                new BoardLayout(playersBoard, computersBoard, history, false, -1, -1);
                                AnsiConsole.MarkupLine($"\n[bold red1]NIESTETY, {bot.Name} zatopił wszystkie Twoje statki. Przegrałeś.[/]");
                                gameRunning = false;
                            }
                            break;
                        case ShotResult.Miss:
                            AnsiConsole.MarkupLine("[dodgerblue1]PUDŁO![/]");
                            break;
                    }

                    if (gameRunning)
                    {
                        if (result == ShotResult.Miss)
                        {
                            playerTurn = true;
                            AnsiConsole.MarkupLine("\nNaciśnij Enter, aby rozpocząć swoją turę...");
                            Console.ReadLine();
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("\n[bold red]Komputer trafił! Będzie strzelał ponownie.[/]");
                            Thread.Sleep(1800);
                        }
                    }
                }
            } 

            // ZMIANA: Powrót do menu po wciśnięciu Enter
            AnsiConsole.MarkupLine("\n[bold]Koniec gry! Naciśnij Enter, aby wrócić do menu głównego...[/]");
            Console.ReadLine();
        }

        static public void GamesHistory()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow]Funkcja historii gier nie została jeszcze zaimplementowana.[/]");
            // ZMIANA: Powrót do menu po wciśnięciu Enter
            AnsiConsole.MarkupLine("\nNaciśnij Enter, aby wrócić do menu głównego...");
            Console.ReadLine();
        }

        static public void Autors()
        {
                        Console.Clear();
            // ZMIANA: Dodano logo ASCII
            AnsiConsole.Write(new Markup(
@" [bold cyan]
 ██████╗  █████╗ ██████╗  █████╗  ██████╗ ███████╗                           
██╔════╝ ██╔══██╗██╔══██╗██╔══██╗██╔════╝ ██╔════╝                           
██║  ███╗███████║██████╔╝███████║██║  ███╗█████╗                             
██║   ██║██╔══██║██╔══██╗██╔══██║██║   ██║██╔══╝                             
╚██████╔╝██║  ██║██║  ██║██║  ██║╚██████╔╝███████╗                           
 ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝                           
                                                                             
██████╗ ███████╗██╗   ██╗███████╗██╗      ██████╗ ██████╗ ███╗   ███╗███████╗███╗   ██╗████████╗  
██╔══██╗██╔════╝██║   ██║██╔════╝██║     ██╔═══██╗██╔══██╗████╗ ████║██╔════╝████╗  ██║╚══██╔══╝ 
██║  ██║█████╗  ██║   ██║█████╗  ██║     ██║   ██║██████╔╝██╔████╔██║█████╗  ██╔██╗ ██║   ██║ 
██║  ██║██╔══╝  ╚██╗ ██╔╝██╔══╝  ██║     ██║   ██║██╔═══╝ ██║╚██╔╝██║██╔══╝  ██║╚██╗██║   ██║  
██████╔╝███████╗ ╚████╔╝ ███████╗███████╗╚██████╔╝██║     ██║ ╚═╝ ██║███████╗██║ ╚████║   ██║   
╚═════╝ ╚══════╝  ╚═══╝  ╚══════╝╚══════╝ ╚═════╝ ╚═╝     ╚═╝     ╚═╝╚══════╝╚═╝  ╚═══╝   ╚═╝[/] 
                                                                             
                                                         
"
            ));
            
            Console.WriteLine(); // Dodatkowa linia odstępu
            AnsiConsole.MarkupLine("- CEO [green]Kamil Muc[/]");
            AnsiConsole.MarkupLine("- Lead Developer [green]Bartosz Ogiński[/]");
            AnsiConsole.MarkupLine("- Główny Księgowy [green]Jan Mrozewski[/]");
            AnsiConsole.MarkupLine("- Specjalista ds. Marketingu [green]Bartosz Nowak[/]");
            AnsiConsole.MarkupLine("- Project Manager [green]Patryk Parzyński[/]");

            AnsiConsole.MarkupLine("\nNaciśnij Enter, aby wrócić do menu głównego...");
            Console.ReadLine();

        }

        static public void Exit()
        {
            Console.Clear();
            AnsiConsole.WriteLine("Goodbye!");
            AnsiConsole.Write("Press 'Enter' to exit...");
            Console.ReadKey(); 
            Environment.Exit(0);
        }
    }
}