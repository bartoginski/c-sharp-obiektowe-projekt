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
        while (true)
        {
            board.DisplayBoard(true);
            keyControl.HandleKeyPress();

            // Sprawdź, czy ustawianie statków zostało zakończone
            if (KeyControl.placementComplete) // Upewnij się, że pole jest publiczne lub dodaj właściwość
                break;
        }

        // Wyświetl planszę po ustawieniu statków
        board.DisplayBoard(false);
    }
}