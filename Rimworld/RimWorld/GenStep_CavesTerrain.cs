using Verse;
using Verse.Noise;

namespace RimWorld
{
	public class GenStep_CavesTerrain : GenStep
	{
		private const float WaterFrequency = 0.08f;

		private const float GravelFrequency = 0.16f;

		private const float WaterThreshold = 0.93f;

		private const float GravelThreshold = 0.55f;

		public override int SeedPart => 1921024373;

		public override void Generate(Map map, GenStepParams parms)
		{
			if (Find.World.HasCaves(map.Tile))
			{
				Perlin perlin = new Perlin(0.079999998211860657, 2.0, 0.5, 6, Rand.Int, QualityMode.Medium);
				Perlin perlin2 = new Perlin(0.15999999642372131, 2.0, 0.5, 6, Rand.Int, QualityMode.Medium);
				MapGenFloatGrid caves = MapGenerator.Caves;
				foreach (IntVec3 allCell in map.AllCells)
				{
					IntVec3 current = allCell;
					if (!(caves[current] <= 0f))
					{
						TerrainDef terrain = current.GetTerrain(map);
						if (!terrain.IsRiver)
						{
							float num = (float)perlin.GetValue((double)current.x, 0.0, (double)current.z);
							float num2 = (float)perlin2.GetValue((double)current.x, 0.0, (double)current.z);
							if (num > 0.93f)
							{
								map.terrainGrid.SetTerrain(current, TerrainDefOf.WaterShallow);
							}
							else if (num2 > 0.55f)
							{
								map.terrainGrid.SetTerrain(current, TerrainDefOf.Gravel);
							}
						}
					}
				}
			}
		}
	}
}
