using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Alert_HypothermicAnimals : Alert
	{
		private IEnumerable<Pawn> HypothermicAnimals => from p in PawnsFinder.AllMaps_Spawned
		where p.RaceProps.Animal && p.Faction == null && p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia) != null
		select p;

		public override string GetLabel()
		{
			return "Hypothermic wild animals (debug)";
		}

		public override string GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Debug alert:\n\nThese wild animals are hypothermic. This may indicate a bug (but it may not, if the animals are trapped or in some other wierd but legitimate situation):");
			foreach (Pawn hypothermicAnimal in HypothermicAnimals)
			{
				stringBuilder.AppendLine("    " + hypothermicAnimal + " at " + hypothermicAnimal.Position);
			}
			return stringBuilder.ToString();
		}

		public override AlertReport GetReport()
		{
			if (!Prefs.DevMode)
			{
				return false;
			}
			return AlertReport.CulpritsAre(HypothermicAnimals);
		}
	}
}
