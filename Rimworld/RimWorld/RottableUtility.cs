using Verse;

namespace RimWorld
{
	public static class RottableUtility
	{
		public static bool IsNotFresh(this Thing t)
		{
			CompRottable compRottable = t.TryGetComp<CompRottable>();
			return compRottable != null && compRottable.Stage != RotStage.Fresh;
		}

		public static bool IsDessicated(this Thing t)
		{
			CompRottable compRottable = t.TryGetComp<CompRottable>();
			return compRottable != null && compRottable.Stage == RotStage.Dessicated;
		}

		public static RotStage GetRotStage(this Thing t)
		{
			return t.TryGetComp<CompRottable>()?.Stage ?? RotStage.Fresh;
		}
	}
}
