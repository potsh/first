using RimWorld.BaseGen;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class GenStep_Turrets : GenStep
	{
		public IntRange defaultTurretsCountRange = new IntRange(4, 5);

		public IntRange defaultMortarsCountRange = new IntRange(0, 1);

		public IntRange widthRange = new IntRange(3, 4);

		public IntRange guardsCountRange = new IntRange(1, 1);

		private const int Padding = 7;

		public const int DefaultGuardsCount = 1;

		public override int SeedPart => 895502705;

		public override void Generate(Map map, GenStepParams parms)
		{
			if (!MapGenerator.TryGetVar("RectOfInterest", out CellRect var))
			{
				var = FindRandomRectToDefend(map);
			}
			Faction faction = (map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer) ? map.ParentFaction : (from x in Find.FactionManager.AllFactions
			where !x.defeated && x.HostileTo(Faction.OfPlayer) && !x.def.hidden && (int)x.def.techLevel >= 4
			select x).RandomElementWithFallback(Find.FactionManager.RandomEnemyFaction());
			int randomInRange = widthRange.RandomInRange;
			CellRect rect = var.ExpandedBy(7 + randomInRange).ClipInsideMap(map);
			int value;
			int value2;
			if (parms.siteCoreOrPart != null)
			{
				value = parms.siteCoreOrPart.parms.turretsCount;
				value2 = parms.siteCoreOrPart.parms.mortarsCount;
			}
			else
			{
				value = defaultTurretsCountRange.RandomInRange;
				value2 = defaultMortarsCountRange.RandomInRange;
			}
			ResolveParams resolveParams = default(ResolveParams);
			resolveParams.rect = rect;
			resolveParams.faction = faction;
			resolveParams.edgeDefenseWidth = randomInRange;
			resolveParams.edgeDefenseTurretsCount = value;
			resolveParams.edgeDefenseMortarsCount = value2;
			resolveParams.edgeDefenseGuardsCount = guardsCountRange.RandomInRange;
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			RimWorld.BaseGen.BaseGen.symbolStack.Push("edgeDefense", resolveParams);
			RimWorld.BaseGen.BaseGen.Generate();
			ResolveParams resolveParams2 = default(ResolveParams);
			resolveParams2.rect = rect;
			resolveParams2.faction = faction;
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			RimWorld.BaseGen.BaseGen.symbolStack.Push("outdoorLighting", resolveParams2);
			RimWorld.BaseGen.BaseGen.Generate();
		}

		private CellRect FindRandomRectToDefend(Map map)
		{
			IntVec3 size = map.Size;
			int x2 = size.x;
			IntVec3 size2 = map.Size;
			int rectRadius = Mathf.Max(Mathf.RoundToInt((float)Mathf.Min(x2, size2.z) * 0.07f), 1);
			TraverseParms traverseParams = TraverseParms.For(TraverseMode.PassDoors);
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(delegate(IntVec3 x)
			{
				if (!map.reachability.CanReachMapEdge(x, traverseParams))
				{
					return false;
				}
				CellRect cellRect = CellRect.CenteredOn(x, rectRadius);
				int num = 0;
				CellRect.CellRectIterator iterator = cellRect.GetIterator();
				while (!iterator.Done())
				{
					if (!iterator.Current.InBounds(map))
					{
						return false;
					}
					if (iterator.Current.Standable(map) || iterator.Current.GetPlant(map) != null)
					{
						num++;
					}
					iterator.MoveNext();
				}
				return (float)num / (float)cellRect.Area >= 0.6f;
			}, map, out IntVec3 result))
			{
				return CellRect.CenteredOn(result, rectRadius);
			}
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map), map, out result))
			{
				return CellRect.CenteredOn(result, rectRadius);
			}
			return CellRect.CenteredOn(CellFinder.RandomCell(map), rectRadius).ClipInsideMap(map);
		}
	}
}
