namespace ProgramowanieObiektoweProjekt;
using ProgramowanieObiektoweProjekt.Models.Boards;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var board = new Board();
        var keyControl = new KeyControl(board);

        // Ustawianie statków
        while (!KeyControl.placementComplete)
        {
            // Console.Clear();
            board.DisplayBoard(true, keyControl);
            keyControl.HandleKeyPress();
        }

        // Wyświetl planszę po ustawieniu statków
        board.DisplayBoard(false);
    }
}