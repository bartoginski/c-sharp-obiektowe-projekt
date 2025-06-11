using ProgramowanieObiektoweProjekt.Enums;

internal class BotMedium : BotEasy
{
    // Phases: 0 = 25%, 1 = 50%, 2 = 80%, 3 = random
    private int _phase = 0;
    private readonly int[] _thresholds = { 25, 50, 80 };
    private int _currentSector = 0; // 0 = TL, 1 = TR, 2 = BL, 3 = BR
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
        // Use hunt mode from BotEasy if active
        if (_huntingMode)
        {
            var huntMove = base.BotShotSelection();
            // If hunt mode actually returned a move, use it
            // If the inherited logic falls through to random, proceed to sector logic
            if (huntMove != null && !_shotsMade.Contains((huntMove.Item1, huntMove.Item2)))
            {
                return huntMove;
            }
        }

        UpdatePhase();

        // Try each sector, starting from current, for one that hasn't met the threshold
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

        // All sectors past 80%: fallback to BotEasy
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
        return 100; // After 80%, switch to random
    }

    private int GetSectorForCoord(int x, int y)
    {
        int half = BoardSize / 2;
        if (x < half && y < half) return 0; // TL
        if (x >= half && y < half) return 1; // TR
        if (x < half && y >= half) return 2; // BL
        return 3; // BR
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