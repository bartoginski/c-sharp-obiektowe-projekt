using System;
using System.Collections.Generic;
using System.Linq;
using ProgramowanieObiektoweProjekt.Models.Boards;
using ProgramowanieObiektoweProjekt.Models.Ships;
using ProgramowanieObiektoweProjekt.Enums;
using ProgramowanieObiektoweProjekt.Bot.BotEasy;
using ProgramowanieObiektoweProjekt.Interfaces;

namespace ProgramowanieObiektoweProjekt.Bot.BotMedium
{
	class BotMedium : BotEasy.BotEasy, IBot
	{
		public override string Name { get; } = "Medium";

		private List<Tuple<int, int>> _currentSectorShots = new List<Tuple<int, int>>();
		private int _currentSectorIndex = -1;
		private List<List<Tuple<int, int>>> _sectors;

		public BotMedium()
		{
			InitializeSectors();
			SelectRandomSector();
		}

		private void InitializeSectors()
		{
			_sectors = new List<List<Tuple<int, int>>>();

			// Sector 1 (Top-Left)
			_sectors.Add(GetSectorTiles(0, 4, 0, 4));
			// Sector 2 (Top-Right)
			_sectors.Add(GetSectorTiles(0, 4, 5, 9));
			// Sector 3 (Bottom-Left)
			_sectors.Add(GetSectorTiles(5, 9, 0, 4));
			// Sector 4 (Bottom-Right)
			_sectors.Add(GetSectorTiles(5, 9, 5, 9));
		}

		private List<Tuple<int, int>> GetSectorTiles(int rowStart, int rowEnd, int colStart, int colEnd)
		{
			List<Tuple<int, int>> tiles = new List<Tuple<int, int>>();
			for (int r = rowStart; r <= rowEnd; r++)
			{
				for (int c = colStart; c <= colEnd; c++)
				{
					tiles.Add(Tuple.Create(r, c));
				}
			}
			return tiles;
		}

		private void SelectRandomSector()
		{
			List<int> availableSectors = new List<int>();
			for (int i = 0; i < _sectors.Count; i++)
			{
				int shotsInSector = _shotsMade.Count(shot => _sectors[i].Contains(shot));
				if (shotsInSector < _sectors[i].Count * 0.8)
				{
					availableSectors.Add(i);
				}
			}

			if (availableSectors.Any())
			{
				_currentSectorIndex = availableSectors[_rand.Next(availableSectors.Count)];
				_currentSectorShots.Clear();
				foreach (var tile in _sectors[_currentSectorIndex])
				{
					if (!_shotsMade.Contains(tile))
					{
						_currentSectorShots.Add(tile);
					}
				}
				_currentSectorShots = _currentSectorShots.OrderBy(x => _rand.Next()).ToList();
			}
			else
			{
				_currentSectorIndex = -1;
			}
		}

		public override Tuple<int, int> BotShotSelection()
		{
			// Priorytet dla trybu polowania (dziedziczone z BotEasy)
			if (_huntingMode && _huntQueue.Any())
			{
				while (_huntQueue.Any())
				{
					Tuple<int, int> target = _huntQueue.Dequeue();
					if (!_shotsMade.Contains(target))
					{
						return target;
					}
				}
				// Je�li _huntQueue jest wyczerpana, ale nadal jeste�my w trybie polowania
				RePopulateHuntQueueFromHits();
				if (_huntQueue.Any())
				{
					return _huntQueue.Dequeue();
				}
				else
				{
					_huntingMode = false;
				}
			}

			// Oryginalna logika BotMedium (strzelanie sektorowe)
			if (_currentSectorIndex != -1 && _currentSectorShots.Any())
			{
				int totalSectorTiles = _sectors[_currentSectorIndex].Count;
				int eightyPercentThreshold = (int)Math.Ceiling(totalSectorTiles * 0.8);

				int shotsMadeInCurrentSector = _shotsMade.Count(shot => _sectors[_currentSectorIndex].Contains(shot));

				if (shotsMadeInCurrentSector < eightyPercentThreshold)
				{
					Tuple<int, int> shot = _currentSectorShots.FirstOrDefault();
					if (shot != null)
					{
						_currentSectorShots.Remove(shot);
						return shot; // Zwr��, nie dodaj�c do _shotsMade
					}
				}
			}

			SelectRandomSector();

			if (_currentSectorIndex != -1 && _currentSectorShots.Any())
			{
				Tuple<int, int> shot = _currentSectorShots.FirstOrDefault();
				if (shot != null)
				{
					_currentSectorShots.Remove(shot);
					return shot; // Zwr��, nie dodaj�c do _shotsMade
				}
			}

			// Fallback do logiki BotEasy (strzelanie losowe), je�li logika sektorowa r�wnie� zawiedzie
			return base.BotShotSelection();
		}

		public override void BotShipPlacement(Board board)
		{
			base.BotShipPlacement(board);
		}

		public override void InformShotResult(Tuple<int, int> shotCoordinates, ShotResult result)
		{
			base.InformShotResult(shotCoordinates, result); // Wywo�aj logik� z klasy bazowej (BotEasy)
															// Tutaj mo�na doda� specyficzn� dla BotMedium logik� przetwarzania wyniku, je�li potrzebna
		}
	}
}