using System;

namespace tvMinesweeper
{
	public static class AllLevels
	{
		static int CurrentLevelIndex = 0;

		public static LevelDetails CurrentLevel {
			get {
				return Levels [CurrentLevelIndex];	
			}
		}

		public static LevelDetails[] Levels = new [] {
			new LevelDetails() { Columns = 8, Rows = 8, MineCount = 20, LevelNumber = 1, LevelName = "Old School Beginner Level", CompletionTimeInSeconds = 300 },
			new LevelDetails() { Columns = 16, Rows = 16, MineCount = 40, LevelNumber = 2, LevelName = "Old School Intermediate Level", CompletionTimeInSeconds = 450 },
			new LevelDetails() { Columns = 24, Rows = 24, MineCount = 99, LevelNumber = 3, LevelName = "Old School Advanced Level", CompletionTimeInSeconds = 450 },
			new LevelDetails() { Columns = 34, Rows = 23, MineCount = 75, LevelNumber = 4, LevelName = "Final Level for the TV", CompletionTimeInSeconds = 550 },
			new LevelDetails() { Columns = 45, Rows = 31, MineCount = 350, LevelNumber = 5, LevelName = "Uber hard one", CompletionTimeInSeconds = 550 },
		};

		public static LevelDetails GetFirstLevel()
		{
			return Levels [0];
		}

		public static LevelDetails GetNextLevel()
		{
			CurrentLevelIndex++;
			if (CurrentLevelIndex >= Levels.Length) {
				CurrentLevelIndex = 0;
			}
			return Levels [CurrentLevelIndex];
		}
	}
}
