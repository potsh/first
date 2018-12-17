using RimWorld;
using System.Collections.Generic;

namespace Verse.AI
{
	public class JobDriver_ReleasePrisoner : JobDriver
	{
		private const TargetIndex PrisonerInd = TargetIndex.A;

		private const TargetIndex ReleaseCellInd = TargetIndex.B;

		private Pawn Prisoner => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = base.pawn;
			LocalTargetInfo target = Prisoner;
			Job job = base.job;
			bool errorOnFailed2 = errorOnFailed;
			return pawn.Reserve(target, job, 1, -1, null, errorOnFailed2);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			_003CMakeNewToils_003Ec__Iterator0 _003CMakeNewToils_003Ec__Iterator = (_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0042: stateMachine*/;
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.B);
			this.FailOn(() => ((Pawn)(Thing)_003CMakeNewToils_003Ec__Iterator._0024this.GetActor().CurJob.GetTarget(TargetIndex.A)).guest.interactionMode != PrisonerInteractionModeDefOf.Release);
			this.FailOnDowned(TargetIndex.A);
			this.FailOnAggroMentalState(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOn(() => !_003CMakeNewToils_003Ec__Iterator._0024this.Prisoner.IsPrisonerOfColony || !_003CMakeNewToils_003Ec__Iterator._0024this.Prisoner.guest.PrisonerIsSecure).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}
}
