using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Utils;

internal class BotEasy : IBot
{
    protected const int BoardSize = Constants.BoardSize;
    protected Random _rand = new();
    protected HashSet<(int x, int y)> _shotsMade = new();
    protected int _huntDirectionTried = 0;
    protected bool _huntingMode = false;
    private List<(int x, int y)> _hits = new();
    private (int x, int y)? _huntOrigin = null;
    private Direction _huntDirection = Direction.Unknown;

    public virtual string Name => "Easy";

    public virtual Tuple<int, int> BotShotSelection()
    {
        // Hunt mode - target around the hit
        if (_huntingMode && _huntOrigin.HasValue)
        {
            var origin = _huntOrigin.Value;

            if (_huntDirection == Direction.Unknown)
            {
                // Try up first
                (int x, int y) up = (origin.x, origin.y - 1);
                if (_isInBounds(up) && !_shotsMade.Contains(up))
                {
                    _huntDirection = Direction.Vertical;
                    _huntDirectionTried = 0;
                    _shotsMade.Add(up);
                    return Tuple.Create(up.x, up.y);
                }
                // Try down
                (int x, int y) down = (origin.x, origin.y + 1);
                if (_isInBounds(down) && !_shotsMade.Contains(down))
                {
                    _huntDirection = Direction.Vertical;
                    _huntDirectionTried = 1;
                    _shotsMade.Add(down);
                    return Tuple.Create(down.x, down.y);
                }
                _huntDirection = Direction.Horizontal;
                _huntDirectionTried = 0;
            }

            if (_huntDirection == Direction.Vertical)
            {
                // Shoot up and down from origin
                for (int dir = 0; dir < 2; dir++)
                {
                    int offset = 1;
                    while (true)
                    {
                        int y = origin.y + (dir == 0 ? -offset : offset);
                        (int x, int y) coord = (origin.x, y);
                        if (!_isInBounds(coord) || _shotsMade.Contains(coord))
                            break;
                        _shotsMade.Add(coord);
                        return Tuple.Create(coord.x, coord.y);
                    }
                }
                _huntDirection = Direction.Horizontal;
                _huntDirectionTried = 0;
            }

            if (_huntDirection == Direction.Horizontal)
            {
                // Shoot left and right from origin
                for (int dir = 0; dir < 2; dir++)
                {
                    int offset = 1;
                    while (true)
                    {
                        int x = origin.x + (dir == 0 ? -offset : offset);
                        (int x, int y) coord = (x, origin.y);
                        if (!_isInBounds(coord) || _shotsMade.Contains(coord))
                            break;
                        _shotsMade.Add(coord);
                        return Tuple.Create(coord.x, coord.y);
                    }
                }
                // No more directions to try
                _huntingMode = false;
                _huntOrigin = null;
                _huntDirection = Direction.Unknown;
                _hits.Clear();
            }
        }

        // Random shot when not hunting
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
                // Start hunting mode
                _huntingMode = true;
                _huntOrigin = shot;
                _huntDirection = Direction.Unknown;
                _huntDirectionTried = 0;
                _hits.Clear();
                _hits.Add(shot);
            }
            else
            {
                _hits.Add(shot);
                // Keep current direction if working
                if (_huntDirection == Direction.Vertical)
                {
                    if (shot.x != _huntOrigin.Value.x)
                        _huntDirection = Direction.Vertical;
                }
                else if (_huntDirection == Direction.Horizontal)
                {
                    if (shot.y != _huntOrigin.Value.y)
                        _huntDirection = Direction.Vertical;
                }
            }
        }
        else if (result == ShotResult.Sunk)
        {
            // Ship destroyed - stop hunting
            _huntingMode = false;
            _huntOrigin = null;
            _huntDirection = Direction.Unknown;
            _huntDirectionTried = 0;
            _hits.Clear();
        }
        else if (result == ShotResult.Miss)
        {
            if (_huntingMode)
            {
                if (_huntDirection == Direction.Vertical)
                {
                    // Try horizontal direction
                    _huntDirection = Direction.Horizontal;
                    _huntDirectionTried = 0;
                }
                else if (_huntDirection == Direction.Horizontal)
                {
                    // Give up hunting
                    _huntingMode = false;
                    _huntOrigin = null;
                    _huntDirection = Direction.Unknown;
                    _huntDirectionTried = 0;
                    _hits.Clear();
                }
            }
        }
    }

    public virtual void BotShipPlacement(Board board)
    {
        foreach (var ship in board.Ships)
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
        // Not implemented for easy bot
    }

    private bool _isInBounds((int x, int y) coord)
        => coord.x >= 0 && coord.x < BoardSize && coord.y >= 0 && coord.y < BoardSize;
}