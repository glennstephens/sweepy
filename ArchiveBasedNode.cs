using System;
using SpriteKit;
using Foundation;

namespace tvMinesweeper
{
	public abstract class ArchiveBasedNode : SKEmitterNode
	{
		public ArchiveBasedNode (IntPtr template)
			: base (template)
		{
			// calling the base .ctor with the Handle of the Copy will add an extra Retain
			//Release ();
		}

		protected static SKEmitterNode UnarchiveNode (string name, string type)
		{
			var path = NSBundle.MainBundle.PathForResource (name, type);
			return (SKEmitterNode) NSKeyedUnarchiver.UnarchiveFile (path);
		}
	}

	public class ExplosionNode : ArchiveBasedNode
	{
		const double defaultDuration = 0.1f;

		static SKEmitterNode template = UnarchiveNode ("explosion", "sks");

		public ExplosionNode (SKNode target)
			: base ((template as NSObject).Copy ().Handle)
		{
			TargetNode = target;
			NumParticlesToEmit = (uint) (defaultDuration * ParticleBirthRate);
			double totalTime = defaultDuration + ParticleLifetime + ParticleLifetimeRange / 2;

			RunAction (SKAction.Sequence (
				SKAction.WaitForDuration (totalTime),
				SKAction.RemoveFromParent ()
			));
		}
	}
}

