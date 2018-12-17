using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_BuildSnowman : JobDriver
	{
		private float workLeft = -1000f;

		protected const int BaseWorkAmount = 2300;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = base.pawn;
			LocalTargetInfo targetA = base.job.targetA;
			Job job = base.job;
			bool errorOnFailed2 = errorOnFailed;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed2);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			_003CMakeNewToils_003Ec__Iterator0 _003CMakeNewToils_003Ec__Iterator = (_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0036: stateMachine*/;
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref workLeft, "workLeft", 0f);
		}
	}
}
