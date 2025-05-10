namespace ProgramowanieObiektoweProjekt;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Models.Ships;
using System;
class Program
{
    static int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
    static char[,] board1 = new char[10, 10];
    static char[,] board2 = new char[10, 10];
    static int[] ships = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
    static int currentShip = 0;
    static bool isHorizontal = true;
    static bool player1 = true;
    static bool placementComplete = false;

    static void HandleKeyPress()
    {
        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.LeftArrow:
                if (player1) { if (y1 > 0) y1--; }
                else { if (y2 > 0) y2--; }
                break;
            case ConsoleKey.RightArrow:
                if (player1) { if (y1 < 9 - (isHorizontal ? ships[currentShip] - 1 : 0)) y1++; }
                else { if (y2 < 9 - (isHorizontal ? ships[currentShip] - 1 : 0)) y2++; }
                break;
            case ConsoleKey.UpArrow:
                if (player1) { if (x1 > 0) x1--; }
                else { if (x2 > 0) x2--; }
                break;
            case ConsoleKey.DownArrow:
                if (player1) { if (x1 < 9 - (isHorizontal ? 0 : ships[currentShip] - 1)) x1++; }
                else { if (x2 < 9 - (isHorizontal ? 0 : ships[currentShip] - 1)) x2++; }
                break;
            case ConsoleKey.Spacebar:
                if (CanRotateShip())
                    isHorizontal = !isHorizontal;
                break;
            case ConsoleKey.Enter:
                if (CanPlaceShipHere())
                {
                    PlaceShipOnBoard();
                    currentShip++;
                    if (currentShip >= ships.Length)
                    {
                        if (player1)
                        {
                            player1 = false;
                            currentShip = 0;
                            x2 = 0; y2 = 0;
                        }
                        else
                        {
                            placementComplete = true;
                        }
                    }
                }
                break;
            case ConsoleKey.Escape:
                placementComplete = true;
                break;
        }
    }

    static bool IsShipPreviewTile(int row, int col)
    {
        if (placementComplete) return false;

        int currentRow = player1 ? x1 : x2;
        int currentCol = player1 ? y1 : y2;
        int shipLength = ships[currentShip];

        if (isHorizontal)
            return row == currentRow && col >= currentCol && col < currentCol + shipLength && col < 10;
        else
            return col == currentCol && row >= currentRow && row < currentRow + shipLength && row < 10;
    }

    static bool CanPlaceShipHere()
    {
        char[,] board = player1 ? board1 : board2;
        int startRow = player1 ? x1 : x2;
        int startCol = player1 ? y1 : y2;
        int shipLength = ships[currentShip];

        if (isHorizontal)
        {
            if (startCol + shipLength > 10) return false;

            for (int col = startCol; col < startCol + shipLength; col++)
                if (board[startRow, col] != '·' || !IsTileAvailable(board, startRow, col)) return false;
        }
        else
        {
            if (startRow + shipLength > 10) return false;

            for (int row = startRow; row < startRow + shipLength; row++)
                if (board[row, startCol] != '·' || !IsTileAvailable(board, row, startCol)) return false;
        }

        return true;
    }

    static bool IsTileAvailable(char[,] board, int row, int col)
    {
        for (int deltaRow = -1; deltaRow <= 1; deltaRow++)
        {
            for (int deltaCol = -1; deltaCol <= 1; deltaCol++)
            {
                int neighborRow = row + deltaRow;
                int neighborCol = col + deltaCol;

                if (neighborRow >= 0 && neighborRow < 10 &&
                    neighborCol >= 0 && neighborCol < 10 &&
                    board[neighborRow, neighborCol] != '·')
                {
                    return false;
                }
            }
        }
        return true;
    }

    static bool CanRotateShip()
    {
        char[,] board = player1 ? board1 : board2;
        int startRow = player1 ? x1 : x2;
        int startCol = player1 ? y1 : y2;
        int shipLength = ships[currentShip];

        if (isHorizontal)
        {
            if (startRow + shipLength > 10) return false;

            for (int row = startRow; row < startRow + shipLength; row++)
                if (board[row, startCol] != '·' || !IsTileAvailable(board, row, startCol)) return false;
        }
        else
        {
            if (startCol + shipLength > 10) return false;

            for (int col = startCol; col < startCol + shipLength; col++)
                if (board[startRow, col] != '·' || !IsTileAvailable(board, startRow, col)) return false;
        }

        return true;
    }

    static void PlaceShipOnBoard()
    {
        char[,] board = player1 ? board1 : board2;
        int startRow = player1 ? x1 : x2;
        int startCol = player1 ? y1 : y2;
        int shipLength = ships[currentShip];

        if (isHorizontal)
        {
            for (int col = startCol; col < startCol + shipLength; col++)
                board[startRow, col] = '■';
        }
        else
        {
            for (int row = startRow; row < startRow + shipLength; row++)
                board[row, startCol] = '■';
        }
    }
}
