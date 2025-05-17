using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;

namespace ProgramowanieObiektoweProjekt.Interfaces
{
    internal interface IPlayer
    {
        int Points { get; }
        string Name { get; }
        Board Board { get; }

        List<(int x, int y, ShotResult result)> MoveHistory { get; }
    }
}
