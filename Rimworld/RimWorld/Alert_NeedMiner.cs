using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	public class Alert_NeedMiner : Alert
	{
		public Alert_NeedMiner()
		{
			defaultLabel = "NeedMiner".Translate();
			defaultExplanation = "NeedMinerDesc".Translate();
			defaultPriority = AlertPriority.High;
		}

		public override AlertReport GetReport()
		{
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				Map map = maps[i];
				if (map.IsPlayerHome)
				{
					Designation designation = (from d in map.designationManager.allDesignations
					where d.def == DesignationDefOf.Mine
					select d).FirstOrDefault();
					if (designation != null)
					{
						bool flag = false;
						foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
						{
							if (!item.Downed && item.workSettings != null && item.workSettings.GetPriority(WorkTypeDefOf.Mining) > 0)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return AlertReport.CulpritIs(designation.target.Thing);
						}
					}
				}
			}
			return false;
		}
	}
}
