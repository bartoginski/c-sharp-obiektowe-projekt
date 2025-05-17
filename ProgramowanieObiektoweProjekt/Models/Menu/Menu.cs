using ProgramowanieObiektoweProjekt.Models.Player;
using ProgramowanieObiektoweProjekt.Models.Boards;
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
            RealPlayer Player1 = new RealPlayer("Gracz 1", playersBoard);
            var keyControl = new KeyControl(playersBoard);

            while (!KeyControl.placementComplete)
            {
                Console.Clear();
                playersBoard.DisplayBoard(true, keyControl);
                keyControl.HandleKeyPress();
            }
            playersBoard.DisplayBoard(false);

            // Tworzenie plansz i historii
            //var playerBoard = new Board();
            var enemyBoard = new Board();
            var history = new HistoryTab();

            // (opcjonalnie: rozmieszczenie statków itd.)

            // Wyświetlenie layoutu z 2 planszami po lewej i historią po prawej2
            new BoardLayout(playersBoard, enemyBoard, history);
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
