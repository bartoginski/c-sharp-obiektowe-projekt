using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;

internal interface IBot
{
    string Name { get; }
    Tuple<int, int> BotShotSelection();
    void BotShipPlacement(Board board);
    void InformShotResult(Tuple<int, int> shotCoordinates, ShotResult result);
}
