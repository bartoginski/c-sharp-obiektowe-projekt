sing System;
using System.Collections.Generic;
using ProgramowanieObiektoweProjekt.Models.Boards; 
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Enums; 

namespace ProgramowanieObiektoweProjekt.Bot.BotEasy
{
    class BotEasy
    {
        public string Name = "Poziom £atwy";
        // Initiating random number generation
        private readonly Random _rand = new Random();

        public virtual Tuple<int, int> BotShotSelection()
        {
            // Generating random numbers
            int columnnum1 = _rand.Next(1, 10);
            int columnnum2 = _rand.Next(1, 10);
            int columnnum3 = _rand.Next(1, 10);
            int randomIndex = _rand.Next(1, 4);
            int chosenColumnNumber;

            // Select the number based on the random index
            if (randomIndex == 1)
            {
                chosenColumnNumber = columnnum1;
            }
            else if (randomIndex == 2)
            {
                chosenColumnNumber = columnnum2;
            }
            else
            {
                chosenColumnNumber = columnnum3;
            }

            // Generating random numbers
            int rownum1 = _rand.Next(1, 10);
            int rownum2 = _rand.Next(1, 10);
            int rownum3 = _rand.Next(1, 10);
            int randomIndex = _rand.Next(1, 4);
            int chosenRowNumber;

            // Select the number based on the random index
            if (randomIndex == 1)
            {
                chosenRowNumber = rownum1;
            }
            else if (randomIndex == 2)
            {
                chosenRowNumber = rownum2;
            }
            else
            {
                chosenRowNumber = rownum3;
            }
            return Tuple.Create(chosenColumnNumber, chosenRowNumber);
        }

        public virtual void BotShipPlacement(Board board) // Now takes a Board object as a parameter
        {
            List<ShipBase> shipsToPlace = new List<ShipBase>(board.ships); // Use the board's ship list

            foreach (var ship in shipsToPlace)
            {
                bool placed = false;
                while (!placed)
                {
                    // Randomly choose orientation
                    bool isHorizontal = _rand.Next(2) == 0;
                    Direction orientation = isHorizontal ? Direction.Horizontal : Direction.Vertical;

                    int rowStart = 0;
                    int colStart = 0;

                    if (isHorizontal)
                    {
                        colStart = _rand.Next(1, 11 - ship.Length + 1);
                        rowStart = _rand.Next(1, 11);
                    }
                    else // Vertical
                    {
                        rowStart = _rand.Next(1, 11 - ship.Length + 1);
                        colStart = _rand.Next(1, 11);  
                    }

                    // Check if the placement is valid using the Board's method
                    if (board.IsValidPlacement(ship, rowStart, colStart, orientation))
                    {
                        board.PlaceShip(ship, rowStart, colStart, orientation);
                        placed = true;
                    }
                }
            }
        }
    }
}