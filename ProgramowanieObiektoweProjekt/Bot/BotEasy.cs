using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Utils;
using System; // Dla Random, Tuple
using System.Collections.Generic; // Dla List, Queue
using System.Linq; // Dla .Any()

// Zakładam, że IBot jest zdefiniowane w tym samym namespace lub jest odpowiedni using
// np. using ProgramowanieObiektoweProjekt.Interfaces;

internal class BotEasy : IBot
{
    protected const int BoardSize = Constants.BoardSize;
    protected Random _rand = new();
    // Lista _shotsMade powinna być dostępna dla Menu.cs, aby dodać pola wokół zatopionego statku gracza
    // Można ją uczynić 'internal' lub dodać publiczną metodę do dodawania pól.
    // Dla uproszczenia, zostawiam protected, ale Menu.cs nie będzie mogło bezpośrednio jej modyfikować.
    // W Menu.cs można by przekazać listę tych pól do specjalnej metody w IBot.
    // np. void AddAvoidCells(List<(int col, int row)> cells);
    // Jeśli Menu.cs ma aktualizować _shotsMade, to musi być ono `internal` lub `public`.
    // Zmieniam na `internal` dla tego przykładu.
    internal List<(int x, int y)> _shotsMade = new(); // x to kolumna, y to wiersz
    protected Queue<(int x, int y)> _huntQueue = new();
    protected bool _huntingMode = false;
    protected List<(int x, int y)> _hits = new(); // Przechowuje trafienia w aktualnie polowany statek

    public virtual string Name => "EasyBot"; // Zmieniono nazwę, "Easy" jest zbyt ogólne

    public virtual Tuple<int, int> BotShotSelection()
    {
        if (_huntingMode && _huntQueue.Any())
        {
            while (_huntQueue.Any())
            {
                var target = _huntQueue.Dequeue();
                if (!_shotsMade.Contains(target)) // Upewnij się, że to pole nie było już strzelane
                {
                    _shotsMade.Add(target); 
                    return Tuple.Create(target.x, target.y);
                }
            }
            // Jeśli kolejka polowania jest pusta, ale nadal jesteśmy w trybie polowania
            // (np. wszystkie bezpośrednie sąsiedztwa zostały ostrzelane lub były częścią zatopionego statku)
            // spróbuj odbudować kolejkę z pozostałych trafień (jeśli są) lub wyjdź z trybu polowania
            RePopulateHuntQueueFromHits(); 
            if (_huntQueue.Any()) {
                // Ponownie spróbuj pobrać z kolejki po repopulacji
                 while (_huntQueue.Any()) {
                    var target = _huntQueue.Dequeue();
                    if (!_shotsMade.Contains(target)) {
                        _shotsMade.Add(target);
                        return Tuple.Create(target.x, target.y);
                    }
                }
            }
            // Jeśli nadal nic, wyłącz tryb polowania
            _huntingMode = false; 
        }

        // Tryb losowego strzelania
        while (true)
        {
            int x = _rand.Next(0, BoardSize); // X to kolumna
            int y = _rand.Next(0, BoardSize); // Y to wiersz

            if (!_shotsMade.Contains((x, y)))
            {
                _shotsMade.Add((x, y));
                return Tuple.Create(x, y);
            }
        }
    }

    public virtual void BotShipPlacement(Board board)
    {
        foreach (var ship in board.ships) // Zakładamy, że board.ships to List<ShipBase>
        {
            bool placed = false;
            while (!placed)
            {
                Direction dir = _rand.Next(2) == 0 ? Direction.Horizontal : Direction.Vertical;
                ship.IsHorizontal = (dir == Direction.Horizontal); 

                int x, y; 

                if (dir == Direction.Horizontal)
                {
                    x = _rand.Next(0, BoardSize - ship.Length + 1); 
                    y = _rand.Next(0, BoardSize); 
                }
                else // Vertical
                {
                    x = _rand.Next(0, BoardSize); 
                    y = _rand.Next(0, BoardSize - ship.Length + 1); 
                }

                if (board.IsValidPlacement(ship, x, y, dir))
                {
                    board.PlaceShip(ship, x, y, dir);
                    placed = true;
                }
            }
        }
    }

    public virtual void InformShotResult(Tuple<int, int> coord, ShotResult result)
    {
        // coord to (kolumna, wiersz)
        if (result == ShotResult.Hit)
        {
            _huntingMode = true;
            if (!_hits.Contains((coord.Item1, coord.Item2)))
            {
                _hits.Add((coord.Item1, coord.Item2));
            }
            EnqueueAdjacent((coord.Item1, coord.Item2));
        }
        else if (result == ShotResult.Sunk)
        {
            // Po zatopieniu, plansza przeciwnika (gracza) jest aktualizowana przez Board.MarkAroundSunkShip.
            // Bot powinien wyczyścić stan polowania na ten konkretny statek.
            _hits.Clear(); 
            _huntQueue.Clear(); 
            _huntingMode = false; 
            // Pętla gry w Menu.cs może dodatkowo poinformować bota o polach wokół zatopionego statku,
            // aby dodał je do _shotsMade, jeśli IBot miałby metodę np. AddCellsToAvoid().
            // Na razie bot dowie się, że te pola są "IsHit", jeśli spróbuje w nie strzelić
            // i jego pętla w BotShotSelection pominie je, bo są już w _shotsMade (jeśli zostały tam dodane).
        }
        else if (result == ShotResult.Miss)
        {
            // Jeśli spudłowaliśmy w trybie polowania, to może oznaczać, że wybrany kierunek był zły.
            // Obecna logika BotShotSelection (opróżnianie _huntQueue i ew. repopulacja) powinna to obsłużyć.
        }
    }

    protected void EnqueueAdjacent((int x, int y) coord) // x to kolumna, y to wiersz
    {
        int col = coord.x;
        int row = coord.y;

        var adj = new List<(int x, int y)>
        {
            (col, row - 1), // Up
            (col, row + 1), // Down
            (col - 1, row), // Left
            (col + 1, row)  // Right
        };

        foreach (var t in adj)
        {
            if (IsInBounds(t) && !_shotsMade.Contains(t) && !_huntQueue.Contains(t))
            {
                _huntQueue.Enqueue(t);
            }
        }
    }

    protected void RePopulateHuntQueueFromHits()
    {
        _huntQueue.Clear(); 
        foreach (var h_coord in _hits) // h_coord to (kolumna, wiersz)
        {
            EnqueueAdjacent(h_coord); 
        }
    }

    protected bool IsInBounds((int x, int y) coord) // x to kolumna, y to wiersz
    {
        return coord.x >= 0 && coord.x < BoardSize &&
               coord.y >= 0 && coord.y < BoardSize;
    }

    // Opcjonalna metoda, jeśli Menu.cs ma informować bota o polach do unikania
    public void AddCellsToAvoid(List<(int col, int row)> cells)
    {
        foreach(var cell in cells)
        {
            if (!_shotsMade.Contains(cell))
            {
                _shotsMade.Add(cell);
            }
        }
    }
}