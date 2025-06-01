using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

internal class BotEasy : IBot
{
    protected const int BoardSize = Constants.BoardSize;
    protected Random _rand = new();
    protected List<(int x, int y)> _shotsMade = new();
    protected Queue<(int x, int y)> _huntQueue = new();
    protected bool _huntingMode = false;
    protected List<(int x, int y)> _hits = new();

    public virtual string Name => "Easy";

    public virtual Tuple<int, int> BotShotSelection()
    {
        if (_huntingMode && _huntQueue.Any())
        {
            while (_huntQueue.Any())
            {
                var target = _huntQueue.Dequeue();
                if (!_shotsMade.Contains(target))
                {
                    _shotsMade.Add(target);
                    return Tuple.Create(target.x, target.y);
                }
            }
            RePopulateHuntQueueFromHits();
            if (_huntQueue.Any())
            {
                var target = _huntQueue.Dequeue();
                if (!_shotsMade.Contains(target))
                {
                    _shotsMade.Add(target);
                    return Tuple.Create(target.x, target.y);
                }
            }
            _huntingMode = false;
        }

        while (true)
        {
            int x = _rand.Next(0, BoardSize);
            int y = _rand.Next(0, BoardSize);

            if (!_shotsMade.Contains((x, y)))
            {
                _shotsMade.Add((x, y));
                return Tuple.Create(x, y);
            }
        }
    }

    public virtual void BotShipPlacement(Board board)
    {
        foreach (var ship in board.ships)
        {
            bool placed = false;
            while (!placed)
            {
                Direction dir = _rand.Next(2) == 0 ? Direction.Horizontal : Direction.Vertical;
                ship.IsHorizontal = (dir == Direction.Horizontal);

                int x, y;

                if (dir == Direction.Horizontal)
                {
                    x = _rand.Next(0, BoardSize - ship.Length + 1);
                    y = _rand.Next(0, BoardSize);
                }
                else
                {
                    x = _rand.Next(0, BoardSize);
                    y = _rand.Next(0, BoardSize - ship.Length + 1);
                }

                if (board.IsValidPlacement(ship, x, y, dir))
                {
                    board.PlaceShip(ship, x, y, dir);
                    placed = true;
                }
            }
        }
    }

    public virtual void InformShotResult(Tuple<int, int> coord, ShotResult result)
    {
        if (result == ShotResult.Hit)
        {
            _huntingMode = true;
            _hits.Add((coord.Item1, coord.Item2));
            EnqueueAdjacent((coord.Item1, coord.Item2));
        }
        else if (result == ShotResult.Sunk)
        {
            _hits.Clear();
            _huntQueue.Clear();
            _huntingMode = false;
        }
    }

    protected void EnqueueAdjacent((int x, int y) coord)
    {
        int col = coord.x;
        int row = coord.y;

        var adj = new List<(int x, int y)>
        {
            (col, row - 1),
            (col, row + 1),
            (col - 1, row),
            (col + 1, row)
        };

        foreach (var t in adj)
        {
            if (IsInBounds(t) && !_shotsMade.Contains(t) && !_huntQueue.Contains(t))
            {
                _huntQueue.Enqueue(t);
            }
        }
    }

    protected void RePopulateHuntQueueFromHits()
    {
        _huntQueue.Clear();
        foreach (var h in _hits)
        {
            EnqueueAdjacent(h);
        }
    }

    protected bool IsInBounds((int x, int y) coord)
    {
        return coord.x >= 0 && coord.x < BoardSize &&
               coord.y >= 0 && coord.y < BoardSize;
    }

    // CHANGED: Bot shooting now interacts with Board and uses IsHit via Board.Shoot
    public virtual ShotResult BotShoot(Board board)
    {
        var target = BotShotSelection();
        ShotResult result = board.Shoot(target.Item1, target.Item2);
        InformShotResult(target, result);
        return result;
    }
}