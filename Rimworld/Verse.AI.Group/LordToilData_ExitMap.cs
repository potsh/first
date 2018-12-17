namespace Verse.AI.Group
{
	public class LordToilData_ExitMap : LordToilData
	{
		public LocomotionUrgency locomotion;

		public bool canDig;

		public override void ExposeData()
		{
			Scribe_Values.Look(ref locomotion, "locomotion", LocomotionUrgency.None);
			Scribe_Values.Look(ref canDig, "canDig", defaultValue: false);
		}
	}
}
