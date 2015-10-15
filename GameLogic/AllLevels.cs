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

		public static LevelDetails[] Levels = new LevelDetails[] {
			new LevelDetails() { Columns = 10, Rows = 10, MineCount = 20, LevelName = "Warming Up", CompletionTimeInSeconds = 300 },
			new LevelDetails() { Columns = 34, Rows = 23, MineCount = 75, LevelName = "Final Level", CompletionTimeInSeconds = 600 },
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
