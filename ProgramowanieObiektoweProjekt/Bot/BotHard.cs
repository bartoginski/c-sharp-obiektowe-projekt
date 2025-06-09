using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

internal class BotHard : BotMedium
{
    private List<(int x, int y)> _diagonalShots = new();
    private int _nextDiagonalIndex = 0;

    public override string Name => "Hard";

    public BotHard()
    {
        // Generate all diagonal cells on the board (checkerboard pattern)
        for (int x = 0; x < BoardSize; x++)
            for (int y = 0; y < BoardSize; y++)
                if ((x + y) % 2 == 0)
                    _diagonalShots.Add((x, y));
        // Shuffle diagonals for randomness
        _diagonalShots = _diagonalShots.OrderBy(_ => _rand.Next()).ToList();
    }

    public override Tuple<int, int> BotShotSelection()
    {
        // 1. Use hunt mode from base if active
        if (_huntingMode)
        {
            var huntMove = base.BotShotSelection();
            if (huntMove != null && !_shotsMade.Contains((huntMove.Item1, huntMove.Item2)))
            {
                return huntMove;
            }
        }

        // 2. Diagonal search
        while (_nextDiagonalIndex < _diagonalShots.Count)
        {
            var coord = _diagonalShots[_nextDiagonalIndex++];
            if (!_shotsMade.Contains(coord))
            {
                _shotsMade.Add(coord);
                return Tuple.Create(coord.x, coord.y);
            }
        }

        // 3. Use BotMedium's sector logic
        return base.BotShotSelection();
    }

    public override void BotShipPlacement(Board board)
    {
        // Try to scatter ships as much as possible, maximizing minimum distance and no adjacency
        var rnd = new Random();
        var ships = board.ships.OrderByDescending(ship => ship.Length).ToList();
        var placedShips = new List<(int x, int y, int length, bool isHorizontal)>();

        foreach (var ship in ships)
        {
            bool placed = false;
            int maxAttempts = 1000;
            int attempts = 0;
            (int x, int y, bool isHorizontal) bestPos = (0, 0, true);
            int bestMinDist = -1;

            while (!placed && attempts < maxAttempts)
            {
                bool isHorizontal = rnd.Next(2) == 0;
                ship.IsHorizontal = isHorizontal;

                int xMax = isHorizontal ? BoardSize - ship.Length : BoardSize - 1;
                int yMax = isHorizontal ? BoardSize - 1 : BoardSize - ship.Length;

                int x = rnd.Next(0, xMax + 1);
                int y = rnd.Next(0, yMax + 1);

                if (!IsValidPlacementWithNoAdjacency(board, x, y, isHorizontal, ship.Length))
                {
                    attempts++;
                    continue;
                }

                // Calculate min distance to other ships
                int minDist = int.MaxValue;
                foreach (var other in placedShips)
                {
                    int x1 = x, y1 = y, x2 = other.x, y2 = other.y;
                    // Get all tiles of this ship
                    for (int j = 0; j < ship.Length; j++)
                    {
                        int sx = isHorizontal ? x1 + j : x1;
                        int sy = isHorizontal ? y1 : y1 + j;
                        for (int k = 0; k < other.length; k++)
                        {
                            int ox = other.isHorizontal ? x2 + k : x2;
                            int oy = other.isHorizontal ? y2 : y2 + k;
                            minDist = Math.Min(minDist, Math.Abs(sx - ox) + Math.Abs(sy - oy));
                        }
                    }
                }
                // Prefer positions with larger minimum distance from other ships
                if (minDist > bestMinDist)
                {
                    bestMinDist = minDist;
                    bestPos = (x, y, isHorizontal);
                }

                attempts++;
            }

            // Place at the best found position
            ship.IsHorizontal = bestPos.isHorizontal;
            if (board.IsValidPlacement(ship, bestPos.x, bestPos.y, bestPos.isHorizontal ? Direction.Horizontal : Direction.Vertical)
                && IsValidPlacementWithNoAdjacency(board, bestPos.x, bestPos.y, bestPos.isHorizontal, ship.Length))
            {
                board.PlaceShip(ship, bestPos.x, bestPos.y, bestPos.isHorizontal ? Direction.Horizontal : Direction.Vertical);
                placedShips.Add((bestPos.x, bestPos.y, ship.Length, bestPos.isHorizontal));
            }
        }
    }

    // Checks: ship fits, no adjacency (sides or corners)
    private bool IsValidPlacementWithNoAdjacency(Board board, int x, int y, bool isHorizontal, int length)
    {
        int dx = isHorizontal ? 1 : 0;
        int dy = isHorizontal ? 0 : 1;

        for (int i = 0; i < length; i++)
        {
            int cx = x + dx * i;
            int cy = y + dy * i;
            for (int ox = -1; ox <= 1; ox++)
            {
                for (int oy = -1; oy <= 1; oy++)
                {
                    int nx = cx + ox;
                    int ny = cy + oy;
                    if (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize)
                    {
                        if (board.GetTile(nx, ny).HasShip)
                            return false;
                    }
                }
            }
        }
        return true;
    }
}