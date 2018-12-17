using RimWorld.BaseGen;
using RimWorld.Planet;
using Verse;

namespace RimWorld
{
	public class GenStep_ItemStash : GenStep_Scatterer
	{
		public ThingSetMakerDef thingSetMakerDef;

		private const int Size = 7;

		public override int SeedPart => 913432591;

		protected override bool CanScatterAt(IntVec3 c, Map map)
		{
			if (!base.CanScatterAt(c, map))
			{
				return false;
			}
			if (!c.SupportsStructureType(map, TerrainAffordanceDefOf.Heavy))
			{
				return false;
			}
			if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors)))
			{
				return false;
			}
			CellRect.CellRectIterator iterator = CellRect.CenteredOn(c, 7, 7).GetIterator();
			while (!iterator.Done())
			{
				if (!iterator.Current.InBounds(map) || iterator.Current.GetEdifice(map) != null)
				{
					return false;
				}
				iterator.MoveNext();
			}
			return true;
		}

		protected override void ScatterAt(IntVec3 loc, Map map, int count = 1)
		{
			CellRect cellRect = CellRect.CenteredOn(loc, 7, 7).ClipInsideMap(map);
			ResolveParams resolveParams = default(ResolveParams);
			resolveParams.rect = cellRect;
			resolveParams.faction = map.ParentFaction;
			ItemStashContentsComp component = map.Parent.GetComponent<ItemStashContentsComp>();
			if (component != null && component.contents.Any)
			{
				resolveParams.stockpileConcreteContents = component.contents;
			}
			else
			{
				resolveParams.thingSetMakerDef = (thingSetMakerDef ?? ThingSetMakerDefOf.MapGen_DefaultStockpile);
			}
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			RimWorld.BaseGen.BaseGen.symbolStack.Push("storage", resolveParams);
			RimWorld.BaseGen.BaseGen.Generate();
			MapGenerator.SetVar("RectOfInterest", cellRect);
		}
	}
}
