using System;
using System.Linq;
using System.Collections.Generic;

namespace tvMinesweeper
{
	public class ClickResult
	{
		public bool GameOver {
			get;
			set;
		}

		public bool GameReset {
			get;
			set;
		} = false;

		public List<Spot> AffectedSpots = new List<Spot> ();

		public Spot BombSpot = null;
	}
}
