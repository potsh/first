using Verse;
using Verse.AI;

namespace RimWorld
{
	public class ThinkNode_ConditionalGuest : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn pawn)
		{
			return pawn.HostFaction != null && !pawn.IsPrisoner;
		}
	}
}
