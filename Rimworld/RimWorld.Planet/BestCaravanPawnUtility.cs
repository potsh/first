using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet
{
	public static class BestCaravanPawnUtility
	{
		public static Pawn FindBestDiplomat(Caravan caravan)
		{
			return FindPawnWithBestStat(caravan, StatDefOf.NegotiationAbility);
		}

		public static Pawn FindBestNegotiator(Caravan caravan)
		{
			return FindPawnWithBestStat(caravan, StatDefOf.TradePriceImprovement);
		}

		public static Pawn FindBestEntertainingPawnFor(Caravan caravan, Pawn forPawn)
		{
			Pawn pawn = null;
			float num = -1f;
			for (int i = 0; i < caravan.pawns.Count; i++)
			{
				Pawn pawn2 = caravan.pawns[i];
				if (pawn2 != forPawn && pawn2.RaceProps.Humanlike && !pawn2.Dead && !pawn2.Downed && !pawn2.InMentalState && pawn2.IsPrisoner == forPawn.IsPrisoner && !StatDefOf.SocialImpact.Worker.IsDisabledFor(pawn2))
				{
					float statValue = pawn2.GetStatValue(StatDefOf.SocialImpact);
					if (pawn == null || statValue > num)
					{
						pawn = pawn2;
						num = statValue;
					}
				}
			}
			return pawn;
		}

		public static Pawn FindPawnWithBestStat(Caravan caravan, StatDef stat)
		{
			Pawn pawn = null;
			float num = -1f;
			List<Pawn> pawnsListForReading = caravan.PawnsListForReading;
			for (int i = 0; i < pawnsListForReading.Count; i++)
			{
				Pawn pawn2 = pawnsListForReading[i];
				if (IsConsciousOwner(pawn2, caravan) && !stat.Worker.IsDisabledFor(pawn2))
				{
					float statValue = pawn2.GetStatValue(stat);
					if (pawn == null || statValue > num)
					{
						pawn = pawn2;
						num = statValue;
					}
				}
			}
			return pawn;
		}

		private static bool IsConsciousOwner(Pawn pawn, Caravan caravan)
		{
			return !pawn.Dead && !pawn.Downed && !pawn.InMentalState && caravan.IsOwner(pawn);
		}
	}
}
