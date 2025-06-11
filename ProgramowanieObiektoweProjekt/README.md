# Dokumentacja Techniczna - Battleship Game

## Spis treści
1. [Przegląd projektu](#przegląd-projektu)
2. [Architektura systemu](#architektura-systemu)
3. [Interfejsy](#interfejsy)
4. [Modele danych](#modele-danych)
5. [System botów](#system-botów)
6. [Mechanizmy gry](#mechanizmy-gry)
7. [Interfejs użytkownika](#interfejs-użytkownika)
8. [Konfiguracja](#konfiguracja)

## Przegląd projektu

Battleship Game to implementacja klasycznej gry w statki napisana w C# z wykorzystaniem biblioteki Spectre.Console do tworzenia interfejsu terminala.

### Technologie
- **.NET 8.0**
- **Spectre.Console** (v0.50.1-preview) - interfejs użytkownika w terminalu

## Architektura systemu

Projekt jest zorganizowany w strukturę folderów odzwierciedlającą architekturę warstwową:

```
ProgramowanieObiektoweProjekt/
├── Bot/                # Implementacje sztucznej inteligencji
├── Enums/              # Wyliczenia używane w grze
├── Interfaces/         # Definicje interfejsów
├── Models/             # Modele danych i logika biznesowa
│   ├── Boards/         # Plansze i elementy UI
│   ├── Menu/           # System menu
│   ├── Player/         # Implementacje graczy
│   └── Ships/          # Typy statków
├── Utils/              # Narzędzia i stałe
└── Program.cs          # Punkt wejścia aplikacji
```

## Interfejsy

### IBoard
**Lokalizacja:** `Interfaces/IBoard.cs`

Definiuje interfejs dla plansz gry.

```csharp
internal interface IBoard
{
    void PlaceShip(IShip ship, int x, int y, Direction direction);
    ShotResult Shoot(int x, int y);
    void DisplayBoard(bool revealShips, KeyControl keyControl);
}
```

**Główne metody:**
- `PlaceShip()` - umieszcza statek na planszy
- `Shoot()` - wykonuje strzał w określone współrzędne
- `DisplayBoard()` - renderuje planszę w terminalu

### IBot
**Lokalizacja:** `Interfaces/IBot.cs`

Interfejs dla implementacji sztucznej inteligencji.

```csharp
internal interface IBot
{
    string Name { get; }
    Tuple<int, int> BotShotSelection();
    void BotShipPlacement(Board board);
    void InformShotResult(Tuple<int, int> shotCoordinates, ShotResult result);
    void AddCellsToAvoid(List<(int col, int row)> cells);
}
```

**Kluczowe funkcjonalności:**
- `BotShotSelection()` - algorytm wyboru celu
- `BotShipPlacement()` - automatyczne rozmieszczanie statków
- `InformShotResult()` - informowanie o wyniku strzału
- `AddCellsToAvoid()` - dodawanie pól do unikania

### IPlayer
**Lokalizacja:** `Interfaces/IPlayer.cs`

Reprezentuje gracza.

```csharp
internal interface IPlayer
{
    int Points { get; }
    string Name { get; }
    Board Board { get; }
    List<(int x, int y, ShotResult result)> MoveHistory { get; }
}
```

### IShip
**Lokalizacja:** `Interfaces/IShip.cs`

Podstawowy interfejs dla wszystkich typów statków.

```csharp
internal interface IShip
{
    int Length { get; }
    bool IsSunk { get; }
    void Hit();
}
```

## Modele danych

### Board
**Lokalizacja:** `Models/Boards/Board.cs`

Główna implementacja planszy gry.

**Kluczowe właściwości:**
- `Tile[,] tiles` - dwuwymiarowa tablica pól planszy (10x10)
- `List<ShipBase> ships` - lista statków na planszy

**Typy statków:**
- 1x Battleship (długość 4)
- 2x Cruiser (długość 3)
- 3x Destroyer (długość 2)
- 4x Submarine (długość 1)

**Główne metody:**
- `PlaceShip()` - umieszcza statek z walidacją kolizji
- `Shoot()` - obsługuje logikę strzału
- `MarkAroundSunkShip()` - oznacza pola wokół zatopionych statków
- `IsValidPlacement()` - sprawdza czy umieszczenie statku jest legalne
- `GetBoardRenderable()` - tworzy reprezentację planszy dla Spectre.Console

### Tile
**Lokalizacja:** `Models/Boards/Tile.cs`

Reprezentuje pojedyncze pole na planszy.

```csharp
internal class Tile
{
    public ShipBase? OccupyingShip { get; set; }
    public bool IsHit { get; set; }
    public bool HasShip => OccupyingShip != null;
}
```

### ShipBase
**Lokalizacja:** `Models/Ships/ShipBase.cs`

Abstrakcyjna klasa bazowa dla wszystkich statków.

**Właściwości:**
- `Name` - nazwa typu statku
- `Length` - długość statku
- `IsHorizontal` - orientacja (pozioma/pionowa)
- `IsSunk` - czy statek jest zatopiony
- `OccupiedTilesList` - lista zajmowanych pól

**Implementacje statków:** `Models/Ships/Ships.cs`
- `BattleShip` - pancernik (4 pola)
- `Cruiser` - krążownik (3 pola)
- `Destroyer` - niszczyciel (2 pola)
- `Submarine` - łódź podwodna (1 pole)

## System botów

### BotEasy
**Lokalizacja:** `Bot/BotEasy.cs`

Podstawowa implementacja AI z trybem polowania.

**Algorytm:**
1. **Tryb losowy:** Strzela w losowe, nieostrzelane pola
2. **Tryb polowania:** Po trafieniu przechodzi w tryb systematycznego poszukiwania
3. **Polowanie kierunkowe:** Próbuje określić orientację statku i strzelać wzdłuż osi

**Kluczowe zmienne:**
- `_huntingMode` - czy bot jest w trybie polowania
- `_huntOrigin` - punkt początkowy polowania
- `_huntDirection` - kierunek polowania ("vertical", "horizontal", "unknown")

### BotMedium
**Lokalizacja:** `Bot/BotMedium.cs`

Rozszerza BotEasy o strategię sektorową.

**Strategia:**
1. Dzieli planszę na 4 sektory (TL, TR, BL, BR)
2. Wykonuje fazową eksplorację sektorów:
    - Faza 1: 25% pokrycia każdego sektora
    - Faza 2: 50% pokrycia każdego sektora
    - Faza 3: 80% pokrycia każdego sektora
    - Faza 4: Losowe strzały

### BotHard
**Lokalizacja:** `Bot/BotHard.cs`

Najbardziej zaawansowany bot wykorzystujący strategię diagonalną.

**Strategia:**
1. Dziedziczy tryb polowania z BotMedium
2. Implementuje strzały diagonalne w sektorach
3. Pokrywa do 80% pól diagonalnych w każdym sektorze
4. Optymalizuje prawdopodobieństwo trafienia większych statków

## Mechanizmy gry

### KeyControl
**Lokalizacja:** `Models/Boards/KeyControl.cs`

System obsługi klawiatury dla rozmieszczania statków i ogólnego systemu nawigacji.

**Funkcjonalności:**
- Poruszanie kursorem (strzałki)
- Obracanie statku (spacja)
- Umieszczanie statku (Enter)
- Walidacja pozycji w czasie rzeczywistym

**Kontrola stanu:**
- `currentShipIndexForPlacement` - indeks aktualnie umieszczanego statku
- `placementComplete` - czy rozmieszczanie zostało zakończone

### HistoryTab
**Lokalizacja:** `Models/Boards/HistoryTab.cs`

System śledzenia statystyk gry. **(Work in progress)**

```csharp
public class HistoryTab
{
    public int ShotsFired { get; private set; }
    public int Hits { get; private set; }
    public int Misses => ShotsFired - Hits;
}
```

### BoardLayout
**Lokalizacja:** `Models/Boards/BoardLayout.cs`

System renderowania układu plansz w terminalu.

**Układ:**
- Lewa kolumna: plansze gracza i przeciwnika
- Prawa kolumna: historia i instrukcja
- Dynamiczne podświetlanie kursora

## Interfejs użytkownika

### Menu System
**Lokalizacja:** `Models/Menu/Menu.cs`

Główny system menu z opcjami:
- **Nowa gra** - rozpoczyna rozgrywkę z wyborem poziomu trudności
- **Historia gier** - (placeholder)
- **Autorzy** - informacje o twórcach
- **Wyjście** - zamknięcie aplikacji

### Mechanizm gry
**Funkcja:** `StartGame()`

**Fazy gry:**
1. **Wybór poziomu trudności bota**
2. **Rozmieszczanie statków gracza** (interaktywne)
3. **Automatyczne rozmieszczanie statków bota**
4. **Pętla rozgrywki:**
    - Tura gracza (kontrola kursorem)
    - Tura bota (automatyczna)
    - Sprawdzenie warunków zwycięstwa

### Renderowanie
System wykorzystuje Spectre.Console do tworzenia kolorowego, interaktywnego interfejsu:

- **Kolory stanu pól:**
    - `[deepskyblue1]░[/]` - woda
    - `[red]X[/]` - trafiony statek
    - `[blue]M[/]` - pudło
    - `[yellow]O[/]` - podgląd umieszczania statku

## Enumy

### Direction
**Lokalizacja:** `Enums/Direction.cs`
```csharp
internal enum Direction
{
    Horizontal,
    Vertical, 
}
```

### ShotResult
**Lokalizacja:** `Enums/ShotResult.cs`
```csharp
internal enum ShotResult
{
    Miss,    // Pudło
    Hit,     // Trafienie
    Sunk     // Zatopienie
}
```

## Konfiguracja

### Constants
**Lokalizacja:** `Utils/Constants.cs`

```csharp
public static class Constants
{
    public const int BoardSize = 10;      // Rozmiar planszy (10x10)
    public const bool DevMode = false;    // Tryb deweloperski (pokazuje statki przeciwnika)
}
```

### Zależności projektu
**Plik:** `ProgramowanieObiektoweProjekt.csproj`

- **.NET 8.0** - framework docelowy
- **Spectre.Console 0.50.1-preview** - biblioteka UI terminala
- **ImplicitUsings** - włączone automatyczne using
- **Nullable** - włączona obsługa nullable reference types

## Wzorce projektowe

### Strategy Pattern
- Różne implementacje botów (`IBot`)
- Wymienne algorytmy AI

### Template Method Pattern
- `ShipBase` jako szablon dla konkretnych typów statków
- `BotEasy` jako baza dla bardziej zaawansowanych botów

### Observer Pattern
- `InformShotResult()` - powiadamianie botów o wyniku strzału
- `HistoryTab` - śledzenie statystyk gry

### Factory Pattern (implied)
- Tworzenie różnych typów statków
- Instancjonowanie botów na podstawie wyboru trudności

## Kluczowe features

### 1. Inteligentny system AI
- **3 poziomy trudności** z różnymi strategiami
- **Tryb polowania** - systematyczne poszukiwanie po trafieniu
- **Strategia sektorowa** - podział planszy na obszary
- **Optymalizacja diagonalna** - preferowanie pól o wyższym prawdopodobieństwie

### 2. Interaktywny interfejs
- **Kolorowy terminal** z wykorzystaniem Spectre.Console
- **Kontrola kursorem** podczas rozmieszczania i strzelania
- **Podgląd w czasie rzeczywistym** podczas umieszczania statków
- **Walidacja pozycji** z wizualną informacją zwrotną

### 3. Kompletny system gry
- **Automatyczne oznaczanie** pól wokół zatopionych statków
- **Historia strzałów** z podstawowymi statystykami
- **Warunki zwycięstwa** z odpowiednimi komunikatami
- **Przewidywanie kolizji** statków z regułą 1 pola odstępu

### 4. Architektura modularna
- **Separacja logiki** od prezentacji
- **Łatwa rozszerzalność** - dodawanie nowych typów botów
- **Testowalna struktura** z jasno zdefiniowanymi interfejsami
- **Konfigurowalność** przez system stałych

## Możliwe rozszerzenia

1. **Multiplayer online** - implementacja `IPlayer` dla graczy sieciowych
2. **Zapisywanie gier** - rozszerzenie `HistoryTab` o persistencję danych
3. **Nowe typy statków** - implementacja `IShip` dla specjalnych jednostek
4. **Zaawansowane AI** - nowe strategie w systemie botów
   5. AI oparte o zewnętrzny model.
5. **Tryby gry** - różne warianty zasad i rozmiarów plansz

## Obsługa błędów

System implementuje podstawową obsługę błędów:
- **Walidacja granic planszy** w metodach strzału i umieszczania
- **Sprawdzanie kolizji** podczas rozmieszczania statków
- **Obsługa duplikatów strzałów** z informacją dla gracza
- **Bezpieczne rzutowanie** typów w krytycznych miejscach

---