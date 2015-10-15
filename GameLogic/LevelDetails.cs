using System;

namespace tvMinesweeper
{
	public class LevelDetails
	{
		public int Rows {
			get;
			set;
		}

		public int Columns {
			get;
			set;
		}

		public int MineCount {
			get;
			set;
		}

		public string LevelName {
			get;
			set;
		}

		public int CompletionTimeInSeconds {
			get;
			set;
		}

		public int StartX {
			get;
			set;
		} = 0;

		public int StartY {
			get;
			set;
		} = 0;
	}
}

