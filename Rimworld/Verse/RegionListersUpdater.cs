using System.Collections.Generic;
using Verse.AI;

namespace Verse
{
	public static class RegionListersUpdater
	{
		private static List<Region> tmpRegions = new List<Region>();

		public static void DeregisterInRegions(Thing thing, Map map)
		{
			ThingDef def = thing.def;
			if (ListerThings.EverListable(def, ListerThingsUse.Region))
			{
				GetTouchableRegions(thing, map, tmpRegions, allowAdjacentEvenIfCantTouch: true);
				for (int i = 0; i < tmpRegions.Count; i++)
				{
					ListerThings listerThings = tmpRegions[i].ListerThings;
					if (listerThings.Contains(thing))
					{
						listerThings.Remove(thing);
					}
				}
				tmpRegions.Clear();
			}
		}

		public static void RegisterInRegions(Thing thing, Map map)
		{
			ThingDef def = thing.def;
			if (ListerThings.EverListable(def, ListerThingsUse.Region))
			{
				GetTouchableRegions(thing, map, tmpRegions);
				for (int i = 0; i < tmpRegions.Count; i++)
				{
					ListerThings listerThings = tmpRegions[i].ListerThings;
					if (!listerThings.Contains(thing))
					{
						listerThings.Add(thing);
					}
				}
				tmpRegions.Clear();
			}
		}

		public static void RegisterAllAt(IntVec3 c, Map map, HashSet<Thing> processedThings = null)
		{
			List<Thing> thingList = c.GetThingList(map);
			int count = thingList.Count;
			for (int i = 0; i < count; i++)
			{
				Thing thing = thingList[i];
				if (processedThings == null || processedThings.Add(thing))
				{
					RegisterInRegions(thing, map);
				}
			}
		}

		public static void GetTouchableRegions(Thing thing, Map map, List<Region> outRegions, bool allowAdjacentEvenIfCantTouch = false)
		{
			outRegions.Clear();
			CellRect cellRect = thing.OccupiedRect();
			CellRect cellRect2 = cellRect;
			if (CanRegisterInAdjacentRegions(thing))
			{
				cellRect2 = cellRect2.ExpandedBy(1);
			}
			CellRect.CellRectIterator iterator = cellRect2.GetIterator();
			while (!iterator.Done())
			{
				IntVec3 current = iterator.Current;
				if (current.InBounds(map))
				{
					Region validRegionAt_NoRebuild = map.regionGrid.GetValidRegionAt_NoRebuild(current);
					if (validRegionAt_NoRebuild != null && validRegionAt_NoRebuild.type.Passable() && !outRegions.Contains(validRegionAt_NoRebuild))
					{
						if (cellRect.Contains(current))
						{
							outRegions.Add(validRegionAt_NoRebuild);
						}
						else if (allowAdjacentEvenIfCantTouch || ReachabilityImmediate.CanReachImmediate(current, thing, map, PathEndMode.Touch, null))
						{
							outRegions.Add(validRegionAt_NoRebuild);
						}
					}
				}
				iterator.MoveNext();
			}
		}

		private static bool CanRegisterInAdjacentRegions(Thing thing)
		{
			return true;
		}
	}
}
