using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;

internal interface IBot
{
    string Name { get; }
    Tuple<int, int> BotShotSelection();
    void BotShipPlacement(Board board); // Board to konkretna klasa, nie interfejs IBoard
    void InformShotResult(Tuple<int, int> shotCoordinates, ShotResult result);
    void AddCellsToAvoid(List<(int col, int row)> cells); // <-- DODAJ TĘ LINIĘ
}
