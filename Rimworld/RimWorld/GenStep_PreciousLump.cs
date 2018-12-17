using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class GenStep_PreciousLump : GenStep_ScatterLumpsMineable
	{
		public List<ThingDef> mineables;

		public FloatRange totalValueRange = new FloatRange(1000f, 2000f);

		public override int SeedPart => 1634184421;

		public override void Generate(Map map, GenStepParams parms)
		{
			if (parms.siteCoreOrPart != null && parms.siteCoreOrPart.parms.preciousLumpResources != null)
			{
				forcedDefToScatter = parms.siteCoreOrPart.parms.preciousLumpResources;
			}
			else
			{
				forcedDefToScatter = mineables.RandomElement();
			}
			count = 1;
			float randomInRange = totalValueRange.RandomInRange;
			float baseMarketValue = forcedDefToScatter.building.mineableThing.BaseMarketValue;
			forcedLumpSize = Mathf.Max(Mathf.RoundToInt(randomInRange / ((float)forcedDefToScatter.building.mineableYield * baseMarketValue)), 1);
			base.Generate(map, parms);
		}

		protected override bool CanScatterAt(IntVec3 c, Map map)
		{
			return map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors));
		}

		protected override void ScatterAt(IntVec3 c, Map map, int stackCount = 1)
		{
			base.ScatterAt(c, map, stackCount);
			int minX = recentLumpCells.Min((IntVec3 x) => x.x);
			int minZ = recentLumpCells.Min((IntVec3 x) => x.z);
			int maxX = recentLumpCells.Max((IntVec3 x) => x.x);
			int maxZ = recentLumpCells.Max((IntVec3 x) => x.z);
			CellRect var = CellRect.FromLimits(minX, minZ, maxX, maxZ);
			MapGenerator.SetVar("RectOfInterest", var);
		}
	}
}
