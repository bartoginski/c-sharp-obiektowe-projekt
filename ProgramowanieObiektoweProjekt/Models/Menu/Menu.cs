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
            var choices = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .HighlightStyle(new Style(
                        background: Color.White,
                        foreground: Color.Black))
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
            else if (choices == "Autors")
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
