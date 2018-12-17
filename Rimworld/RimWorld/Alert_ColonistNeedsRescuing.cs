using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Alert_ColonistNeedsRescuing : Alert_Critical
	{
		private IEnumerable<Pawn> ColonistsNeedingRescue
		{
			get
			{
				foreach (Pawn item in PawnsFinder.AllMaps_FreeColonistsSpawned)
				{
					if (NeedsRescue(item))
					{
						yield return item;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
				yield break;
				IL_00c3:
				/*Error near IL_00c4: Unexpected return in MoveNext()*/;
			}
		}

		public static bool NeedsRescue(Pawn p)
		{
			if (p.Downed && !p.InBed() && !(p.ParentHolder is Pawn_CarryTracker))
			{
				if (p.jobs.jobQueue != null && p.jobs.jobQueue.Count > 0 && p.jobs.jobQueue.Peek().job.CanBeginNow(p))
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public override string GetLabel()
		{
			if (ColonistsNeedingRescue.Count() == 1)
			{
				return "ColonistNeedsRescue".Translate();
			}
			return "ColonistsNeedRescue".Translate();
		}

		public override string GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Pawn item in ColonistsNeedingRescue)
			{
				stringBuilder.AppendLine("    " + item.LabelShort);
			}
			return "ColonistsNeedRescueDesc".Translate(stringBuilder.ToString());
		}

		public override AlertReport GetReport()
		{
			return AlertReport.CulpritsAre(ColonistsNeedingRescue);
		}
	}
}
