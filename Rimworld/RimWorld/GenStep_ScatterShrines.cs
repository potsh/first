using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class GenStep_ScatterShrines : GenStep_ScatterRuinsSimple
	{
		private static readonly IntRange ShrinesCountX = new IntRange(1, 4);

		private static readonly IntRange ShrinesCountZ = new IntRange(1, 4);

		private static readonly IntRange ExtraHeightRange = new IntRange(0, 8);

		private const int MarginCells = 1;

		public override int SeedPart => 1801222485;

		protected override bool CanScatterAt(IntVec3 c, Map map)
		{
			if (!base.CanScatterAt(c, map))
			{
				return false;
			}
			Building edifice = c.GetEdifice(map);
			if (edifice == null || !edifice.def.building.isNaturalRock)
			{
				return false;
			}
			return true;
		}

		protected override void ScatterAt(IntVec3 loc, Map map, int stackCount = 1)
		{
			int randomInRange = ShrinesCountX.RandomInRange;
			int randomInRange2 = ShrinesCountZ.RandomInRange;
			int randomInRange3 = ExtraHeightRange.RandomInRange;
			IntVec2 standardAncientShrineSize = SymbolResolver_AncientShrinesGroup.StandardAncientShrineSize;
			int num = 1;
			int num2 = randomInRange * standardAncientShrineSize.x + (randomInRange - 1) * num;
			int num3 = randomInRange2 * standardAncientShrineSize.z + (randomInRange2 - 1) * num;
			int num4 = num2 + 2;
			int num5 = num3 + 2 + randomInRange3;
			CellRect rect = new CellRect(loc.x, loc.z, num4, num5);
			rect.ClipInsideMap(map);
			if (rect.Width == num4 && rect.Height == num5)
			{
				foreach (IntVec3 cell in rect.Cells)
				{
					List<Thing> list = map.thingGrid.ThingsListAt(cell);
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].def == ThingDefOf.AncientCryptosleepCasket)
						{
							return;
						}
					}
				}
				if (CanPlaceAncientBuildingInRange(rect, map))
				{
					ResolveParams resolveParams = default(ResolveParams);
					resolveParams.rect = rect;
					resolveParams.disableSinglePawn = true;
					resolveParams.disableHives = true;
					resolveParams.ancientTempleEntranceHeight = randomInRange3;
					RimWorld.BaseGen.BaseGen.globalSettings.map = map;
					RimWorld.BaseGen.BaseGen.symbolStack.Push("ancientTemple", resolveParams);
					RimWorld.BaseGen.BaseGen.Generate();
					int nextSignalTagID = Find.UniqueIDsManager.GetNextSignalTagID();
					string signalTag = "ancientTempleApproached-" + nextSignalTagID;
					SignalAction_Letter signalAction_Letter = (SignalAction_Letter)ThingMaker.MakeThing(ThingDefOf.SignalAction_Letter);
					signalAction_Letter.signalTag = signalTag;
					signalAction_Letter.letter = LetterMaker.MakeLetter("LetterLabelAncientShrineWarning".Translate(), "AncientShrineWarning".Translate(), LetterDefOf.NeutralEvent, new TargetInfo(rect.CenterCell, map));
					GenSpawn.Spawn(signalAction_Letter, rect.CenterCell, map);
					RectTrigger rectTrigger = (RectTrigger)ThingMaker.MakeThing(ThingDefOf.RectTrigger);
					rectTrigger.signalTag = signalTag;
					rectTrigger.Rect = rect.ExpandedBy(1).ClipInsideMap(map);
					rectTrigger.destroyIfUnfogged = true;
					GenSpawn.Spawn(rectTrigger, rect.CenterCell, map);
				}
			}
		}
	}
}
