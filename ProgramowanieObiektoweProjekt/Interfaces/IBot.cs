using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;

internal interface IBot
{
    string Name { get; }
    Tuple<int, int> BotShotSelection();
    void BotShipPlacement(Board board); // Board is concrete class, not IBoard interface
    void InformShotResult(Tuple<int, int> shotCoordinates, ShotResult result);
    void AddCellsToAvoid(List<(int col, int row)> cells); // Add cells to avoid shooting
}