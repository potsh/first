using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_Flick : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode => PathEndMode.Touch;

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			List<Designation> desList = pawn.Map.designationManager.allDesignations;
			int i = 0;
			while (true)
			{
				if (i >= desList.Count)
				{
					yield break;
				}
				if (desList[i].def == DesignationDefOf.Flick)
				{
					break;
				}
				i++;
			}
			yield return desList[i].target.Thing;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Flick) == null)
			{
				return false;
			}
			LocalTargetInfo target = t;
			bool ignoreOtherReservations = forced;
			if (!pawn.CanReserve(target, 1, -1, null, ignoreOtherReservations))
			{
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(JobDefOf.Flick, t);
		}
	}
}
