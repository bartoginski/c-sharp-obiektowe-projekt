using System;
using System.Collections.Generic;
using ProgramowanieObiektoweProjekt.Models.Boards; 
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Enums; 

namespace ProgramowanieObiektoweProjekt.Bot.BotEasy
{
    class BotEasy
    {
        public string Name = "Easy";

        public virtual Tuple<int, int> BotShotSelection()
        {
            int row, col;
            bool shotIsNew = false;

            while (!shotIsNew)
            {
                row = _rand.Next(0, BoardSize);
                col = _rand.Next(0, BoardSize);

                if (!_shotsMade.Contains((row, col)))
                {
                    _shotsMade.Add((row, col));
                    shotIsNew = true;
                    return Tuple.Create(row, col);
                }
            }
            return Tuple.Create(-1, -1);
        }
        public virtual void BotShipPlacement(Board board)
        {
            List<ShipBase> shipsToPlace = new List<ShipBase>(board.ships);

            foreach (var ship in shipsToPlace)
            {
                bool placed = false;
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

                    if (board.TryPlaceShip(ship, rowStart, colStart, orientation))
                    {
                        placed = true;
                    }
                }

            }
        }
    }
}