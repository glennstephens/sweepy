using System;
using Foundation;
using GameController;
using SpriteKit;
using UIKit;

namespace tvMinesweeper
{
	public delegate void HandleController(TVControllerAction action);

	public partial class GameViewController : UIViewController
	{
		public GameViewController (IntPtr handle) : base (handle)
		{
		}

		public void FireAction(TVControllerAction item)
		{
			Console.WriteLine ("Remote Control Action: " + item);
			scene.ProcessControllerAction (item);
		}

		GameScene scene;

		DateTime LastFlagTime = DateTime.MinValue;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.AddGestureRecognizer (new UISwipeGestureRecognizer (() => FireAction (TVControllerAction.Left)) {
				Direction = UISwipeGestureRecognizerDirection.Left
			});
			View.AddGestureRecognizer (new UISwipeGestureRecognizer (() => FireAction (TVControllerAction.Right)) {
				Direction = UISwipeGestureRecognizerDirection.Right
			});
			View.AddGestureRecognizer (new UISwipeGestureRecognizer (() => FireAction (TVControllerAction.Up)) {
				Direction = UISwipeGestureRecognizerDirection.Up
			});
			View.AddGestureRecognizer (new UISwipeGestureRecognizer (() => FireAction (TVControllerAction.Down)) {
				Direction = UISwipeGestureRecognizerDirection.Down
			});
			View.AddGestureRecognizer (new UITapGestureRecognizer (() => {
				// Trying to avoid the press after the long press
				if ((DateTime.Now - LastFlagTime).TotalMilliseconds > 100)
					FireAction (TVControllerAction.Tap);
			})
				{
					AllowedPressTypes = new NSNumber[] { NSNumber.FromLong((nint)Convert.ToInt32 (UIPressType.Select)) },
					AllowedTouchTypes = new NSNumber[] { NSNumber.FromLong((nint)Convert.ToInt32 (UITouchType.Direct)) },
				});
			View.AddGestureRecognizer (new UILongPressGestureRecognizer((lp) => {
				// Only accept the first action as the UILongPressGestureRecognizer is a continuous recogniser
				if (lp.State == UIGestureRecognizerState.Began)
				{
					LastFlagTime = DateTime.Now;
					FireAction (TVControllerAction.Flag);
				}
			}));
						
			var skView = (SKView)View;
			skView.ShowsFPS = false;
			skView.ShowsNodeCount = false;
			skView.IgnoresSiblingOrder = false;

			scene = SKNode.FromFile<GameScene> ("GameScene");
			scene.ScaleMode = SKSceneScaleMode.Fill;

			skView.PresentScene (scene);
		}

//		public override bool PrefersStatusBarHidden ()
//		{
//			return true;
//		}
	}
}

