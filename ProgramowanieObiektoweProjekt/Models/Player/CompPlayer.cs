using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Interfaces;
using ProgramowanieObiektoweProjekt.Models.Boards;

namespace ProgramowanieObiektoweProjekt.Models.Player
{
    internal class CompPlayer : IPlayer
    {
        private int _points;
        public int Points => _points;

        public string Name { get; }
        public Board Board { get; }

        private readonly List<(int x, int y, ShotResult result)> _moveHistory = new();
        public List<(int x, int y, ShotResult result)> MoveHistory => _moveHistory;

        public CompPlayer(string name, Board board)
        {
            Name = "Computer";
            Board = board;
            _points = 0;
        }

    }
}