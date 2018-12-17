using UnityEngine;
using Verse;

namespace RimWorld
{
	public class IncidentWorker_ThrumboPasses : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
			{
				return false;
			}
			if (!map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(ThingDefOf.Thrumbo))
			{
				return false;
			}
			IntVec3 cell;
			return TryFindEntryCell(map, out cell);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (!TryFindEntryCell(map, out IntVec3 cell))
			{
				return false;
			}
			PawnKindDef thrumbo = PawnKindDefOf.Thrumbo;
			float num = StorytellerUtility.DefaultThreatPointsNow(map);
			int value = GenMath.RoundRandom(num / thrumbo.combatPower);
			int max = Rand.RangeInclusive(2, 4);
			value = Mathf.Clamp(value, 1, max);
			int num2 = Rand.RangeInclusive(90000, 150000);
			IntVec3 result = IntVec3.Invalid;
			if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(cell, map, 10f, out result))
			{
				result = IntVec3.Invalid;
			}
			Pawn pawn = null;
			for (int i = 0; i < value; i++)
			{
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(cell, map, 10);
				pawn = PawnGenerator.GeneratePawn(thrumbo);
				GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
				pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num2;
				if (result.IsValid)
				{
					pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(result, map, 10);
				}
			}
			Find.LetterStack.ReceiveLetter("LetterLabelThrumboPasses".Translate(thrumbo.label).CapitalizeFirst(), "LetterThrumboPasses".Translate(thrumbo.label), LetterDefOf.PositiveEvent, pawn);
			return true;
		}

		private bool TryFindEntryCell(Map map, out IntVec3 cell)
		{
			return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
		}
	}
}
