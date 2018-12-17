using Verse;

namespace RimWorld.BaseGen
{
	public class SymbolResolver_EdgeSandbags : SymbolResolver
	{
		private static readonly IntRange LineLengthRange = new IntRange(2, 5);

		private static readonly IntRange GapLengthRange = new IntRange(1, 5);

		public override void Resolve(ResolveParams rp)
		{
			Map map = BaseGen.globalSettings.map;
			int num = 0;
			int num2 = 0;
			int num3 = -1;
			int edgeCellsCount = rp.rect.EdgeCellsCount;
			IntRange lineLengthRange = LineLengthRange;
			int max = lineLengthRange.max;
			IntRange gapLengthRange = GapLengthRange;
			if (edgeCellsCount < (max + gapLengthRange.max) * 2)
			{
				num = rp.rect.EdgeCellsCount;
			}
			else if (Rand.Bool)
			{
				num = LineLengthRange.RandomInRange;
			}
			else
			{
				num2 = GapLengthRange.RandomInRange;
			}
			foreach (IntVec3 edgeCell in rp.rect.EdgeCells)
			{
				num3++;
				if (num2 > 0)
				{
					num2--;
					if (num2 == 0)
					{
						if (num3 == rp.rect.EdgeCellsCount - 2)
						{
							num2 = 1;
						}
						else
						{
							num = LineLengthRange.RandomInRange;
						}
					}
				}
				else if (edgeCell.Standable(map) && !edgeCell.Roofed(map) && edgeCell.SupportsStructureType(map, ThingDefOf.Sandbags.terrainAffordanceNeeded) && !GenSpawn.WouldWipeAnythingWith(edgeCell, Rot4.North, ThingDefOf.Sandbags, map, (Thing x) => x.def.category == ThingCategory.Building || x.def.category == ThingCategory.Item))
				{
					if (num > 0)
					{
						num--;
						if (num == 0)
						{
							num2 = GapLengthRange.RandomInRange;
						}
					}
					Thing thing = ThingMaker.MakeThing(ThingDefOf.Sandbags);
					thing.SetFaction(rp.faction);
					GenSpawn.Spawn(thing, edgeCell, map);
				}
			}
		}
	}
}
