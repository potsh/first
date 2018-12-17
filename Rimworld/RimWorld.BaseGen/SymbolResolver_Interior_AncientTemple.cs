using System.Collections.Generic;
using Verse;

namespace RimWorld.BaseGen
{
	public class SymbolResolver_Interior_AncientTemple : SymbolResolver
	{
		private const float MechanoidsChance = 0.65f;

		private static readonly IntRange MechanoidCountRange = new IntRange(1, 5);

		private static readonly IntRange HivesCountRange = new IntRange(1, 2);

		public override void Resolve(ResolveParams rp)
		{
			List<Thing> list = ThingSetMakerDefOf.MapGen_AncientTempleContents.root.Generate();
			for (int i = 0; i < list.Count; i++)
			{
				ResolveParams resolveParams = rp;
				resolveParams.singleThingToSpawn = list[i];
				BaseGen.symbolStack.Push("thing", resolveParams);
			}
			if (!Find.Storyteller.difficulty.peacefulTemples)
			{
				if (Rand.Chance(0.65f))
				{
					ResolveParams resolveParams2 = rp;
					int? mechanoidsCount = rp.mechanoidsCount;
					resolveParams2.mechanoidsCount = ((!mechanoidsCount.HasValue) ? MechanoidCountRange.RandomInRange : mechanoidsCount.Value);
					BaseGen.symbolStack.Push("randomMechanoidGroup", resolveParams2);
				}
				else
				{
					ResolveParams resolveParams3 = rp;
					int? hivesCount = rp.hivesCount;
					resolveParams3.hivesCount = ((!hivesCount.HasValue) ? HivesCountRange.RandomInRange : hivesCount.Value);
					BaseGen.symbolStack.Push("hives", resolveParams3);
				}
			}
			int? ancientTempleEntranceHeight = rp.ancientTempleEntranceHeight;
			int num = ancientTempleEntranceHeight.HasValue ? ancientTempleEntranceHeight.Value : 0;
			ResolveParams resolveParams4 = rp;
			resolveParams4.rect.minZ += num;
			BaseGen.symbolStack.Push("ancientShrinesGroup", resolveParams4);
		}
	}
}
