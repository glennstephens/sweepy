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

				if (WasClicked) {
					switch (NearbyCount) {
					case 0: return MineCellDisplayType.NoSurroundingBombs;
					case 1: return MineCellDisplayType.Number1;
					case 2: return MineCellDisplayType.Number2;
					case 3: return MineCellDisplayType.Number3;
					case 4: return MineCellDisplayType.Number4;
					case 5: return MineCellDisplayType.Number5;
					case 6: return MineCellDisplayType.Number6;
					case 7: return MineCellDisplayType.Number7;
					case 8: return MineCellDisplayType.Number8;
					}
				}
				
				return MineCellDisplayType.Unclicked;
			}
		}
	}	
}
