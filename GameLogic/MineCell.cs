using System;
using System.Linq;
using System.Collections.Generic;

namespace tvMinesweeper
{
	public class MineCell
	{
		public int X = 0;
		public int Y = 0;

		public bool HasItem = false;
		public bool WasClicked = false;
		public bool IsFlagged = false;
		public int NearbyCount = 0;

		public MineCellDisplayType Display
		{
			get {
				if (IsFlagged)
					return MineCellDisplayType.Flag;
				
				if (WasClicked && HasItem)
					return MineCellDisplayType.Bomb;

				if (WasClicked)
					return MineCellDisplayType.Numbers;
				
				return MineCellDisplayType.Unclicked;
			}
		}
	}	
}
