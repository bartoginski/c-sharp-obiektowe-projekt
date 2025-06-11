using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Models.Boards;
using System;
using System.Collections.Generic;

public interface IBot
{
    string Name { get; }
    Tuple<int, int> BotShotSelection();
    void InformShotResult(Tuple<int, int> coord, ShotResult result, List<(int x, int y)> sunkShipCoords = null);
    void BotShipPlacement(Board board);
    void AddCellsToAvoid(List<(int col, int row)> cells);
}