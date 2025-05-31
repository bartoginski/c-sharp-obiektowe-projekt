using System;
using System.Generic;
using System.Linq;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Bot.BotEasy;
using ProgramowanieObiektoweProjekt.Bot.BotMedium;
using ProgramowanieObiektoweProjekt.Interfaces;

namespace ProgramowanieObiektoweProjekt.Bot.BotHard
{
    class BotHard : BotMedium.BotMedium, IBot
    {
        public override string Name { get; } = "Hard";

        private List<Tuple<int, int>> _diagonalShotsOrder = new List<Tuple<int, int>>();
        private int _currentDiagonalShotIndex = 0;

        public BotHard()
        {
            InitializeDiagonalShotsOrder();
        }

        private void InitializeDiagonalShotsOrder()
        {
            for (int sum = 0; sum <= (BoardSize - 1) * 2; sum++)
            {
                for (int r = 0; r < BoardSize; r++)
                {
                    int c = sum - r;
                    if (c >= 0 && c < BoardSize)
                    {
                        _diagonalShotsOrder.Add(Tuple.Create(r, c));
                    }
                }
            }
            _diagonalShotsOrder = _diagonalShotsOrder.Distinct().OrderBy(x => Guid.NewGuid()).ToList();
        }

        public override Tuple<int, int> BotShotSelection()
        {
            if (_huntingMode && _huntQueue.Any())
            {
                while (_huntQueue.Any())
                {
                    Tuple<int, int> target = _huntQueue.Dequeue();
                    if (!_shotsMade.Contains(target))
                    {
                        return target;
                    }
                }
                RePopulateHuntQueueFromHits();
                if (_huntQueue.Any())
                {
                    return _huntQueue.Dequeue();
                }
                else
                {
                    _huntingMode = false;
                }
            }

            if (_currentDiagonalShotIndex < _diagonalShotsOrder.Count)
            {
                Tuple<int, int> shot;
                do
                {
                    shot = _diagonalShotsOrder[_currentDiagonalShotIndex];
                    _currentDiagonalShotIndex++;
                } while (_shotsMade.Contains(shot) && _currentDiagonalShotIndex < _diagonalShotsOrder.Count);

                if (!_shotsMade.Contains(shot))
                {
                    return shot;
                }
            }

            return base.BotShotSelection();
        }

        public override void BotShipPlacement(Board board)
        {
            List<ShipBase> shipsToPlace = new List<ShipBase>(board.ships);

            shipsToPlace = shipsToPlace.OrderByDescending(s => s.Length).ToList();

            foreach (var ship in shipsToPlace)
            {
                bool placed = false;
                int attempts = 0;
                int maxAttempts = 500;

                while (!placed && attempts < maxAttempts)
                {
                    attempts++;
                    bool isHorizontal = _rand.Next(2) == 0;
                    Direction orientation = isHorizontal ? Direction.Horizontal : Direction.Vertical;

                    int rowStart, colStart;

                    if (ship.Length > 2 && _rand.Next(100) < 70)
                    {
                        if (isHorizontal)
                        {
                            rowStart = _rand.Next(2) == 0 ? 0 : BoardSize - 1;
                            colStart = _rand.Next(0, BoardSize - ship.Length + 1);
                        }
                        else
                        {
                            colStart = _rand.Next(2) == 0 ? 0 : BoardSize - 1;
                            rowStart = _rand.Next(0, BoardSize - ship.Length + 1);
                        }
                    }
                    else
                    {
                        if (isHorizontal)
                        {
                            colStart = _rand.Next(0, BoardSize - ship.Length + 1);
                            rowStart = _rand.Next(0, BoardSize);
                        }
                        else
                        {
                            rowStart = _rand.Next(0, BoardSize - ship.Length + 1);
                            colStart = _rand.Next(0, BoardSize);
                        }
                    }

                    double placementScore = EvaluatePlacement(board, ship, rowStart, colStart, orientation);

                    if (placementScore > -1000 && board.TryPlaceShip(ship, colStart, rowStart, orientation))
                    {
                        placed = true;
                    }
                }
                if (!placed)
                {
                    Console.WriteLine($"Could not place {ship.Name} strategically, placing randomly.");
                    while (!placed)
                    {
                        bool isHorizontal = _rand.Next(2) == 0;
                        Direction orientation = isHorizontal ? Direction.Horizontal : Direction.Vertical;

                        int rowStart, colStart;

                        if (isHorizontal)
                        {
                            colStart = _rand.Next(0, BoardSize - ship.Length + 1);
                            rowStart = _rand.Next(0, BoardSize);
                        }
                        else
                        {
                            rowStart = _rand.Next(0, BoardSize - ship.Length + 1);
                            colStart = _rand.Next(0, BoardSize);
                        }
                        if (board.TryPlaceShip(ship, colStart, rowStart, orientation))
                        {
                            placed = true;
                        }
                    }
                }
            }
        }

        private double EvaluatePlacement(Board board, ShipBase ship, int startRow, int startCol, Direction direction)
        {
            double score = 0;
            int endRow = direction == Direction.Vertical ? startRow + ship.Length - 1 : startRow;
            int endCol = direction == Direction.Horizontal ? startCol + ship.Length - 1 : startCol;

            if (endRow >= BoardSize || endCol >= BoardSize || startRow < 0 || startCol < 0)
            {
                return -9999;
            }

            for (int r = startRow - 1; r <= endRow + 1; r++)
            {
                for (int c = startCol - 1; c <= endCol + 1; c++)
                {
                    if (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize)
                    {
                        bool isCurrentShipPosition = false;
                        if (direction == Direction.Horizontal)
                        {
                            isCurrentShipPosition = (r == startRow && c >= startCol && c <= endCol);
                        }
                        else
                        {
                            isCurrentShipPosition = (c == startCol && r >= startRow && r <= endRow);
                        }

                        if (!isCurrentShipPosition && board.GetTile(r, c).HasShip)
                        {
                            score -= 50;
                        }
                    }
                }
            }

            if (ship.Length > 2)
            {
                if (startRow == 0 || endRow == BoardSize - 1 || startCol == 0 || endCol == BoardSize - 1)
                {
                    score += 20;
                }
            }

            if (ship.Length == 1)
            {
                bool tooClose = false;
                for (int r = startRow - 2; r <= endRow + 2; r++)
                {
                    for (int c = startCol - 2; c <= endCol + 2; c++)
                    {
                        if (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize)
                        {
                            bool isCurrentShipPosition = false;
                            if (direction == Direction.Horizontal)
                            {
                                isCurrentShipPosition = (r == startRow && c >= startCol && c <= endCol);
                            }
                            else
                            {
                                isCurrentShipPosition = (c == startCol && r >= startRow && r <= endRow);
                            }

                            if (!isCurrentShipPosition && board.GetTile(r, c).HasShip)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                    }
                    if (tooClose) break;
                }
                if (!tooClose)
                {
                    score += 15;
                }
                else
                {
                    score -= 10;
                }
            }
            return score;
        }

        public override void InformShotResult(Tuple<int, int> shotCoordinates, ShotResult result)
        {
            base.InformShotResult(shotCoordinates, result);
        }
    }
}