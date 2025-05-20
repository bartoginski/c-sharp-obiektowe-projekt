using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Models.Menu;

namespace ProgramowanieObiektoweProjekt
{
    class Program
    {
        static void Main(string[] args)
        {

            //// Tworzenie plansz i historii
            //var playerBoard = new Board();
            //var enemyBoard = new Board();
            //var history = new HistoryTab();

            //// (opcjonalnie: rozmieszczenie statków itd.)

            //// Wyświetlenie layoutu z 2 planszami po lewej i historią po prawej2
            //new BoardLayout(playerBoard, enemyBoard, history);

            Menu.MenuDisplay();
        }
    }
}