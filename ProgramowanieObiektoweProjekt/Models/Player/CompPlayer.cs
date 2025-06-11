using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Boards;

namespace ProgramowanieObiektoweProjekt.Models.Player
{
    internal class CompPlayer : IPlayer
    {
        public int Points { get; }

        public string Name { get; }
        public Board Board { get; }

        public List<(int x, int y, ShotResult result)> MoveHistory { get; } = new();

        public CompPlayer(string name, Board board)
        {
            Name = "Computer";
            Board = board;
            Points = 0;
        }

    }
}