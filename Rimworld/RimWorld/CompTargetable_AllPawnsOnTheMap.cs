using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class CompTargetable_AllPawnsOnTheMap : CompTargetable
	{
		protected override bool PlayerChoosesTarget => false;

		protected override TargetingParameters GetTargetingParameters()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.validator = ((TargetInfo x) => BaseTargetValidator(x.Thing));
			return targetingParameters;
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			if (parent.MapHeld != null)
			{
				TargetingParameters tp = GetTargetingParameters();
				foreach (Pawn item in parent.MapHeld.mapPawns.AllPawnsSpawned)
				{
					if (tp.CanTarget(item))
					{
						yield return (Thing)item;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_0109:
			/*Error near IL_010a: Unexpected return in MoveNext()*/;
		}
	}
}
