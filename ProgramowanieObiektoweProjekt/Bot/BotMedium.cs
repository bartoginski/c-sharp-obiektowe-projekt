using ProgramowanieObiektoweProjekt.Enums;

internal class BotMedium : BotEasy
{
    // Phases: 0 = 25%, 1 = 50%, 2 = 80%, 3 = random
    private int _phase = 0;
    private readonly int[] _thresholds = [25, 50, 80];
    private int _currentSector = 0; // Current sector being targeted
    private Dictionary<int, HashSet<(int x, int y)>> _sectorShots = new();
    private Dictionary<int, int> _sectorSizes = new();

    public BotMedium()
    {
        // Initialize sector tracking
        for (int i = 0; i < 4; i++)
            _sectorShots[i] = new HashSet<(int x, int y)>();

        // Calculate sector sizes
        int half = BoardSize / 2;
        _sectorSizes[0] = half * half; // Top-left
        _sectorSizes[1] = (BoardSize - half) * half; // Top-right
        _sectorSizes[2] = half * (BoardSize - half); // Bottom-left
        _sectorSizes[3] = (BoardSize - half) * (BoardSize - half); // Bottom-right
    }

    public override string Name => "Medium";

    public override Tuple<int, int> BotShotSelection()
    {
        // Hunt mode takes priority
        if (_huntingMode)
        {
            var huntMove = base.BotShotSelection();
            if (huntMove != null && !_shotsMade.Contains((huntMove.Item1, huntMove.Item2)))
            {
                return huntMove;
            }
        }

        UpdatePhase();

        // Find a sector that needs more shots
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

        // All sectors done - use random shots
        return base.BotShotSelection();
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
        return 100; // Switch to random after 80%
    }

    private int GetSectorForCoord(int x, int y)
    {
        int half = BoardSize / 2;
        if (x < half && y < half) return 0; // Top-left
        if (x >= half && y < half) return 1; // Top-right
        if (x < half && y >= half) return 2; // Bottom-left
        return 3; // Bottom-right
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

    public override void InformShotResult(Tuple<int, int> coord, ShotResult result)
    {
        int sector = GetSectorForCoord(coord.Item1, coord.Item2);
        _sectorShots[sector].Add((coord.Item1, coord.Item2));
        base.InformShotResult(coord, result);
    }
}