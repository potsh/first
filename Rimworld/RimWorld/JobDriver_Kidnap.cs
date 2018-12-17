using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_Kidnap : JobDriver_TakeAndExitMap
	{
		protected Pawn Takee => (Pawn)base.Item;

		public override string GetReport()
		{
			if (Takee == null || pawn.HostileTo(Takee))
			{
				return base.GetReport();
			}
			return JobDefOf.Rescue.reportString.Replace("TargetA", Takee.LabelShort);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOn(() => ((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0029: stateMachine*/)._0024this.Takee == null || (!((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0029: stateMachine*/)._0024this.Takee.Downed && ((_003CMakeNewToils_003Ec__Iterator0)/*Error near IL_0029: stateMachine*/)._0024this.Takee.Awake()));
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
	}
}
