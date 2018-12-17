using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public abstract class JobDriver_PlantWork : JobDriver
	{
		private float workDone;

		protected float xpPerTick;

		protected const TargetIndex PlantInd = TargetIndex.A;

		protected Plant Plant => (Plant)job.targetA.Thing;

		protected virtual DesignationDef RequiredDesignation => null;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			LocalTargetInfo target = base.job.GetTarget(TargetIndex.A);
			if (target.IsValid)
			{
				Pawn pawn = base.pawn;
				LocalTargetInfo target2 = target;
				Job job = base.job;
				bool errorOnFailed2 = errorOnFailed;
				if (!pawn.Reserve(target2, job, 1, -1, null, errorOnFailed2))
				{
					return false;
				}
			}
			base.pawn.ReserveAsManyAsPossible(base.job.GetTargetQueue(TargetIndex.A), base.job);
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			_003CMakeNewToils_003Ec__Iterator0 _003CMakeNewToils_003Ec__Iterator = (_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_004e: stateMachine*/;
			Init();
			yield return Toils_JobTransforms.MoveCurrentTargetIntoQueue(TargetIndex.A);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref workDone, "workDone", 0f);
		}

		protected virtual void Init()
		{
		}

		protected virtual Toil PlantWorkDoneToil()
		{
			return null;
		}
	}
}
