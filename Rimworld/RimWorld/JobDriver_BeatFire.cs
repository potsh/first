using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_BeatFire : JobDriver
	{
		protected Fire TargetFire => (Fire)job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			_003CMakeNewToils_003Ec__Iterator0 _003CMakeNewToils_003Ec__Iterator = (_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0036: stateMachine*/;
			this.FailOnDespawnedOrNull(TargetIndex.A);
			Toil beat = new Toil();
			Toil approach = new Toil();
			approach.initAction = delegate
			{
				if (_003CMakeNewToils_003Ec__Iterator._0024this.Map.reservationManager.CanReserve(_003CMakeNewToils_003Ec__Iterator._0024this.pawn, _003CMakeNewToils_003Ec__Iterator._0024this.TargetFire))
				{
					_003CMakeNewToils_003Ec__Iterator._0024this.pawn.Reserve(_003CMakeNewToils_003Ec__Iterator._0024this.TargetFire, _003CMakeNewToils_003Ec__Iterator._0024this.job);
				}
				_003CMakeNewToils_003Ec__Iterator._0024this.pawn.pather.StartPath(_003CMakeNewToils_003Ec__Iterator._0024this.TargetFire, PathEndMode.Touch);
			};
			approach.tickAction = delegate
			{
				if (_003CMakeNewToils_003Ec__Iterator._0024this.pawn.pather.Moving && _003CMakeNewToils_003Ec__Iterator._0024this.pawn.pather.nextCell != _003CMakeNewToils_003Ec__Iterator._0024this.TargetFire.Position)
				{
					_003CMakeNewToils_003Ec__Iterator._0024this.StartBeatingFireIfAnyAt(_003CMakeNewToils_003Ec__Iterator._0024this.pawn.pather.nextCell, beat);
				}
				if (_003CMakeNewToils_003Ec__Iterator._0024this.pawn.Position != _003CMakeNewToils_003Ec__Iterator._0024this.TargetFire.Position)
				{
					_003CMakeNewToils_003Ec__Iterator._0024this.StartBeatingFireIfAnyAt(_003CMakeNewToils_003Ec__Iterator._0024this.pawn.Position, beat);
				}
			};
			approach.FailOnDespawnedOrNull(TargetIndex.A);
			approach.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			approach.atomicWithPrevious = true;
			yield return approach;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private bool StartBeatingFireIfAnyAt(IntVec3 cell, Toil nextToil)
		{
			List<Thing> thingList = cell.GetThingList(base.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Fire fire = thingList[i] as Fire;
				if (fire != null && fire.parent == null)
				{
					job.targetA = fire;
					pawn.pather.StopDead();
					JumpToToil(nextToil);
					return true;
				}
			}
			return false;
		}
	}
}
