using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet
{
	public class AbandonComp : WorldObjectComp
	{
		public override IEnumerable<Gizmo> GetGizmos()
		{
			MapParent mapParent = parent as MapParent;
			if (mapParent.HasMap && mapParent.Faction == Faction.OfPlayer)
			{
				yield return (Gizmo)SettlementAbandonUtility.AbandonCommand(mapParent);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}
	}
}
