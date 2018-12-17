using RimWorld.Planet;
using System.Collections.Generic;

namespace Verse.AI
{
	public class JobDriver_Goto : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, job.targetA.Cell);
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil gotoCell = Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
			gotoCell.AddPreTickAction(delegate
			{
				if (((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0038: stateMachine*/)._0024this.job.exitMapOnArrival && ((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0038: stateMachine*/)._0024this.pawn.Map.exitMapGrid.IsExitCell(((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0038: stateMachine*/)._0024this.pawn.Position))
				{
					((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0038: stateMachine*/)._0024this.TryExitMap();
				}
			});
			gotoCell.FailOn(() => ((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_004f: stateMachine*/)._0024this.job.failIfCantJoinOrCreateCaravan && !CaravanExitMapUtility.CanExitMapAndJoinOrCreateCaravanNow(((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_004f: stateMachine*/)._0024this.pawn));
			yield return gotoCell;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private void TryExitMap()
		{
			if (!job.failIfCantJoinOrCreateCaravan || CaravanExitMapUtility.CanExitMapAndJoinOrCreateCaravanNow(pawn))
			{
				pawn.ExitMap(allowedToJoinOrCreateCaravan: true, CellRect.WholeMap(base.Map).GetClosestEdge(pawn.Position));
			}
		}
	}
}
