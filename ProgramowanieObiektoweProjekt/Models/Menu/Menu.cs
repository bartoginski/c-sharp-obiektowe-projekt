﻿using ProgramowanieObiektoweProjekt.Models.Player;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Utils;
using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Interfaces;
using Spectre.Console;

namespace ProgramowanieObiektoweProjekt.Models.Menu
{
    internal class Menu
    {
        // Variables to track player cursor position during shooting
        private static int _playerShotCursorX = 0;
        private static int _playerShotCursorY = 0;
        private static Stats stats = new Stats();

        private static void TitleDisplay()
        {
            Console.WriteLine("  ██████╗  █████╗ ████████╗████████╗██╗      ███████╗███████╗██╗  ██╗██╗██████╗\n" +
                              "  ██╔══██╗██╔══██╗╚══██╔══╝╚══██╔══╝██║      ██╔════╝██╔════╝██║  ██║██║██╔══██╗\n" +
                              "  ██████╔╝███████║   ██║      ██║   ██║      █████╗  ███████╗███████║██║██████╔╝\n" +
                              "  ██╔══██╗██╔══██║   ██║      ██║   ██║      ██╔══╝  ╚════██║██╔══██║██║██╔═══╝ \n" +
                              "  ██████╔╝██║  ██║   ██║      ██║   ███████╗ ███████╗███████║██║  ██║██║██║     \n" +
                              "  ╚═════╝ ╚═╝  ╚═╝   ╚═╝      ╚═╝   ╚══════╝ ╚══════╝╚══════╝╚═╝  ╚═╝╚═╝╚═╝     ");
        }

        public static void MenuDisplay()
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

        // Format coordinates for display
        private static string FormatCoordinate(int col, int row)
        {
            if (col < 0 || col >= Constants.BoardSize || row < 0 || row >= Constants.BoardSize) return "N/A";
            return $"{(char)('A' + row)}{col + 1}";
        }
        
        public static void StartGame()
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

            // Create bot based on difficulty selection
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

            // Ship placement phase
            var keyControl = new KeyControl(playersBoard);
            KeyControl.PlacementComplete = false;
            KeyControl.CurrentShipIndexForPlacement = 0;

            while (!KeyControl.PlacementComplete)
            {
                Console.Clear();
                Console.Write("\x1b[2J\x1b[H"); // Clear lines with color text
                if (KeyControl.CurrentShipIndexForPlacement < playersBoard.Ships.Count)
                {
                    AnsiConsole.MarkupLine($"[bold]Umieść statek: {playersBoard.Ships[KeyControl.CurrentShipIndexForPlacement].Name} (Długość: {playersBoard.Ships[KeyControl.CurrentShipIndexForPlacement].Length})[/]");
                    AnsiConsole.MarkupLine($"Pozycja: {FormatCoordinate(keyControl.GetCurrentX(), keyControl.GetCurrentY())}, Orientacja: {(playersBoard.Ships[KeyControl.CurrentShipIndexForPlacement].IsHorizontal ? "Pozioma" : "Pionowa")}");
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

            // Bot places ships automatically
            bot.BotShipPlacement(computersBoard);
            AnsiConsole.MarkupLine("[green]Statki komputera rozmieszczone![/]");
            Thread.Sleep(1500);

            // Main game loop
            var history = new HistoryTab();
            bool playerTurn = true;
            _playerShotCursorX = 0;
            _playerShotCursorY = 0;
            bool gameRunning = true;

            while (gameRunning)
            {
                Console.Clear();
                Console.Write("\x1b[2J\x1b[H"); // Clear lines with color text
                new BoardLayout(playersBoard, computersBoard, history, playerTurn, _playerShotCursorX, _playerShotCursorY);

                if (playerTurn)
                {
                    AnsiConsole.MarkupLine($"\n[bold steelblue]Tura gracza: {player1.Name}[/]");
                    AnsiConsole.MarkupLine($"Wybierz pole strzałkami (Cel: {FormatCoordinate(_playerShotCursorX, _playerShotCursorY)}). Enter by strzelić.");

                    ConsoleKeyInfo key = Console.ReadKey(true);
                    
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            if (_playerShotCursorX > 0) _playerShotCursorX--;
                            break;
                        case ConsoleKey.RightArrow:
                            if (_playerShotCursorX < Constants.BoardSize - 1) _playerShotCursorX++;
                            break;
                        case ConsoleKey.UpArrow:
                            if (_playerShotCursorY > 0) _playerShotCursorY--;
                            break;
                        case ConsoleKey.DownArrow:
                            if (_playerShotCursorY < Constants.BoardSize - 1) _playerShotCursorY++;
                            break;
                        case ConsoleKey.Enter:
                            Tile targetTile = computersBoard.GetTile(_playerShotCursorY, _playerShotCursorX);
                            if (targetTile.IsHit)
                            {
                                AnsiConsole.MarkupLine("[yellow]To pole już zostało ostrzelane. Wybierz inne.[/]");
                                Thread.Sleep(1200);
                            }
                            else
                            {
                                var (shotCol, shotRow) = (playerShotCursorX: _playerShotCursorX, playerShotCursorY: _playerShotCursorY);
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
                                            stats.saveStats("Wygrana", history.ShotsFired, history.Hits, history.Misses);
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
                else // Bot turn
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
                                stats.saveStats("Przegrana", history.ShotsFired, history.Hits, history.Misses);
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

            // Return to menu after pressing Enter
            AnsiConsole.MarkupLine("\n[bold]Koniec gry! Naciśnij Enter, aby wrócić do menu głównego...[/]");
            Console.ReadLine();
        }

        public static void GamesHistory()
        {
            Console.Clear();
            stats.openStats();
            Console.WriteLine("Wciśnij klawisz aby kontynuować...");
            Console.ReadLine();
        }

        static public void Autors()
        {
                        Console.Clear();
            // Added ASCII logo
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
            
            Console.WriteLine(); // Extra spacing line
            AnsiConsole.MarkupLine("- CEO [green]Kamil Muc[/]");
            AnsiConsole.MarkupLine("- Lead Developer [green]Bartosz Ogiński[/]");
            AnsiConsole.MarkupLine("- Główny Księgowy [green]Jan Mrozewski[/]");
            AnsiConsole.MarkupLine("- Specjalista ds. Marketingu [green]Bartosz Nowak[/]");
            AnsiConsole.MarkupLine("- Project Manager [green]Patryk Parzyński[/]");

            AnsiConsole.MarkupLine("\nNaciśnij Enter, aby wrócić do menu głównego...");
            Console.ReadLine();

        }

        private static void Exit()
        {
            Console.Clear();
            AnsiConsole.WriteLine("Goodbye!");
            AnsiConsole.Write("Press 'Enter' to exit...");
            Console.ReadKey(); 
            Environment.Exit(0);
        }
    }
}
