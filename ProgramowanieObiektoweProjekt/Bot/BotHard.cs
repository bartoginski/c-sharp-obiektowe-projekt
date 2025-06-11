using ProgramowanieObiektoweProjekt.Enums;

internal class BotHard : BotMedium
{
    // Track shots in each board sector
    private Dictionary<int, HashSet<(int x, int y)>> _sectorDiagonalShots = new();
    private Dictionary<int, int> _sectorDiagonalSizes = new();
    private int _currentSector = 0;
    private const int DiagonalThreshold = 80; // Stop at 80% coverage

    public override string Name => "Hard";

    public BotHard()
    {
        // Initialize 4 sectors: TL, TR, BL, BR
        for (int i = 0; i < 4; i++)
        {
            _sectorDiagonalShots[i] = new HashSet<(int x, int y)>();
            _sectorDiagonalSizes[i] = 0;
        }
        
        // Count diagonal cells in each sector
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if ((x + y) % 2 == 0) // Diagonal pattern
                {
                    int sector = GetSectorForCoord(x, y);
                    _sectorDiagonalSizes[sector]++;
                }
            }
        }
    }

    public override Tuple<int, int> BotShotSelection()
    {
        // Hunt mode first (from parent class)
        if (_huntingMode)
        {
            var huntMove = base.BotShotSelection();
            if (huntMove != null && !_shotsMade.Contains((huntMove.Item1, huntMove.Item2)))
            {
                return huntMove;
            }
        }

        // Try diagonal shots in each sector
        for (int i = 0; i < 4; i++)
        {
            int sector = (_currentSector + i) % 4;
            if (GetSectorDiagonalShotPercent(sector) < DiagonalThreshold)
            {
                _currentSector = sector;
                var coord = SelectDiagonalInSector(sector);
                if (coord != null)
                    return coord;
            }
        }

        // Fallback to random shots
        return base.BotShotSelection();
    }

    private int GetSectorForCoord(int x, int y)
    {
        int half = BoardSize / 2;
        if (x < half && y < half) return 0; // Top-left
        if (x >= half && y < half) return 1; // Top-right
        if (x < half && y >= half) return 2; // Bottom-left
        return 3; // Bottom-right
    }

    private double GetSectorDiagonalShotPercent(int sector)
    {
        if (_sectorDiagonalSizes[sector] == 0)
            return 100.0;
        return (_sectorDiagonalShots[sector].Count * 100.0) / _sectorDiagonalSizes[sector];
    }

    private Tuple<int, int>? SelectDiagonalInSector(int sector)
    {
        int half = BoardSize / 2;
        int xStart = (sector == 0 || sector == 2) ? 0 : half;
        int yStart = (sector == 0 || sector == 1) ? 0 : half;
        int xEnd = (sector == 0 || sector == 2) ? half : BoardSize;
        int yEnd = (sector == 0 || sector == 1) ? half : BoardSize;

        List<(int x, int y)> candidates = new();
        for (int x = xStart; x < xEnd; x++)
            for (int y = yStart; y < yEnd; y++)
                if ((x + y) % 2 == 0 && !_shotsMade.Contains((x, y)))
                    candidates.Add((x, y));

        if (candidates.Count > 0)
        {
            var coord = candidates[_rand.Next(candidates.Count)];
            _shotsMade.Add(coord);
            _sectorDiagonalShots[sector].Add(coord);
            return Tuple.Create(coord.x, coord.y);
        }
        return null;
    }

    public override void InformShotResult(Tuple<int, int> coord, ShotResult result)
    {
        int sector = GetSectorForCoord(coord.Item1, coord.Item2);
        if ((coord.Item1 + coord.Item2) % 2 == 0)
            _sectorDiagonalShots[sector].Add((coord.Item1, coord.Item2));
        base.InformShotResult(coord, result);
    }
}