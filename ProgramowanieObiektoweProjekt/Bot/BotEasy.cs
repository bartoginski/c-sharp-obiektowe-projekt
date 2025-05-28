<<<<<<< Updated upstream
using System;
using System.Collections.Generic;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Enums;
=======
sing System;
using System.Collections.Generic;
using ProgramowanieObiektoweProjekt.Models.Boards; 
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Enums; 
>>>>>>> Stashed changes

namespace ProgramowanieObiektoweProjekt.Bot.BotEasy
{
    class BotEasy
    {
        public string Name = "Easy";
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // Initiating random number generation
        private readonly Random _rand = new Random();
        // A list to store previously shot coordinates
        private readonly List<Tuple<int, int>> _shotsFired = new List<Tuple<int, int>>();

        public virtual Tuple<int, int> BotShotSelection()
        {
            Tuple<int, int> shot;
            do
            {
                // Generating random numbers for column
                int columnnum1 = _rand.Next(1, 11);
                int columnnum2 = _rand.Next(1, 11);
                int columnnum3 = _rand.Next(1, 11);
                int randomIndexCol = _rand.Next(1, 4); // Renamed to avoid duplicate declaration
                int chosenColumnNumber;

                // Select the number based on the random index
                if (randomIndexCol == 1)
                {
                    chosenColumnNumber = columnnum1;
                }
                else if (randomIndexCol == 2)
                {
                    chosenColumnNumber = columnnum2;
                }
                else
                {
                    chosenColumnNumber = columnnum3;
                }

                // Generating random numbers for row
                int rownum1 = _rand.Next(1, 11);
                int rownum2 = _rand.Next(1, 11);
                int rownum3 = _rand.Next(1, 11);
                int randomIndexRow = _rand.Next(1, 4); // Renamed to avoid duplicate declaration
                int chosenRowNumber;

                // Select the number based on the random index
                if (randomIndexRow == 1)
                {
                    chosenRowNumber = rownum1;
                }
                else if (randomIndexRow == 2)
                {
                    chosenRowNumber = rownum2;
                }
                else
                {
                    chosenRowNumber = rownum3;
                }
                shot = Tuple.Create(chosenColumnNumber, chosenRowNumber);

            } while (_shotsFired.Contains(shot)); // Keep looping if the shot has already been fired

            // Add the new, unique shot to the list of fired shots
            _shotsFired.Add(shot);
            return shot;
        }

        public virtual void BotShipPlacement(Board board) // Now takes a Board object as a parameter
        {
            List<ShipBase> shipsToPlace = new List<ShipBase>(board.ships); // Use the board's ship list
=======
=======
>>>>>>> Stashed changes

        public virtual Tuple<int, int> BotShotSelection()
        {
            int row, col;
            bool shotIsNew = false;

            // Loop until a unique set of coordinates is found
            while (!shotIsNew)
            {
                // Generate random 0-indexed row and column within board boundaries
                row = _rand.Next(0, BoardSize); // Generates 0 to 9
                col = _rand.Next(0, BoardSize); // Generates 0 to 9

                // Check if these coordinates have already been shot
                if (!_shotsMade.Contains((row, col)))
                {
                    _shotsMade.Add((row, col)); // Add the new unique shot to the set
                    shotIsNew = true;           // Mark as new to exit the loop
                    return Tuple.Create(row, col); // Return the unique coordinates
                }
                // If the shot was not new, the loop continues to generate new random coordinates
            }
            // This return should theoretically only be hit if all possible board cells are shot,
            // or if an unforeseen issue occurs.
            return Tuple.Create(-1, -1);
        }
        public virtual void BotShipPlacement(Board board)
        {
            // ... (rest of BotShipPlacement method remains the same) ...
            List<ShipBase> shipsToPlace = new List<ShipBase>(board.ships);
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

            foreach (var ship in shipsToPlace)
            {
                bool placed = false;
                while (!placed)
                {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
=======
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            }
        }
    }
}