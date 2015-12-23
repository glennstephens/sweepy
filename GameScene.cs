using System;
using System.Collections.Generic;
using CoreGraphics;
using SpriteKit;
using UIKit;

namespace tvMinesweeper
{
	public class GameScene : SKScene
	{
		public GameScene (IntPtr handle) : base (handle)
		{
			BackgroundColor = UIColor.White;
		}

		MinesweeperMap map;
		int CurrentX = 0;
		int CurrentY = 0;
		nfloat cellWidth;
		nfloat cellHeight;

		Dictionary<int, SKSpriteNode> spritesForLocation = new Dictionary<int, SKSpriteNode>();
		SKSpriteNode currentSpot;
		SKLabelNode gameOverNode;

		SKLabelNode gameOverNode2;

		//const string displayFont = "Pixel-Art Regular"; 
		const string displayFont = "Avenir-Book";
		const string displayFontGameOver = "Avenir-Medium";
		const float displayFontSize = 32f;
		const float displayFontSizeGameOver = 130f;

		public override void DidMoveToView (SKView view)
		{
			BackgroundColor = UIColor.White;

			map = new MinesweeperMap ();
			map.SetupBoard (AllLevels.GetFirstLevel ());

			DisplayMap ();
		}

		void DisplayMap()
		{
			foreach (var item in spritesForLocation) {
				item.Value.RunAction (SKAction.RemoveFromParent ());
			}
			spritesForLocation.Clear ();

			gameOverNode?.RunAction (SKAction.RemoveFromParent ());
			gameOverNode = null;

			nfloat topTextSpace = 80f;

			cellWidth = Scene.Frame.Width / map.MapColumns;
			cellHeight = (Scene.Frame.Height - topTextSpace) / map.MapRows;

			for (var y = 0; y < map.MapRows; y++)
				for (var x = 0; x < map.MapColumns; x++)
				{
					// Add the item in the appropriate position
					var cell = map[x, y];
					SKSpriteNode sprite = GetSpriteForCell (cell);
					AddChild (sprite);
					spritesForLocation [map.LocationForXY (x, y)] = sprite;
				}

			CurrentX = AllLevels.CurrentLevel.StartX;
			CurrentY = AllLevels.CurrentLevel.StartY;

			UpdateCurrentSpot ();

			// Add the text information (Level <number>: <name>)
			if (levelNumber != null) {
				levelNumber.RunAction (SKAction.RemoveFromParent ());
				levelNumber = null;
			}
			if (levelName != null) {
				levelName.RunAction (SKAction.RemoveFromParent ());
				levelName = null;
			}

			levelName = new SKLabelNode (displayFont) {
				Text = map.CurrentLevel.LevelName,
				Position = new CGPoint(GetCellPosition (0, -1).X, Scene.Frame.Height - 33f),
				FontColor = UIColor.Black,
				FontSize = displayFontSize,
				HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Left
			};
			AddChild (levelName);

			levelNumber = new SKLabelNode (displayFont) {
				Text = map.CurrentLevel.LevelName,
				Position = new CGPoint(GetCellPosition (map.MapColumns, -1).X / 2, levelName.Position.Y),
				FontColor = UIColor.Black,
				FontSize = displayFontSize,
				HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Left
			};
			AddChild (levelNumber);

			UpdateLevelDisplay ();
		}

		SKLabelNode levelNumber, levelName;

		void UpdateCurrentSpot()
		{
			currentSpot?.RemoveFromParent ();

			currentSpot = GetSpriteForCell(CurrentX, CurrentY);
			var action = SKAction.RepeatActionForever (
				SKAction.Sequence (
					SKAction.ScaleBy (2f, 0.6),
					SKAction.ScaleBy (0.5f, 0.6)
				));
			currentSpot.RunAction (action);
			AddChild (currentSpot);
		}

		void UpdateLevelDisplay ()
		{
			levelNumber.Text = string.Format ("Flagged: {0}. Mines Left: {1}", 
			                                      map.MinesFlagged, map.MinesLeft);
			
			levelName.Text = map.CurrentLevel.LevelName;
		}

