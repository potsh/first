using Verse;

namespace RimWorld
{
	public class CompProperties_RoomIdentifier : CompProperties
	{
		public RoomStatDef roomStat;

		public CompProperties_RoomIdentifier()
		{
			compClass = typeof(CompRoomIdentifier);
		}
	}
}
