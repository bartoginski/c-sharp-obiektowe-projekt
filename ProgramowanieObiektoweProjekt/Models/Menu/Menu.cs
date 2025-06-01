using ProgramowanieObiektoweProjekt.Models.Player;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Utils;
using Spectre.Console;

namespace ProgramowanieObiektoweProjekt.Models.Menu
{
    internal class Menu
    {
        static public void TitleDisplay()
        {
            Console.WriteLine(  "██████╗  █████╗ ████████╗████████╗██╗     ███████╗███████╗██╗  ██╗██╗██████╗\n" +
                                "██╔══██╗██╔══██╗╚══██╔══╝╚══██╔══╝██║     ██╔════╝██╔════╝██║  ██║██║██╔══██╗\n" +
                                "██████╔╝███████║   ██║      ██║   ██║     █████╗  ███████╗███████║██║██████╔╝\n" +
                                "██╔══██╗██╔══██║   ██║      ██║   ██║     ██╔══╝  ╚════██║██╔══██║██║██╔═══╝ \n" +
                                "██████╔╝██║  ██║   ██║      ██║   ███████╗███████╗███████║██║  ██║██║██║ \n" +
                                "╚═════╝ ╚═╝  ╚═╝   ╚═╝      ╚═╝   ╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝╚═╝╚═╝     ");
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
                        "Start game", "Games history", "Autors",
                        "Exit",
                        }
                    ));

            if (choices == "Start game")
            {
                StartGame();
            }
            else if (choices == "Games history")
            {
                GamesHistory();
            }
            else if (choices == "Autors")
            {
                Autors();
            }
            else
            {
                Exit();
            }
        }

        static public void StartGame()
        {
            Console.Clear();
            //Console.OutputEncoding = System.Text.Encoding.UTF8;
            Board playersBoard = new Board();
            Board computersBoard = new Board();
            // Define variable by interface
            IBot bot = new BotEasy();
            RealPlayer Player1 = new RealPlayer("Gracz 1", playersBoard);
            CompPlayer Player2 = new CompPlayer("Gracz 2", computersBoard);
            var keyControl = new KeyControl(playersBoard);

            while (!KeyControl.placementComplete)
            {
                Console.Clear();
                playersBoard.DisplayBoard(true, keyControl);
                keyControl.HandleKeyPress();
            }
            bot.BotShipPlacement(computersBoard);

            playersBoard.DisplayBoard();
            // Show board only when DevMode is on
            computersBoard.DisplayBoard();

            // Tworzenie plansz i historii
            //var playerBoard = new Board();
            var history = new HistoryTab();

            // (opcjonalnie: rozmieszczenie statków itd.)

            // Wyświetlenie layoutu z 2 planszami po lewej i historią po prawej2
            new BoardLayout(playersBoard, computersBoard, history);
        }

        static public void GamesHistory()
        {

        }

        static public void Autors()
        {

        }

        static public void Exit()
        {
            Console.Clear();
            AnsiConsole.WriteLine("Goodbye!");
            AnsiConsole.Write("Press 'Enter' to exit...");
            Console.Read();
            Environment.Exit(0);
        }
    }
}
