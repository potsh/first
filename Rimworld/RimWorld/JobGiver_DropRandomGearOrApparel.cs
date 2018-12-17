using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_DropRandomGearOrApparel : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.equipment != null && pawn.equipment.HasAnything())
			{
				return new Job(JobDefOf.DropEquipment, pawn.equipment.AllEquipmentListForReading.RandomElement());
			}
			if (pawn.apparel != null && pawn.apparel.WornApparel.Any())
			{
				return new Job(JobDefOf.RemoveApparel, pawn.apparel.WornApparel.RandomElement());
			}
			return null;
		}
	}
}
