namespace ProgramowanieObiektoweProjekt;
using ProgramowanieObiektoweProjekt.Models.Boards;

class Program
{
    static void Main(string[] args)
    {
        var board = new Board();

        board.DisplayBoard(false);
    }
}