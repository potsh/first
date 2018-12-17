using RimWorld.Planet;
using System.Collections.Generic;

namespace RimWorld
{
	public class WorldObjectCompProperties_TimedForcedExit : WorldObjectCompProperties
	{
		public WorldObjectCompProperties_TimedForcedExit()
		{
			compClass = typeof(TimedForcedExit);
		}

		public override IEnumerable<string> ConfigErrors(WorldObjectDef parentDef)
		{
			using (IEnumerator<string> enumerator = base.ConfigErrors(parentDef).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string e = enumerator.Current;
					yield return e;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			if (!typeof(MapParent).IsAssignableFrom(parentDef.worldObjectClass))
			{
				yield return parentDef.defName + " has WorldObjectCompProperties_TimedForcedExit but it's not MapParent.";
				/*Error: Unable to find new state assignment for yield return*/;
			}
			yield break;
			IL_0111:
			/*Error near IL_0112: Unexpected return in MoveNext()*/;
		}
	}
}
