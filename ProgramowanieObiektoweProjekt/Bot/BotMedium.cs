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

			_sectors.Add(GetSectorTiles(0, 4, 0, 4));
			_sectors.Add(GetSectorTiles(0, 4, 5, 9));
			_sectors.Add(GetSectorTiles(5, 9, 0, 4));
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
						return shot;
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
					return shot;
				}
			}

			return base.BotShotSelection();
		}

		public override void BotShipPlacement(Board board)
		{
			base.BotShipPlacement(board);
		}

		public override void InformShotResult(Tuple<int, int> shotCoordinates, ShotResult result)
		{
			base.InformShotResult(shotCoordinates, result); 
		}
	}
}