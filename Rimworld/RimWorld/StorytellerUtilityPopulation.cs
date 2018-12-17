using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	[HasDebugOutput]
	public static class StorytellerUtilityPopulation
	{
		private static float PopulationValue_Prisoner = 0.5f;

		private static StorytellerDef StorytellerDef => Find.Storyteller.def;

		public static float PopulationIntent => CalculatePopulationIntent(StorytellerDef, AdjustedPopulation, Find.StoryWatcher.watcherPopAdaptation.AdaptDays);

		public static float AdjustedPopulation
		{
			get
			{
				float num = 0f;
				num += (float)PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.Count();
				return num + (float)PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_PrisonersOfColony.Count() * PopulationValue_Prisoner;
			}
		}

		private static float CalculatePopulationIntent(StorytellerDef def, float curPop, float popAdaptation)
		{
			float num = def.populationIntentFactorFromPopCurve.Evaluate(curPop);
			if (num > 0f)
			{
				num *= def.populationIntentFactorFromPopAdaptDaysCurve.Evaluate(popAdaptation);
			}
			return num;
		}

		public static string DebugReadout()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("IntenderPopulation");
			stringBuilder.AppendLine("   Adjusted population: " + AdjustedPopulation.ToString("F1"));
			stringBuilder.AppendLine("   Pop adaptation days: " + Find.StoryWatcher.watcherPopAdaptation.AdaptDays.ToString("F2"));
			stringBuilder.AppendLine("   PopulationIntent: " + PopulationIntent.ToString("F2"));
			return stringBuilder.ToString();
		}

		[DebugOutput]
		public static void PopulationIntents()
		{
			List<float> list = new List<float>();
			for (int i = 0; i < 30; i++)
			{
				list.Add((float)i);
			}
			List<float> list2 = new List<float>();
			for (int j = 0; j < 40; j += 2)
			{
				list2.Add((float)j);
			}
			DebugTables.MakeTablesDialog(list2, (float ds) => "d-" + ds.ToString("F0"), list, (float rv) => rv.ToString("F2"), (float ds, float p) => CalculatePopulationIntent(StorytellerDef, p, (float)(int)ds).ToString("F2"), "pop");
		}
	}
}
