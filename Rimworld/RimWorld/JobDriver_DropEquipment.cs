using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_DropEquipment : JobDriver
	{
		private const int DurationTicks = 30;

		private ThingWithComps TargetEquipment => (ThingWithComps)base.TargetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			yield return new Toil
			{
				initAction = delegate
				{
					((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0043: stateMachine*/)._0024this.pawn.pather.StopDead();
				},
				defaultCompleteMode = ToilCompleteMode.Delay,
				defaultDuration = 30
			};
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}
}
