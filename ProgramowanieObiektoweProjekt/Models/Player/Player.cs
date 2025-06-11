using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Boards;

namespace ProgramowanieObiektoweProjekt.Models.Player
{
    internal class RealPlayer(string name, Board board) : IPlayer
    {
        public int Points { get; } = 0;

        public string Name { get; } = name;
        public Board Board { get; } = board;

        public List<(int x, int y, ShotResult result)> MoveHistory { get; } = new();
    }
}
