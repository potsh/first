using System.Linq;
using Verse;

namespace RimWorld
{
	public static class DeepDrillUtility
	{
		public static ThingDef GetNextResource(IntVec3 p, Map map)
		{
			GetNextResource(p, map, out ThingDef resDef, out int _, out IntVec3 _);
			return resDef;
		}

		public static bool GetNextResource(IntVec3 p, Map map, out ThingDef resDef, out int countPresent, out IntVec3 cell)
		{
			for (int i = 0; i < 9; i++)
			{
				IntVec3 intVec = p + GenRadial.RadialPattern[i];
				if (intVec.InBounds(map))
				{
					ThingDef thingDef = map.deepResourceGrid.ThingDefAt(intVec);
					if (thingDef != null)
					{
						resDef = thingDef;
						countPresent = map.deepResourceGrid.CountAt(intVec);
						cell = intVec;
						return true;
					}
				}
			}
			resDef = GetBaseResource(map);
			countPresent = 2147483647;
			cell = p;
			return false;
		}

		public static ThingDef GetBaseResource(Map map)
		{
			if (!map.Biome.hasBedrock)
			{
				return null;
			}
			return (from rock in Find.World.NaturalRockTypesIn(map.Tile)
			select rock.building.mineableThing).FirstOrDefault();
		}
	}
}
