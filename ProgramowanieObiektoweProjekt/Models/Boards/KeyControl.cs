using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Utils;

namespace ProgramowanieObiektoweProjekt.Models.Boards
{
    internal class KeyControl
    {
        private readonly Board _board;

        public static int CurrentShipIndexForPlacement = 0; // Indeks aktualnie umieszczanego statku
        public static bool PlacementComplete = false;

        // Aktualne koordynaty "kursora" na planszy (lewego górnego rogu statku)
        // x_coor to kolumna, y_coor to wiersz
        private int _xCoor = 0;
        private int _yCoor = 0;

        public KeyControl(Board board)
        {
            _board = board;
            // Resetuj stan przy tworzeniu nowego KeyControl, jeśli potrzebne
            CurrentShipIndexForPlacement = 0;
            PlacementComplete = false;
            _xCoor = 0;
            _yCoor = 0;
        }

        public int GetCurrentX() => _xCoor;
        public int GetCurrentY() => _yCoor;

        public void HandleKeyPress()
        {
            if (PlacementComplete || CurrentShipIndexForPlacement >= _board.Ships.Count)
            {
                PlacementComplete = true; // Upewnij się, że jest to ustawione
                return;
            }

            var currentShip = _board.Ships[CurrentShipIndexForPlacement];
            var key = Console.ReadKey(true).Key;

            int prev_x = _xCoor;
            int prev_y = _yCoor;
            bool prev_isHor = currentShip.IsHorizontal;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (_xCoor > 0) _xCoor--;
                    break;
                case ConsoleKey.RightArrow:
                    // Ograniczenie ruchu w prawo, aby statek nie wyszedł poza planszę
                    int maxX = Constants.BoardSize - (currentShip.IsHorizontal ? currentShip.Length : 1);
                    if (_xCoor < maxX) _xCoor++;
                    break;
                case ConsoleKey.UpArrow:
                    if (_yCoor > 0) _yCoor--;
                    break;
                case ConsoleKey.DownArrow:
                    // Ograniczenie ruchu w dół
                    int maxY = Constants.BoardSize - (currentShip.IsHorizontal ? 1 : currentShip.Length);
                    if (_yCoor < maxY) _yCoor++;
                    break;
                case ConsoleKey.Spacebar:
                    // Spróbuj obrócić statek
                    currentShip.IsHorizontal = !currentShip.IsHorizontal;
                    // Sprawdź, czy po obrocie statek nadal mieści się na planszy i jest w prawidłowej pozycji
                    // Jeśli nie, cofnij obrót i pozycję kursora (jeśli obrót go przesunął poza planszę)
                    int newMaxX_afterRotate = Constants.BoardSize - (currentShip.IsHorizontal ? currentShip.Length : 1);
                    int newMaxY_afterRotate = Constants.BoardSize - (currentShip.IsHorizontal ? 1 : currentShip.Length);

                    if (_xCoor > newMaxX_afterRotate) _xCoor = newMaxX_afterRotate;
                    if (_yCoor > newMaxY_afterRotate) _yCoor = newMaxY_afterRotate;
                    
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
                    if (_board.IsValidPlacement(currentShip, _xCoor, _yCoor, dir))
                    {
                        _board.PlaceShip(currentShip, _xCoor, _yCoor, dir);
                        CurrentShipIndexForPlacement++;
                        _xCoor = 0; // Reset pozycji dla następnego statku
                        _yCoor = 0;
                        if (CurrentShipIndexForPlacement >= _board.Ships.Count)
                        {
                            PlacementComplete = true;
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
                    PlacementComplete = true;
                    break;
            }
        }

        public bool IsShipPreviewTile(int row, int col)
        {
            if (PlacementComplete || CurrentShipIndexForPlacement >= _board.Ships.Count)
                return false;

            var currentShip = _board.Ships[CurrentShipIndexForPlacement];
            if (currentShip.IsHorizontal)
            {
                return row == _yCoor && col >= _xCoor && col < _xCoor + currentShip.Length;
            }

            // Vertical
            return col == _xCoor && row >= _yCoor && row < _yCoor + currentShip.Length;
        }
    }
}