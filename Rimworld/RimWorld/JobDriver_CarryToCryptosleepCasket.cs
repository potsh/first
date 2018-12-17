using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_CarryToCryptosleepCasket : JobDriver
	{
		private const TargetIndex TakeeInd = TargetIndex.A;

		private const TargetIndex DropPodInd = TargetIndex.B;

		protected Pawn Takee => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		protected Building_CryptosleepCasket DropPod => (Building_CryptosleepCasket)job.GetTarget(TargetIndex.B).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = base.pawn;
			LocalTargetInfo target = Takee;
			Job job = base.job;
			bool errorOnFailed2 = errorOnFailed;
			int result;
			if (pawn.Reserve(target, job, 1, -1, null, errorOnFailed2))
			{
				pawn = base.pawn;
				target = DropPod;
				job = base.job;
				errorOnFailed2 = errorOnFailed;
				result = (pawn.Reserve(target, job, 1, -1, null, errorOnFailed2) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnDestroyedOrNull(TargetIndex.B);
			this.FailOnAggroMentalState(TargetIndex.A);
			this.FailOn(() => !((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_005e: stateMachine*/)._0024this.DropPod.Accepts(((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_005e: stateMachine*/)._0024this.Takee));
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.B)
				.FailOn(() => ((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0084: stateMachine*/)._0024this.DropPod.GetDirectlyHeldThings().Count > 0)
				.FailOn(() => !((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0095: stateMachine*/)._0024this.Takee.Downed)
				.FailOn(() => !((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_00a6: stateMachine*/)._0024this.pawn.CanReach(((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_00a6: stateMachine*/)._0024this.Takee, PathEndMode.OnCell, Danger.Deadly))
				.FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public override object[] TaleParameters()
		{
			return new object[2]
			{
				pawn,
				Takee
			};
		}
	}
}
