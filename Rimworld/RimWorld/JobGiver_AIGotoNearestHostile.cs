using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_AIGotoNearestHostile : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			float num = 3.40282347E+38f;
			Thing thing = null;
			List<IAttackTarget> potentialTargetsFor = pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
			for (int i = 0; i < potentialTargetsFor.Count; i++)
			{
				IAttackTarget attackTarget = potentialTargetsFor[i];
				if (!attackTarget.ThreatDisabled(pawn))
				{
					Thing thing2 = (Thing)attackTarget;
					int num2 = thing2.Position.DistanceToSquared(pawn.Position);
					if ((float)num2 < num && pawn.CanReach(thing2, PathEndMode.OnCell, Danger.Deadly))
					{
						num = (float)num2;
						thing = thing2;
					}
				}
			}
			if (thing != null)
			{
				Job job = new Job(JobDefOf.Goto, thing);
				job.checkOverrideOnExpire = true;
				job.expiryInterval = 500;
				job.collideWithPawns = true;
				return job;
			}
			return null;
		}
	}
}
