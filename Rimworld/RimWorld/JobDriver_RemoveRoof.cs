using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_RemoveRoof : JobDriver_AffectRoof
	{
		private static List<IntVec3> removedRoofs = new List<IntVec3>();

		protected override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOn(() => !((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0029: stateMachine*/)._0024this.Map.areaManager.NoRoof[((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0029: stateMachine*/)._0024this.Cell]);
			using (IEnumerator<Toil> enumerator = base.MakeNewToils().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Toil t = enumerator.Current;
					yield return t;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_00d1:
			/*Error near IL_00d2: Unexpected return in MoveNext()*/;
		}

		protected override void DoEffect()
		{
			removedRoofs.Clear();
			base.Map.roofGrid.SetRoof(base.Cell, null);
			removedRoofs.Add(base.Cell);
			RoofCollapseCellsFinder.CheckCollapseFlyingRoofs(removedRoofs, base.Map, removalMode: true);
			removedRoofs.Clear();
		}

		protected override bool DoWorkFailOn()
		{
			return !base.Cell.Roofed(base.Map);
		}
	}
}
