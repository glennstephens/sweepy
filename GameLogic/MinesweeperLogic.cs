using System;
using System.Linq;
using System.Collections.Generic;

namespace tvMinesweeper
{
	public class MinesweeperMap
	{
		public List<MineCell> Map = new List<MineCell> ();
		public List<MineCell> AllMines = new List<MineCell> ();
		public LevelDetails CurrentLevel;

		public bool IsGameOver {
			get;
			set;
		} = false;

		public int MapRows {
			get;
			set;
		} = 23;

		public int MapColumns {
			get;
			set;
		} = 34;

		public int NumberOfSpots {
			get;
			set;
		} = 75;

		public bool IsValidPoint (int x, int y)
		{
			return x >= 0 && y >= 0 && x < MapColumns && y < MapRows;
		}

		public int LocationForXY (int x, int y)
		{
			return y * MapColumns + x;
		}

		public MineCell this [int x, int y] {
			get {
				return Map [LocationForXY(x, y)];
			}
		}

		public List<MineCell> GetItemsAroundSpot (int x, int y)
		{
			var result = new List<MineCell> ();

			for (int xCounter = -1; xCounter <= 1; xCounter++)
				for (int yCounter = -1; yCounter <= 1; yCounter++) {
					if (xCounter == 0 && yCounter == 0)
						continue;

					if (!IsValidPoint (x + xCounter, y + yCounter))
						continue;

					result.Add (this [x + xCounter, y + yCounter]);
				}

			return result;
		}

		public int MineCountAroundSpot (int x, int y)
		{
			return GetItemsAroundSpot (x, y).Count (cell => cell.HasItem);
		}

		List<int> visitedIndex = new List<int> ();

		void MarkAsVisited (int x, int y)
		{
			var spot = LocationForXY (x, y);
			if (!visitedIndex.Contains (spot))
				visitedIndex.Add (spot);
		}

		MineCell FindEmptySpot (Random r)
		{
			bool hasFound = false;
			int x = 0;
			int y = 0;
			while (!hasFound) {
				x = r.Next (MapColumns);
				y = r.Next (MapRows);

				var cell = this [x, y];
				hasFound = cell.HasItem == false;
			}
			return this [x, y];
		}

		public void SetupBoard (LevelDetails level)
		{
			CurrentLevel = level;

			MapRows = level.Rows;
			MapColumns = level.Columns;
			NumberOfSpots = level.MineCount;

			IsGameOver = false;

			Map = new List<MineCell> ();
			visitedIndex.Clear ();

			for (int y = 0; y < MapRows; y++)
				for (int x = 0; x < MapColumns; x++)
					Map.Add (new MineCell () {
						HasItem = false,
						IsFlagged = false,
						NearbyCount = 0,
						WasClicked = false,
						X = x,
						Y = y
					});

			var r = new Random ();

			for (var counter = 0; counter < this.NumberOfSpots; counter++) {
				var spot = FindEmptySpot (r);
				spot.HasItem = true;
				spot.IsFlagged = false;
				spot.WasClicked = false;

				AllMines.Add (spot);
			}

			// Find the nearest amount of items for the mine
			for (var x = 0; x < MapColumns; x++)
				for (var y = 0; y < MapRows; y++) {
					var cell = this [x, y];
					cell.NearbyCount = MineCountAroundSpot (x, y);
				}
		}

		void ClearSpacesAroundSpot (Spot spot, List<Spot> affectedSpots, List<int> visitedIndexes)
		{
			var items = GetItemsAroundSpot (spot.X, spot.Y);
			items.ForEach (c => {
				var actualIndex = LocationForXY (c.X, c.Y);
				if (!c.HasItem)
				{
					if (!visitedIndexes.Contains (actualIndex)) {
						if (c.NearbyCount >= 0) {
							c.WasClicked = true;
							visitedIndexes.Add (actualIndex);
							var s = new Spot (c.X, c.Y);
							affectedSpots.Add (s);
							ClearSpacesAroundSpot (s, affectedSpots, visitedIndexes);
						} else {
							visitedIndexes.Add (actualIndex);
						}
					}
				}
			});
		}

		public List<Spot> GetAllMinesNear(Spot s)
		{
			AllMines.Sort (
				(MineCell c1, MineCell c2) => Math.Abs (c1.X - c2.X) + Math.Abs (c1.Y - c2.Y)
			);

			return AllMines.Select (c => new Spot (c.X, c.Y)).Take (3).ToList ();
		}

		public ClickResult FlagSpot(int x, int y)
		{
			var result = new ClickResult ();

			var spot = new Spot (x, y);

			var cell = this [x, y];

			if (!cell.IsFlagged) {
				cell.WasClicked = true;
				cell.IsFlagged = true;
			} else {
				cell.WasClicked = false;
				cell.IsFlagged = false;
			}

			result.AffectedSpots.Add (spot);

			return result;
		}
			
		public ClickResult ClickOnSpot (int x, int y)
		{
			var result = new ClickResult ();

			if (IsGameOver) {
				// Recreate the level
				SetupBoard(CurrentLevel);

				result.GameOver = false;
				result.GameReset = true;
			} else {
				var spot = new Spot (x, y);

				var cell = this [x, y];
				cell.WasClicked = true;

				if (cell.HasItem) {
					IsGameOver = true;

					result.BombSpot = spot;
					result.GameOver = true;
					AllMines.ForEach (c => {
						c.WasClicked = true;
					});

					result.AffectedSpots.AddRange (
						AllMines.Select (c => new Spot (c.X, c.Y)));

					return result;
				} 

				result.AffectedSpots.Add (spot);

				var visitList = new List<int> ();
				if (cell.NearbyCount == 0)
					ClearSpacesAroundSpot (spot, result.AffectedSpots, visitList);
			}

			return result;
		}
	}
}

