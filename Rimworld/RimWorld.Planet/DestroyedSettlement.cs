using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet
{
	public class DestroyedSettlement : MapParent
	{
		public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
		{
			if (!base.Map.mapPawns.AnyPawnBlockingMapRemoval)
			{
				alsoRemoveWorldObject = true;
				return true;
			}
			alsoRemoveWorldObject = false;
			return false;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			using (IEnumerator<Gizmo> enumerator = base.GetGizmos().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Gizmo g = enumerator.Current;
					yield return g;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			if (base.HasMap && Find.WorldSelector.SingleSelectedObject == this)
			{
				yield return (Gizmo)SettleInExistingMapUtility.SettleCommand(base.Map, requiresNoEnemies: false);
				/*Error: Unable to find new state assignment for yield return*/;
			}
			yield break;
			IL_010d:
			/*Error near IL_010e: Unexpected return in MoveNext()*/;
		}
	}
}
