using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;
using System;
using System.Collections.Generic;
using System.Linq; // Added for .Any()

public class BotEasy : IBot
{
    protected const int BoardSize = 10;
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
                    _shotsMade.Add(target); // Add to shotsMade only if it's a new shot
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
            int x = _rand.Next(0, BoardSize); // X is column
            int y = _rand.Next(0, BoardSize); // Y is row

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
                ship.IsHorizontal = (dir == Direction.Horizontal); // Update ship's IsHorizontal property

                int x, y; // x for column (horizontal position), y for row (vertical position)

                if (dir == Direction.Horizontal)
                {
                    x = _rand.Next(0, BoardSize - ship.Length + 1); // X (column) can be placed such that the whole ship fits horizontally
                    y = _rand.Next(0, BoardSize); // Y (row) can be anywhere
                }
                else // Vertical
                {
                    x = _rand.Next(0, BoardSize); // X (column) can be anywhere
                    y = _rand.Next(0, BoardSize - ship.Length + 1); // Y (row) can be placed such that the whole ship fits vertically
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
            _hits.Add((coord.Item1, coord.Item2)); // Store the hit coordinate
            EnqueueAdjacent((coord.Item1, coord.Item2)); // Enqueue adjacent cells to hunt
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
            (col, row - 1), // Up
            (col, row + 1), // Down
            (col - 1, row), // Left
            (col + 1, row)  // Right
        };

        foreach (var t in adj)
        {
            if (IsInBounds(t) && !_shotsMade.Contains(t) && !_huntQueue.Contains(t)) // Also check if already in queue to prevent duplicates
            {
                _huntQueue.Enqueue(t);
            }
        }
    }

    protected void RePopulateHuntQueueFromHits()
    {
        _huntQueue.Clear(); // Clear existing queue before repopulating
        foreach (var h in _hits)
        {
            EnqueueAdjacent(h); // Re-add adjacent cells for all current hits
        }
    }

    protected bool IsInBounds((int x, int y) coord)
    {
        return coord.x >= 0 && coord.x < BoardSize &&
               coord.y >= 0 && coord.y < BoardSize;
    }
}