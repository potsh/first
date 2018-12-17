using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public abstract class WorkGiver_GatherAnimalBodyResources : WorkGiver_Scanner
	{
		protected abstract JobDef JobDef
		{
			get;
		}

		public override PathEndMode PathEndMode => PathEndMode.Touch;

		protected abstract CompHasGatherableBodyResource GetComp(Pawn animal);

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			List<Pawn> pawns = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
			int i = 0;
			if (i < pawns.Count)
			{
				yield return (Thing)pawns[i];
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			if (pawn2 == null || !pawn2.RaceProps.Animal)
			{
				return false;
			}
			CompHasGatherableBodyResource comp = GetComp(pawn2);
			if (comp != null && comp.ActiveAndFull && !pawn2.Downed && pawn2.CanCasuallyInteractNow())
			{
				LocalTargetInfo target = pawn2;
				bool ignoreOtherReservations = forced;
				if (pawn.CanReserve(target, 1, -1, null, ignoreOtherReservations))
				{
					return true;
				}
			}
			return false;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(JobDef, t);
		}
	}
}
