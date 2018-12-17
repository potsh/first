using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_UnloadCarriers : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

		public override PathEndMode PathEndMode => PathEndMode.Touch;

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return UnloadCarriersJobGiverUtility.HasJobOnThing(pawn, t, forced);
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(JobDefOf.UnloadInventory, t);
		}
	}
}
