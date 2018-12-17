using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class CompTargetable_SingleCorpse : CompTargetable
	{
		protected override bool PlayerChoosesTarget => true;

		protected override TargetingParameters GetTargetingParameters()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = false;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.canTargetItems = true;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			targetingParameters.validator = ((TargetInfo x) => x.Thing is Corpse && BaseTargetValidator(x.Thing));
			return targetingParameters;
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}
}
