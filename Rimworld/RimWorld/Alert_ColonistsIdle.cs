using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Alert_ColonistsIdle : Alert
	{
		public const int MinDaysPassed = 1;

		private IEnumerable<Pawn> IdleColonists
		{
			get
			{
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					if (maps[i].IsPlayerHome)
					{
						foreach (Pawn item in maps[i].mapPawns.FreeColonistsSpawned)
						{
							if (item.mindState.IsIdle)
							{
								yield return item;
								/*Error: Unable to find new state assignment for yield return*/;
							}
						}
					}
				}
				yield break;
				IL_0134:
				/*Error near IL_0135: Unexpected return in MoveNext()*/;
			}
		}

		public override string GetLabel()
		{
			return "ColonistsIdle".Translate(IdleColonists.Count().ToStringCached());
		}

		public override string GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Pawn idleColonist in IdleColonists)
			{
				stringBuilder.AppendLine("    " + idleColonist.LabelShort.CapitalizeFirst());
			}
			return "ColonistsIdleDesc".Translate(stringBuilder.ToString());
		}

		public override AlertReport GetReport()
		{
			if (GenDate.DaysPassed < 1)
			{
				return false;
			}
			return AlertReport.CulpritsAre(IdleColonists);
		}
	}
}
