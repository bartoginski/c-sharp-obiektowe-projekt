using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;
using System;
using System.Collections.Generic;

internal class BotMedium : BotEasy
{
    private int _phase = 0;
    private readonly int[] _thresholds = { 25, 50, 80 };
    private int _currentSector = 0;
    private Dictionary<int, HashSet<(int x, int y)>> _sectorShots = new();
    private Dictionary<int, int> _sectorSizes = new();

    public BotMedium()
    {
        for (int i = 0; i < 4; i++)
            _sectorShots[i] = new HashSet<(int x, int y)>();

        int half = BoardSize / 2;
        _sectorSizes[0] = half * half;
        _sectorSizes[1] = (BoardSize - half) * half;
        _sectorSizes[2] = half * (BoardSize - half);
        _sectorSizes[3] = (BoardSize - half) * (BoardSize - half);
    }

    public override string Name => "Medium";

    public override Tuple<int, int> BotShotSelection()
    {
        if (_huntingMode && _hits.Count > 0)
        {
            string direction = _huntDirection;
            if (_huntDirection == "unknown" && _hits.Count >= 2)
            {
                var h0 = _hits[0];
                var h1 = _hits[1];
                if (h0.x == h1.x)
                    direction = "vertical";
                else if (h0.y == h1.y)
                    direction = "horizontal";
            }
            else
            {
                direction = _huntDirection;
            }

            foreach (var origin in _hits)
            {
                if (direction == "unknown")
                {
                    (int x, int y) up = (origin.x, origin.y - 1);
                    if (IsInBounds(up) && !_shotsMade.Contains(up))
                    {
                        _huntDirection = "vertical";
                        _huntDirectionTried = 0;
                        _shotsMade.Add(up);
                        return Tuple.Create(up.x, up.y);
                    }
                    (int x, int y) down = (origin.x, origin.y + 1);
                    if (IsInBounds(down) && !_shotsMade.Contains(down))
                    {
                        _huntDirection = "vertical";
                        _huntDirectionTried = 1;
                        _shotsMade.Add(down);
                        return Tuple.Create(down.x, down.y);
                    }
                    (int x, int y) left = (origin.x - 1, origin.y);
                    if (IsInBounds(left) && !_shotsMade.Contains(left))
                    {
                        _huntDirection = "horizontal";
                        _huntDirectionTried = 0;
                        _shotsMade.Add(left);
                        return Tuple.Create(left.x, left.y);
                    }
                    (int x, int y) right = (origin.x + 1, origin.y);
                    if (IsInBounds(right) && !_shotsMade.Contains(right))
                    {
                        _huntDirection = "horizontal";
                        _huntDirectionTried = 1;
                        _shotsMade.Add(right);
                        return Tuple.Create(right.x, right.y);
                    }
                }
                else if (direction == "vertical")
                {
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
                }
                else if (direction == "horizontal")
                {
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
                }
            }
        }

        UpdatePhase();

        for (int i = 0; i < 4; i++)
        {
            int sector = (_currentSector + i) % 4;
            if (GetSectorShotPercent(sector) < GetCurrentThreshold())
            {
                _currentSector = sector;
                var coord = SelectShotInSector(sector);
                if (coord != null)
                    return coord;
            }
        }

        return base.BotShotSelection();
    }

    // MODIFIED: Added sunkShipCoords parameter and use it for marking "box"
    public override void InformShotResult(Tuple<int, int> coord, ShotResult result, List<(int x, int y)> sunkShipCoords = null)
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
                if (!_hits.Contains(shot))
                    _hits.Add(shot);
                if (_hits.Count == 2)
                {
                    var h0 = _hits[0];
                    var h1 = _hits[1];
                    if (h0.x == h1.x)
                        _huntDirection = "vertical";
                    else if (h0.y == h1.y)
                        _huntDirection = "horizontal";
                }
                else
                {
                    if (_huntDirection == "vertical" && shot.x != _huntOrigin.Value.x)
                        _huntDirection = "horizontal";
                    else if (_huntDirection == "horizontal" && shot.y != _huntOrigin.Value.y)
                        _huntDirection = "vertical";
                }
            }
        }
        else if (result == ShotResult.Sunk)
        {
            // Use all ship coordinates if provided, else fallback to _hits (for compatibility)
            var cells = sunkShipCoords ?? _hits;

            foreach (var cell in cells)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = cell.x + dx;
                        int ny = cell.y + dy;
                        if (IsInBounds((nx, ny)))
                        {
                            _shotsMade.Add((nx, ny));
                        }
                    }
                }
            }

            _huntingMode = false;
            _huntOrigin = null;
            _huntDirection = "unknown";
            _huntDirectionTried = 0;
            _hits.Clear();
        }
        else if (result == ShotResult.Miss)
        {
            _huntDirection = "unknown";
        }
    }

    private void UpdatePhase()
    {
        bool phaseComplete = true;
        for (int i = 0; i < 4; i++)
        {
            if (GetSectorShotPercent(i) < GetCurrentThreshold())
            {
                phaseComplete = false;
                break;
            }
        }
        if (phaseComplete && _phase < _thresholds.Length)
            _phase++;
    }

    private int GetCurrentThreshold()
    {
        if (_phase < _thresholds.Length)
            return _thresholds[_phase];
        return 100;
    }

    private int GetSectorForCoord(int x, int y)
    {
        int half = BoardSize / 2;
        if (x < half && y < half) return 0;
        if (x >= half && y < half) return 1;
        if (x < half && y >= half) return 2;
        return 3;
    }

    private double GetSectorShotPercent(int sector)
    {
        return (_sectorShots[sector].Count * 100.0) / _sectorSizes[sector];
    }

    private Tuple<int, int>? SelectShotInSector(int sector)
    {
        int half = BoardSize / 2;
        int xStart = (sector == 0 || sector == 2) ? 0 : half;
        int yStart = (sector == 0 || sector == 1) ? 0 : half;
        int xEnd = (sector == 0 || sector == 2) ? half : BoardSize;
        int yEnd = (sector == 0 || sector == 1) ? half : BoardSize;

        List<(int x, int y)> candidates = new();
        for (int x = xStart; x < xEnd; x++)
            for (int y = yStart; y < yEnd; y++)
                if (!_shotsMade.Contains((x, y)))
                    candidates.Add((x, y));

        if (candidates.Count > 0)
        {
            var coord = candidates[_rand.Next(candidates.Count)];
            _shotsMade.Add(coord);
            _sectorShots[sector].Add(coord);
            return Tuple.Create(coord.x, coord.y);
        }
        return null;
    }
}