		public void ProcessControllerAction (TVControllerAction item)
		{
			if (map.IsGameOver && item == TVControllerAction.Flag)
				item = TVControllerAction.Tap;
			
			switch (item) {
			case TVControllerAction.Left:
				CurrentX--;
				if (CurrentX < 0)
					CurrentX = 0;
				break;
			case TVControllerAction.Right:
				CurrentX++;
				if (CurrentX >= map.MapColumns)
					CurrentX = map.MapColumns - 1;
				break;
			case TVControllerAction.Up:
				CurrentY--;
				if (CurrentY < 0)
					CurrentY = 0;
				break;
			case TVControllerAction.Down:
				CurrentY++;
				if (CurrentY >= map.MapRows)
					CurrentY = map.MapRows - 1;
				break;
			case TVControllerAction.Flag:
				var flagResults = map.FlagSpot (CurrentX, CurrentY);

				flagResults.AffectedSpots.ForEach (RedisplaySpot);

				if (flagResults.GameReset) {
					DisplayMap ();
				}

				MoveCurrentPositionToTop ();
				UpdateLevelDisplay ();

				break;
			case TVControllerAction.Tap:
				var actionDetails = map.ClickOnSpot (CurrentX, CurrentY);

				actionDetails.AffectedSpots.ForEach (RedisplaySpot);

				if (actionDetails.GameOver) {
					RedisplaySpot (new Spot (CurrentX, CurrentY));

					// Blow up all the spots
					MakeExplosionAtPoint (new Spot (CurrentX, CurrentY));

					var allMines = map.GetAllMinesNear (new Spot (CurrentX, CurrentY));
					allMines.ForEach (MakeExplosionAtPoint);

					gameOverNode = new SKLabelNode (displayFontGameOver) {
						Text = "Game Over",
							Position = new CGPoint(Scene.Frame.GetMidX(), Scene.Frame. GetMidY()),
						FontColor = UIColor.Blue,
						FontSize = displayFontSizeGameOver,
						VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center,
						HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Center
					};
						float miniBuffer = 3f;
					gameOverNode2 = new SKLabelNode (displayFontGameOver) {
						Text = "Game Over",
							Position = new CGPoint(gameOverNode.Position.X + miniBuffer, gameOverNode.Position.Y - miniBuffer),
							FontColor = UIColor.FromRGBA(0.0f, 0f, 0f, 0.25f),
						FontSize = displayFontSizeGameOver,
						VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center,
						HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Center
					};

					AddChild (gameOverNode2);
					AddChild (gameOverNode);

					gameOverNode.RunAction (SKAction.RotateByAngle (
						(nfloat)(Math.PI / 180) * 720, 
						1.0));
					gameOverNode2.RunAction (SKAction.RotateByAngle (
						(nfloat)(Math.PI / 180) * 720, 
						1.0));
					
					currentSpot.RunAction(SKAction.RemoveFromParent());
					currentSpot = null;
				}

				if (actionDetails.GameReset) {
					DisplayMap ();
				}

				MoveCurrentPositionToTop ();
				UpdateLevelDisplay ();

				break;
			}

			UpdateCurrentSpot();

			currentSpot.RunAction (SKAction.MoveTo (
				GetCellPosition (CurrentX, CurrentY), 0.25
			));
		}

		void MakeExplosionAtPoint(Spot s)
		{
			ExplosionNode explode = new ExplosionNode (spritesForLocation [map.LocationForXY (s.X, s.Y)]);
			explode.Position = GetCellPosition (s.X, s.Y);
			AddChild (explode);
		}

		void RedisplaySpot (Spot spot)
		{
			// Clear the information if needed
			var actualIndex = map.LocationForXY(spot.X, spot.Y);
			if (spritesForLocation.ContainsKey (actualIndex)) {
				this.RemoveChildren (new SKNode[] { spritesForLocation[actualIndex] });
				spritesForLocation [actualIndex] = null;
			}

			var newSprite = GetSpriteForCell (map [spot.X, spot.Y]);
			AddChild (newSprite);
			spritesForLocation [actualIndex] = newSprite;
		}
			
		const string UnclickedSpotImageName = "UnclickedSpot";
		const string BombImageName = "bomb";
		const string ClickedSpotImageName = "ClickedSpot";
		const string FlagImageName = "Flag";

		SKSpriteNode GetSpriteForCell(int x, int y)
		{
			return GetSpriteForCell(map[x, y]);
		}

		SKSpriteNode GetSpriteForCell(MineCell cell)
		{
			SKSpriteNode sprite;
			switch (cell.Display) {
			case MineCellDisplayType.Unclicked:
			default:
				sprite = new SKSpriteNode (UnclickedSpotImageName);
				break;
			case MineCellDisplayType.Bomb:
				sprite = new SKSpriteNode (BombImageName);
				break;
			case MineCellDisplayType.Flag:
				sprite = new SKSpriteNode (FlagImageName);
				break;
			case MineCellDisplayType.Number1:
				sprite = new SKSpriteNode ("Spot1");
				break;
			case MineCellDisplayType.Number2:
				sprite = new SKSpriteNode ("Spot2");
				break;
			case MineCellDisplayType.Number3:
				sprite = new SKSpriteNode ("Spot3");
				break;
			case MineCellDisplayType.Number4:
				sprite = new SKSpriteNode ("Spot4");
				break;
			case MineCellDisplayType.Number5:
				sprite = new SKSpriteNode ("Spot5");
				break;
			case MineCellDisplayType.Number6:
				sprite = new SKSpriteNode ("Spot6");
				break;
			case MineCellDisplayType.Number7:
				sprite = new SKSpriteNode ("Spot7");
				break;
			case MineCellDisplayType.Number8:
				sprite = new SKSpriteNode ("Spot8");
				break;
			case MineCellDisplayType.NoSurroundingBombs:
				sprite = new SKSpriteNode (ClickedSpotImageName);
				break;
			}

			sprite.Position = GetCellPosition (cell.X, cell.Y);
			sprite.Size = new CGSize (cellWidth, cellHeight);

			return sprite;
		}

		CGPoint GetCellPosition(int x, int y)
		{
			var height = Scene.Frame.Height;
			var width = Scene.Frame.Width;

			var actualY = (height - (y + 1) * cellHeight) - cellHeight / 2;
			return new CGPoint (x * cellWidth + cellWidth / 2, actualY);
		}

		void MoveCurrentPositionToTop()
		{
			if (currentSpot != null)
			{
				RemoveChildren(new SKNode[] { currentSpot });
				AddChild(currentSpot);
			}
		}

		public override void Update (double currentTime)
		{
			// Called before each frame is rendered
		}
	}
}

