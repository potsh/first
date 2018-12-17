using Verse;

namespace RimWorld.BaseGen
{
	public abstract class SymbolResolver
	{
		public IntVec2 minRectSize = IntVec2.One;

		public float selectionWeight = 1f;

		public virtual bool CanResolve(ResolveParams rp)
		{
			return rp.rect.Width >= minRectSize.x && rp.rect.Height >= minRectSize.z;
		}

		public abstract void Resolve(ResolveParams rp);
	}
}
