using RimWorld.BaseGen;
using RimWorld.Planet;
using Verse;

namespace RimWorld
{
	public class GenStep_PrisonerWillingToJoin : GenStep_Scatterer
	{
		private const int Size = 8;

		public override int SeedPart => 69356099;

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
			CellRect.CellRectIterator iterator = CellRect.CenteredOn(c, 8, 8).GetIterator();
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
			Faction faction = (map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer) ? map.ParentFaction : Find.FactionManager.RandomEnemyFaction();
			CellRect cellRect = CellRect.CenteredOn(loc, 8, 8).ClipInsideMap(map);
			PrisonerWillingToJoinComp component = map.Parent.GetComponent<PrisonerWillingToJoinComp>();
			Pawn singlePawnToSpawn = (component == null || !component.pawn.Any) ? PrisonerWillingToJoinQuestUtility.GeneratePrisoner(map.Tile, faction) : component.pawn.Take(component.pawn[0]);
			ResolveParams resolveParams = default(ResolveParams);
			resolveParams.rect = cellRect;
			resolveParams.faction = faction;
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			RimWorld.BaseGen.BaseGen.symbolStack.Push("prisonCell", resolveParams);
			RimWorld.BaseGen.BaseGen.Generate();
			ResolveParams resolveParams2 = default(ResolveParams);
			resolveParams2.rect = cellRect;
			resolveParams2.faction = faction;
			resolveParams2.singlePawnToSpawn = singlePawnToSpawn;
			resolveParams2.postThingSpawn = delegate(Thing x)
			{
				MapGenerator.rootsToUnfog.Add(x.Position);
				((Pawn)x).mindState.WillJoinColonyIfRescued = true;
			};
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			RimWorld.BaseGen.BaseGen.symbolStack.Push("pawn", resolveParams2);
			RimWorld.BaseGen.BaseGen.Generate();
			MapGenerator.SetVar("RectOfInterest", cellRect);
		}
	}
}
