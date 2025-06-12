using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Ships; // Dla ShipBase
using ProgramowanieObiektoweProjekt.Utils; // Dla Constants
using System;
using Spectre.Console; // Dla ewentualnych komunikatów, choć nie są tu używane bezpośrednio

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    public class KeyControl
    {
        private readonly Board _board;

        public static int currentShipIndexForPlacement = 0; // Indeks aktualnie umieszczanego statku
        public static bool placementComplete = false;

        // Aktualne koordynaty "kursora" na planszy (lewego górnego rogu statku)
        // x_coor to kolumna, y_coor to wiersz
        private int x_coor = 0;
        private int y_coor = 0;

        public KeyControl(Board board)
        {
            _board = board;
            // Resetuj stan przy tworzeniu nowego KeyControl, jeśli potrzebne
            currentShipIndexForPlacement = 0;
            placementComplete = false;
            x_coor = 0;
            y_coor = 0;
        }

        public int GetCurrentX() => x_coor;
        public int GetCurrentY() => y_coor;

        public void HandleKeyPress()
        {
            if (placementComplete || currentShipIndexForPlacement >= _board.ships.Count)
            {
                placementComplete = true; // Upewnij się, że jest to ustawione
                return;
            }

            var currentShip = _board.ships[currentShipIndexForPlacement];
            var key = Console.ReadKey(true).Key;

            int prev_x = x_coor;
            int prev_y = y_coor;
            bool prev_isHor = currentShip.IsHorizontal;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (x_coor > 0) x_coor--;
                    break;
                case ConsoleKey.RightArrow:
                    // Ograniczenie ruchu w prawo, aby statek nie wyszedł poza planszę
                    int maxX = Constants.BoardSize - (currentShip.IsHorizontal ? currentShip.Length : 1);
                    if (x_coor < maxX) x_coor++;
                    break;
                case ConsoleKey.UpArrow:
                    if (y_coor > 0) y_coor--;
                    break;
                case ConsoleKey.DownArrow:
                    // Ograniczenie ruchu w dół
                    int maxY = Constants.BoardSize - (currentShip.IsHorizontal ? 1 : currentShip.Length);
                    if (y_coor < maxY) y_coor++;
                    break;
                case ConsoleKey.Spacebar:
                    // Spróbuj obrócić statek
                    currentShip.IsHorizontal = !currentShip.IsHorizontal;
                    // Sprawdź, czy po obrocie statek nadal mieści się na planszy i jest w prawidłowej pozycji
                    // Jeśli nie, cofnij obrót i pozycję kursora (jeśli obrót go przesunął poza planszę)
                    int newMaxX_afterRotate = Constants.BoardSize - (currentShip.IsHorizontal ? currentShip.Length : 1);
                    int newMaxY_afterRotate = Constants.BoardSize - (currentShip.IsHorizontal ? 1 : currentShip.Length);

                    if (x_coor > newMaxX_afterRotate) x_coor = newMaxX_afterRotate;
                    if (y_coor > newMaxY_afterRotate) y_coor = newMaxY_afterRotate;
                    
                    // Sprawdzenie, czy obrót jest w ogóle możliwy w tym miejscu
                    // Jeśli po obrocie jest nieprawidłowe umieszczenie, cofnij obrót.
                    // W tym miejscu nie używamy IsValidPlacement, bo to tylko zmiana orientacji.
                    // IsValidPlacement jest używane przy Enter.
                    // Jednak warto by było sprawdzić, czy obrót jest "legalny" w sensie granic planszy.
                    Direction prospectiveDir = currentShip.IsHorizontal ? Direction.Horizontal : Direction.Vertical;
                    // Tutaj nie ma pełnej walidacji z Board.IsValidPlacement, bo to tylko rotacja
                    // Pełna walidacja przy Enter.
                    break;
                case ConsoleKey.Enter:
                    Direction dir = currentShip.IsHorizontal ? Direction.Horizontal : Direction.Vertical;
                    if (_board.IsValidPlacement(currentShip, x_coor, y_coor, dir))
                    {
                        _board.PlaceShip(currentShip, x_coor, y_coor, dir);
                        currentShipIndexForPlacement++;
                        x_coor = 0; // Reset pozycji dla następnego statku
                        y_coor = 0;
                        if (currentShipIndexForPlacement >= _board.ships.Count)
                        {
                            placementComplete = true;
                        }
                    }
                    else
                    {
                        // Opcjonalnie: sygnał dźwiękowy lub krótki komunikat o błędzie
                        // AnsiConsole.MarkupLine("[red]Nie można umieścić statku w tym miejscu![/]");
                        // Thread.Sleep(200); // Aby gracz zauważył
                    }
                    break;
                case ConsoleKey.Escape: // Pozwól na wcześniejsze zakończenie rozmieszczania
                    placementComplete = true;
                    break;
            }
        }

        public bool IsShipPreviewTile(int row, int col)
        {
            if (placementComplete || currentShipIndexForPlacement >= _board.ships.Count)
                return false;

            var currentShip = _board.ships[currentShipIndexForPlacement];
            if (currentShip.IsHorizontal)
            {
                return row == y_coor && col >= x_coor && col < x_coor + currentShip.Length;
            }
            else // Vertical
            {
                return col == x_coor && row >= y_coor && row < y_coor + currentShip.Length;
            }
        }
        // Usunięto CanRotateShip, CanPlaceShipHere, IsTileAvailable, ponieważ
        // główną logikę walidacji przejmuje teraz Board.IsValidPlacement wywoływane przy Enter.
        // Wizualny feedback (np. kolorowanie podglądu na czerwono) jest teraz w Board.GetBoardRenderable.
    }
}