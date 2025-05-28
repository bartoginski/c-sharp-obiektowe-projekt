using System;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Enums; // Required for ShotResult

namespace ProgramowanieObiektoweProjekt.Interfaces
{
    internal interface IBot
    {
        string Name { get; }
        Tuple<int, int> BotShotSelection();
        void BotShipPlacement(Board board);
        void InformShotResult(Tuple<int, int> shotCoordinates, ShotResult result);
    }
}