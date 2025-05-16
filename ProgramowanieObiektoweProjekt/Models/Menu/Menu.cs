using Spectre.Console;
namespace ProgramowanieObiektoweProjekt.Models.Menu
{
    internal class Menu
    {
        //1. Start game
        //2. Games history
        //3. Autors
        //4. Exit
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
            var choices = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices(new[] {
                        "Start game", "Games history", "Autors",
                        "Exit",
        }));

            if (choices == "Start game")
            {
                AnsiConsole.WriteLine("You chosed Start game");
            }
            else if (choices == "Games history")
            {
                AnsiConsole.WriteLine("You chosed Games history");
            }
            else if (choices == "Games history")
            {
                AnsiConsole.WriteLine("You chosed Autors");
            }
            else
            {
                AnsiConsole.WriteLine("You chosed Exit");
            }
        }
    }
}
