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
    protected HashSet<(int x, int y)> _shotsMade = new();
    protected bool _huntingMode = false;
    protected (int x, int y)? _huntOrigin = null;
    protected string _huntDirection = "unknown";
    protected int _huntDirectionTried = 0;
    protected List<(int x, int y)> _hits = new();

    public virtual string Name => "Easy";

    public virtual Tuple<int, int> BotShotSelection()
    {
        // Directional Hunt Mode
        if (_huntingMode && _huntOrigin.HasValue)
        {
            var origin = _huntOrigin.Value;

            if (_huntDirection == "unknown")
            {
                // Try Up
                (int x, int y) up = (origin.x, origin.y - 1);
                if (IsInBounds(up) && !_shotsMade.Contains(up))
                {
                    _huntDirection = "vertical";
                    _huntDirectionTried = 0;
                    _shotsMade.Add(up);
                    return Tuple.Create(up.x, up.y);
                }
                // Try Down
                (int x, int y) down = (origin.x, origin.y + 1);
                if (IsInBounds(down) && !_shotsMade.Contains(down))
                {
                    _huntDirection = "vertical";
                    _huntDirectionTried = 1;
                    _shotsMade.Add(down);
                    return Tuple.Create(down.x, down.y);
                }
                _huntDirection = "horizontal";
                _huntDirectionTried = 0;
            }

            if (_huntDirection == "vertical")
            {
                // Hunt up and down from origin
                for (int dir = 0; dir < 2; dir++)
                {
                    int offset = 1;
                    while (true)
                    {
                        int y = origin.y + (dir == 0 ? -offset : offset);
                        (int x, int y) coord = (origin.x, y);
                        if (!IsInBounds(coord) || _shotsMade.Contains(coord))
                            break;
                        _shotsMade.Add(coord);
                        return Tuple.Create(coord.x, coord.y);
                    }
                }
                _huntDirection = "horizontal";
                _huntDirectionTried = 0;
            }

            if (_huntDirection == "horizontal")
            {
                // Hunt left and right from origin
                for (int dir = 0; dir < 2; dir++)
                {
                    int offset = 1;
                    while (true)
                    {
                        int x = origin.x + (dir == 0 ? -offset : offset);
                        (int x, int y) coord = (x, origin.y);
                        if (!IsInBounds(coord) || _shotsMade.Contains(coord))
                            break;
                        _shotsMade.Add(coord);
                        return Tuple.Create(coord.x, coord.y);
                    }
                }
                _huntingMode = false;
                _huntOrigin = null;
                _huntDirection = "unknown";
                _hits.Clear();
            }
        }

        // Default: Random shot
        while (true)
        {
            int x = _rand.Next(0, BoardSize);
            int y = _rand.Next(0, BoardSize);
            (int x, int y) shot = (x, y);
            if (!_shotsMade.Contains(shot))
            {
                _shotsMade.Add(shot);
                return Tuple.Create(x, y);
            }
        }
    }

    public virtual void InformShotResult(Tuple<int, int> coord, ShotResult result)
    {
        (int x, int y) shot = (coord.Item1, coord.Item2);

        if (result == ShotResult.Hit)
        {
            if (!_huntingMode)
            {
                _huntingMode = true;
                _huntOrigin = shot;
                _huntDirection = "unknown";
                _huntDirectionTried = 0;
                _hits.Clear();
                _hits.Add(shot);
            }
            else
            {
                _hits.Add(shot);
                if (_huntDirection == "vertical")
                {
                    if (shot.x != _huntOrigin.Value.x)
                        _huntDirection = "horizontal";
                }
                else if (_huntDirection == "horizontal")
                {
                    if (shot.y != _huntOrigin.Value.y)
                        _huntDirection = "vertical";
                }
            }
        }
        else if (result == ShotResult.Sunk)
        {
            _huntingMode = false;
            _huntOrigin = null;
            _huntDirection = "unknown";
            _huntDirectionTried = 0;
            _hits.Clear();
        }
        else if (result == ShotResult.Miss)
        {
            if (_huntingMode)
            {
                if (_huntDirection == "vertical")
                {
                    _huntDirection = "horizontal";
                    _huntDirectionTried = 0;
                }
                else if (_huntDirection == "horizontal")
                {
                    _huntingMode = false;
                    _huntOrigin = null;
                    _huntDirection = "unknown";
                    _huntDirectionTried = 0;
                    _hits.Clear();
                }
            }
        }
    }

    public virtual void BotShipPlacement(Board board)
    {
        foreach (var ship in board.ships) // Zakładamy, że board.ships to List<ShipBase>
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

    public virtual void AddCellsToAvoid(List<(int col, int row)> cells)
    {
        // No-op for standard bots
    }

    protected bool IsInBounds((int x, int y) coord)
        => coord.x >= 0 && coord.x < BoardSize && coord.y >= 0 && coord.y < BoardSize;
}